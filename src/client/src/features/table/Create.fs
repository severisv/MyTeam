module Client.Table.Create

open Browser
open Fable.React
open Fable.React.Props
open Fable.Import
open Fable.React
open Shared
open Client.Components
open Shared.Components
open Shared.Components.Forms
open Shared.Components.Base
open Thoth.Json
open Shared.Util

type CreateModel = {
    Team: string
}
let createView = "create-table"

type State =
    { Year : int option }

let isValid =
    function
    | Some year -> year > 1970 && year < System.DateTime.Now.Year + 1
    | None -> false
    
type CreateTable(props) =
    inherit Component<CreateModel, State>(props)
    do base.setInitState ({ Year = None })
    override this.render() =
        let props = this.props
        let state = this.state
        let handleYearChange value = 
            Number.tryParse value
            |> function
            | Some n -> 
                this.setState (fun state props -> { state with Year = Some n })
            | None when not <| Strings.hasValue value -> this.setState (fun state props -> { state with Year = None })
            | None -> ()
             
        Modal.render 
            { OpenButton = fun handleOpen -> linkButton handleOpen [ Icons.add ""; whitespace; str "Legg til sesong" ]
              Content = 
                fun handleClose ->
                    form [] [ 
                        h4 [] [str "Legg til sesong"]
                        formRow [] 
                                [str "År"] 
                                [textInput [ OnChange (fun e -> handleYearChange e.Value)
                                             Value (state.Year |> Option.map (string) |> Option.defaultValue "" |> str) ]]
                            
                        SubmitButton.render 
                                        (fun o -> 
                                        { o with
                                              Size = Normal
                                              Text = str "Legg til" 
                                              SubmittedText = "Lagt til"
                                              Endpoint = SubmitButton.Post (sprintf "/api/tables/%s/%i" props.Team state.Year.Value, None)
                                              IsDisabled = not <| isValid state.Year
                                              OnSubmit = Some <| fun _ -> Dom.window.location.replace(sprintf "/tabell/%s/%i" props.Team state.Year.Value) })                       
                        btn [ OnClick !> handleClose ] [ str "Avbryt" ]                      
                  ]
            }                                                             
                
let element = ofType<CreateTable, _, _>
ReactHelpers.render Decode.Auto.fromString<CreateModel> createView element
