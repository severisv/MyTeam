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
open Shared.Components.Links
open Thoth.Json
open Shared.Util
open Feliz



type CreateModel = { Team: string }
let createView = "create-table"

type State = { Year: int option }

let isValid =
    function
    | Some year -> year > 1970 && year < System.DateTime.Now.Year + 2
    | None -> false


[<ReactComponent>]
let CreateTable (props: CreateModel) =   

        let (state, setState) = React.useState( { Year = None })

        let handleYearChange value =
            Number.tryParse value
            |> function
            | Some n -> setState ({ state with Year = Some n })
            | None when not <| Strings.hasValue value -> setState ({ state with Year = None })
            | None -> ()

        Modal.modal
            { OpenButton =
                  fun handleOpen ->
                      linkButton [ OnClick handleOpen
                                   Style [ PaddingLeft "15px" ] ] [
                          Icons.add ""
                          whitespace
                          str "Legg til sesong"
                      ]
              Content =
                  fun handleClose ->
                      form [] [
                          h4 [] [ str "Legg til sesong" ]
                          formRow
                              []
                              [ str "År" ]
                              [ textInput [ OnChange(fun e -> handleYearChange e.Value)
                                            Value
                                                (state.Year
                                                 |> Option.map (string)
                                                 |> Option.defaultValue ""
                                                 |> str) ] ]

                          Send.sendElement (fun o ->
                              { o with
                                    SendElement = btn, [ ButtonSize.Normal; Primary ], [ str "Legg til" ]
                                    SentElement = btn, [ ButtonSize.Normal; Success ], [ str "Lagt til" ]
                                    Endpoint = Send.Post(sprintf "/api/tables/%s/%O" props.Team state.Year, None)
                                    IsDisabled = not <| isValid state.Year
                                    OnSubmit =
                                        Some
                                        <| fun _ ->
                                            Dom.window.location.replace
                                                (sprintf "/tabell/%s/%i" props.Team state.Year.Value) })
                          btn [ OnClick !>handleClose ] [
                              str "Avbryt"
                          ]
                      ] }

ReactHelpers.render2 Decode.Auto.fromString<CreateModel> createView CreateTable

