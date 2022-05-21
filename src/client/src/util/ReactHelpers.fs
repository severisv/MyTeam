module Shared.Util.ReactHelpers

open Fable.Import
open Fable.React
open Shared
open Fable.Core
open JsInterop
open Browser.Dom
open Browser.Types


let propsVariableName = Strings.replace "-" "__"
let componentDataAttribute = "data-props"


let render deserializeFn id comp =
    let variableName = propsVariableName id

    Browser.Dom.document.getElementById id
    |> fun node ->
        if not <| isNull node then
            !!Browser.Dom.window?(variableName)
            |> deserializeFn
            |> function
                | Ok model -> ReactDom.render (comp model [], node)
                | Error e -> failwithf "Json deserialization failed: %O" e


let render2 deserializeFn id comp =
    let variableName = propsVariableName id

    Browser.Dom.document.getElementById id
    |> fun node ->
        if not <| isNull node then
            !!Browser.Dom.window?(variableName)
            |> deserializeFn
            |> function
                | Ok model -> ReactDom.render (comp model, node)
                | Error e -> failwithf "Json deserialization failed: %O" e

let hydrate elementId deserializeFn comp =
    Browser.Dom.document.getElementById elementId
    |> fun node ->
        if not <| isNull node then
            !!Browser.Dom.window?(propsVariableName elementId)
            |> deserializeFn
            |> function
                | Ok model -> ReactDom.hydrate (comp model [], node)
                | Error e -> failwithf "Json deserialization failed: %O" e

let hydrateView elementId deserializeFn comp =
#if FABLE_COMPILER
    Browser.Dom.document.getElementById elementId
    |> fun node ->
        if not <| isNull node then
            !!Browser.Dom.window?(propsVariableName elementId)
            |> deserializeFn
            |> function
                | Ok model -> ReactDom.hydrate (comp (model), node)
                | Error e -> failwithf "Json deserialization failed: %O" e
#endif
    ()


let seqOfNodeList<'T> (nodes: Browser.Types.NodeListOf<'T>) =
    seq {
        for i in [ 0 .. nodes.length - 1 ] do
            yield nodes.[i]
    }

let hydrateComponent className deserializeFn comp =
    document.getElementsByClassName className
    |> seqOfNodeList
    |> Seq.iter (fun node ->
        if not <| isNull node then
            let parent = node.parentElement

            parent.attributes.getNamedItem componentDataAttribute
            |> fun a -> a.value
            |> deserializeFn
            |> function
                | Ok model -> ReactDom.hydrate (comp (model), parent)
                | Error e -> failwithf "Json deserialization failed: %O" e)

    ()

type OutProps<'Props, 'State> = 'Props * 'State * (('State -> 'Props -> 'State) -> unit)

type UpdateFn<'TState> = 'TState -> 'TState

type Lifecycles<'Props, 'State> =
    { ComponentDidMount: OutProps<'Props, 'State> -> unit }

type ComponentProps<'Props, 'State> =
    { Props: 'Props
      InitialState: 'State
      Lifecycles: Lifecycles<'Props, 'State> option
      Children: OutProps<'Props, 'State> -> ReactElement }

type StateProvider<'Props, 'State>(props) =
    inherit Component<ComponentProps<'Props, 'State>, 'State>(props)
    do base.setInitState props.InitialState

    override this.componentDidMount() =
        props.Lifecycles
        |> Option.map (fun l -> l.ComponentDidMount(this.props.Props, this.state, this.update))
        |> Option.defaultValue ()

    member this.update updateFn =
        this.setState (fun state props -> updateFn state props.Props)

    override this.render() =

        props.Children(this.props.Props, this.state, this.update)

let komponent<'Props, 'State> props initalState lifeCycles (children: OutProps<'Props, 'State> -> ReactElement) =
    ofType<StateProvider<'Props, 'State>, _, _>
        { Props = props
          InitialState = initalState
          Lifecycles = lifeCycles
          Children = children }
        []
