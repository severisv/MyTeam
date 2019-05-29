module Client.Fines.EditRemedyRate

open System
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Components
open Fable.Import
open Shared.Util.ReactHelpers
open Thoth.Json
open Shared.Validation
open Shared.Components
open Shared.Components.Base
open Shared.Components.Forms
open Shared.Components.Tables
open Shared.Features.Fines.Common
open Shared

type EditRateForm = {
    Amount: string
    Description: string
    Name: string
}


type EditRateState = {
    Form: EditRateForm
    Error: string option
}


let editRemedyRate openButton onEdit rate =
    Modal.render
        { OpenButton = openButton
          Content =
            fun handleClose ->
                 komponent<RemedyRate, EditRateState>
                     rate
                     { Form = { Name = rate.Name; Amount = string rate.Amount; Description = rate.Description }
                       Error = None }
                     None
                     (fun (props, state, setState) ->
                        let setFormValue update =
                            setState (fun state props -> { state with Form = update state.Form })
                                                        
                        let validation =
                            Map [
                                "Name", isRequired "Navn" state.Form.Name
                                "Amount", (isNumber "Sats" state.Form.Amount) |> Result.bind(fun _ -> isRequired "Sats" state.Form.Amount)
                            ]                         
                                                  
                        let colNoBorder attr = col ([Attr (Style [BorderBottom "0"]); NoSort] |> List.append attr) []
                                                                        
                        fragment [] [
                            h4 [] [ str "Endre sats" ]        
                            form [Horizontal 3] [
                                state.Error => Alerts.danger        
                                formRow [Horizontal 3]
                                        [str "Navn" ]
                                        [textInput [OnChange (fun e ->
                                                                    let value = e.Value
                                                                    setFormValue (fun form ->
                                                                        { form with Name = value }))
                                                    Placeholder "For sen til trening"
                                                    Validation [validation.["Name"]]
                                                    Value state.Form.Name ] ]
                                
                                formRow [Horizontal 3]
                                        [str "Beskrivelse" ]
                                        [textInput [OnChange (fun e ->
                                                                    let value = e.Value
                                                                    setFormValue (fun form ->
                                                                        { form with Description = value }))
                                                    Placeholder "1 min = 10 kr"
                                                    Value state.Form.Description ] ]
                                
                                formRow [Horizontal 3]
                                        [str "Sats" ]
                                        [textInput [OnChange (fun e ->
                                                                    let value = e.Value
                                                                    setFormValue (fun form ->
                                                                        { form with Amount = value }))
                                                    Placeholder "47"
                                                    Validation [validation.["Amount"]]
                                                    Value state.Form.Amount] ]

                                
                                formRow [Horizontal 3] [] [
                                    SubmitButton.render
                                        (fun o ->
                                            { o with
                                                  IsDisabled = validation |> Map.toList |>  List.exists (function | (_, Error e) -> true | _ -> false)
                                                  Size = ButtonSize.Normal
                                                  Text = str "Lagre"
                                                  Endpoint = SubmitButton.Put (sprintf "/api/remedyrates",
                                                                                Some { rate with 
                                                                                           Name = state.Form.Name
                                                                                           Description = state.Form.Description
                                                                                           Amount = state.Form.Amount |> Number.tryParse |> Option.defaultValue 0 })
                                                  OnSubmit = Some (fun res ->
                                                                        Decode.Auto.fromString<RemedyRate> res
                                                                        |> function
                                                                        | Ok rate ->
                                                                            handleClose()
                                                                            onEdit rate
                                                                        | Error e -> setState (fun state props -> { state with Error = Some e })) })
                                    btn [OnClick !> handleClose ] [str "Avbryt"]
                                ]
                           ]
                        ]
                    )
        }
