module Shared.Util.ReactHelpers

open Fable.Import
open Fable.Import.React
open Fable.Helpers.React
open Shared

let render deserializeFn id comp =
    Browser.document.getElementById id
    |> fun node ->

        if not <| isNull node then
            node.getAttribute (Interop.modelAttributeName)
            |> deserializeFn
            |> function
             | Ok model -> ReactDom.render (comp model [], node)
             | Error e -> failwithf "Json deserialization failed: %O" e




type OutProps<'Props, 'State> =
    'Props * 'State * (('State -> 'Props -> 'State) -> unit)

type UpdateFn<'TState> = 'TState -> 'TState

type Lifecycles<'Props, 'State> = {
    ComponentDidMount : OutProps<'Props, 'State> -> unit }

type ComponentProps<'Props, 'State> = {
    Props : 'Props
    InitialState : 'State
    Lifecycles : Lifecycles<'Props, 'State> option
    Children : OutProps<'Props, 'State> -> ReactElement
 }

type StateProvider<'Props, 'State>(props) =
    inherit Component<ComponentProps<'Props, 'State>, 'State>(props)
    do base.setInitState props.InitialState

    override this.componentDidMount() =
        props.Lifecycles
        |> Option.map (fun l -> l.ComponentDidMount(this.props.Props, this.state, this.update))
        |> Option.defaultValue()

    member this.update updateFn =
        this.setState (fun state props -> updateFn state props.Props)

    override this.render() =

        props.Children(this.props.Props, this.state, this.update)

let komponent<'Props, 'State> props initalState lifeCycles (children : OutProps<'Props, 'State> -> ReactElement) =
    ofType<StateProvider<'Props, 'State>, _, _>
            {
                Props = props
                InitialState = initalState
                Lifecycles = lifeCycles
                Children = children
            }
            []