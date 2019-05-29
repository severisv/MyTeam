module Client.Fines.AddRemedyRate

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

type AddRateForm = {
    Amount: string
    Description: string
    Name: string
}

let emptyForm =
    { Amount = ""
      Description = ""
      Name = "" }

type AddFineState = {
    Form: AddRateForm
    Error: string option
    AddedRates: RemedyRate list
    IsTouched: bool
}


let addRemedyRate openButton onAdd onDelete =
    Modal.render
        { OpenButton = openButton
          Content =
            fun handleClose ->
                 komponent<unit, AddFineState>
                     ()
                     { Form = emptyForm
                       AddedRates = []
                       Error = None
                       IsTouched = false }
                     None
                     (fun (props, state, setState) ->
                        let setFormValue update =
                            setState (fun state props -> { state with Form = update state.Form; IsTouched = true })
                                                        
                        let validation =
                            Map [
                                "Name", isRequired "Navn" state.Form.Name
                                "Amount", (isNumber "Sats" state.Form.Amount) |> Result.bind(fun _ -> isRequired "Sats" state.Form.Amount)
                            ]                         
                                                  
                        let colNoBorder attr = col ([Attr (Style [BorderBottom "0"]); NoSort] |> List.append attr) []
                                                                        
                        fragment [] [
                            h4 [] [ str "Legg til bøtesatser" ]                            
                            table []
                                    [colNoBorder [Attr (Class "green"); Attr (Style [PaddingLeft 0])]
                                     colNoBorder []
                                     colNoBorder [Align Center]
                                     colNoBorder [Align Center]
                                     colNoBorder [Align Right; NoPadding]]
                                    (state.AddedRates
                                     |> List.map (fun rate ->
                                                        tableRow [] [                                                           
                                                            Icons.check
                                                            str <| rate.Name
                                                            str (rate.Description |> Strings.truncate 18)
                                                            Currency.currency [] rate.Amount
                                                            (SubmitButton.render
                                                                (fun o ->
                                                                    { o with
                                                                        Text = Icons.delete
                                                                        ButtonStyle = Danger
                                                                        Size = Sm
                                                                        Endpoint = SubmitButton.Delete <| sprintf "/api/remedyrates/%O" rate.Id
                                                                        OnSubmit = Some (fun _ ->
                                                                            setState (fun state props ->
                                                                                { state with AddedRates = state.AddedRates
                                                                                                          |> List.filter (fun f -> f.Id <> rate.Id) })
                                                                            onDelete rate.Id
                                                                            ) })) ]))                                                
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
                                                    Value state.Form.Name
                                                    IsTouched state.IsTouched ] ]
                                
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
                                                    Value state.Form.Amount
                                                    IsTouched state.IsTouched ] ]

                                
                                formRow [Horizontal 3] [] [
                                    SubmitButton.render
                                        (fun o ->
                                            { o with
                                                  IsDisabled = validation |> Map.toList |>  List.exists (function | (_, Error e) -> true | _ -> false)
                                                  Size = ButtonSize.Normal
                                                  Text = str "Legg til"
                                                  Endpoint = SubmitButton.Post (sprintf "/api/remedyrates",
                                                                                Some { Id = Guid.Empty
                                                                                       Name = state.Form.Name
                                                                                       Description = state.Form.Description
                                                                                       Amount = state.Form.Amount |> Number.tryParse |> Option.defaultValue 0 })
                                                  OnSubmit = Some (fun res ->
                                                                        Decode.Auto.fromString<RemedyRate> res
                                                                        |> function
                                                                        | Ok rate ->
                                                                            setState (fun state props ->
                                                                                { state with
                                                                                    AddedRates = [rate] |> List.append state.AddedRates 
                                                                                    Form = emptyForm
                                                                                    IsTouched = false })
                                                                            onAdd rate
                                                                        | Error e -> setState (fun state props -> { state with Error = Some e })) })
                                    btn [OnClick !> handleClose ] [str "Lukk"]
                                ]
                           ]
                        ]
                    )
        }
