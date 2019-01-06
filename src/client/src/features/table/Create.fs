module MyTeam.Client.Table.Create

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.React
open MyTeam
open MyTeam.Client.Components
open MyTeam.Components
open MyTeam.Shared.Components
open MyTeam.Shared.Components.Forms
open Shared.Features.Table.Table
open Thoth.Json


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
                                              Text = "Legg til" 
                                              SubmittedText = "Lagt til"
                                              Endpoint = SubmitButton.Post (sprintf "/api/tables/%s/%i" props.Team state.Year.Value, ignore)
                                              IsDisabled = not <| isValid state.Year
                                              OnSubmit = fun _ -> Browser.location.replace(sprintf "/tabell/%s/%i" props.Team state.Year.Value) })                       
                        btn Default Normal [ OnClick handleClose ] [ str "Avbryt" ]                      
                  ]
            }                                                             
        
                  
            

let element model = ofType<CreateTable, _, _> model []
let node = Browser.document.getElementById createView

if not <| isNull node then 
    node.getAttribute (Interop.modelAttributeName)
    |> Decode.Auto.fromString<CreateModel>
    |> function 
    | Ok model -> ReactDom.render (element model, node)
    | Error e -> failwithf "Json deserialization failed: %O" e
