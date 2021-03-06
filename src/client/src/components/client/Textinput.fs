module Client.Components.Textinput

open Fable.React
open Fable.React.Props
open Fable.Import
open Shared.Components
open Shared.Components.Input
open Shared.Components.Base
open Client.Util
open Fetch.Types

type State =
    { IsPosting : bool
      IsTouched : bool
      Error : bool }

type Props =
    { Value : string
      Url : string
      OnChange : string -> unit }

type Textinput(props) =
    inherit Component<Props, State>(props)
    
    do 
        base.setInitState ({ Error = false
                             IsTouched = false
                             IsPosting = false })
    
    let mutable timeout = Browser.Dom.window.setTimeout (ignore, 0, [])
    
    member this.debounce fn wait =
        Browser.Dom.window.clearTimeout timeout
        timeout <- Browser.Dom.window.setTimeout (fn, wait, [])
    
    override this.render() =
        let props = this.props
        
        let handleChange value =
            this.debounce (fun () -> 
                this.setState (fun state props -> 
                    { state with Error = false
                                 IsTouched = true
                                 IsPosting = true })
                promise { 
                    let payload : StringPayload = { Value = value }
                    let! res = Http.sendRecord HttpMethod.PUT props.Url payload []
                    if not res.Ok then 
                        failwithf "Received %O from server: %O" res.Status res.StatusText
                    this.setState (fun state props -> { state with IsPosting = false })
                    props.OnChange value
                }
                |> Promise.catch (fun e -> 
                       Browser.Dom.console.error(sprintf "%O" e)
                       this.setState (fun state props -> 
                           { state with Error = true
                                        IsPosting = false }))
                |> Promise.start) 750
        div [ Class "input-text" ] [ input [ Class "form-control"
                                             Type "text"
                                             DefaultValue props.Value
                                             OnChange(fun input -> handleChange input.Value) ]
                                     (match this.state with
                                      | { IsPosting = true } -> Icons.spinner
                                      | { Error = true } -> Icons.warning
                                      | { IsTouched = true } -> Icons.check
                                      | _ -> empty) ]

let render model = ofType<Textinput, _, _> model []
