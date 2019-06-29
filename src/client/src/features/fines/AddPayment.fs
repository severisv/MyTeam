module Client.Fines.AddPayment

open Fable.React
open Fable.React.Props
open Client.Components
open Shared.Util.ReactHelpers
open Thoth.Json
open Shared.Validation
open Shared.Components
open Shared.Components.Base
open Shared.Components.Forms
open Shared.Components.Tables
open Shared.Components.Datepicker
open Shared.Domain.Members
open Client.Util
open Shared.Domain
open System
open Shared

[<CLIMutable>]
type AddPayment = {
    Id: Guid option
    MemberId: Guid
    Date: DateTime
    Amount: int
    Comment: string
}

type Payment = {
    Id: Guid
    Member: Member
    Comment: string
    Amount: int
    Date: DateTime
 }

type AddPaymentForm = {
    MemberId: Guid option
    Date: DateTime option
    Amount: string
    Comment: string
    IsTouched: bool
}

type AddPaymentState = {
    Players: MemberWithTeamsAndRoles list
    Form: AddPaymentForm
    Error: string option
    AddedPayments: AddPayment list
}

let element openLink onAdd onDelete =
    Modal.render
        { OpenButton = openLink
          Content =
            fun handleClose ->
                 komponent<unit, AddPaymentState>
                     ()
                     { Form =
                         {   MemberId = None
                             Date = Some System.DateTime.Now
                             Amount = ""
                             Comment = ""
                             IsTouched = false }
                       Players = []; Error = None; AddedPayments = [] }
                     (Some <| { ComponentDidMount =
                                    fun (_, _, setState) ->
                                        Http.get "/api/members" Decode.Auto.fromString<MemberWithTeamsAndRoles list>
                                                  { OnSuccess = fun result -> setState (fun state props ->
                                                        let result = result |> List.filter(fun p -> p.Details.Status = PlayerStatus.Aktiv)
                                                        { state with Players = result
                                                                     Form = { state.Form with MemberId = result |> List.map(fun r -> r.Details.Id) |> List.tryHead } } )                                                                                   
                                                    OnError = fun _ -> setState (fun state props ->
                                                        { state with Error = Some "Noe gikk galt ved lasting
                                                          av spillere. Prøv å laste siden på nytt" }) }                                        
                                     })
                     (fun (props, state, setState) ->
                        let setFormValue update =
                            setState (fun state props -> { state with Form = update state.Form })
                                                        
                        let validation =
                            Map [
                                "Date", isSome "Dato" state.Form.Date
                                "MemberId", isSome "Hvem" state.Form.MemberId
                                "Amount", (isNumber "Beløp" state.Form.Amount) |> Result.bind(fun _ -> isRequired "Sats" state.Form.Amount)
                            ]
                         
                        
                        let playerName id =
                            state.Players
                            |> List.tryFind (fun p -> p.Details.Id = id)
                            |> Option.map(fun p -> p.Details.Name)
                            |> Option.defaultValue ""
                            
                        let colNoBorder attr = col ([Attr (Style [BorderBottom "0"]); NoSort] |> List.append attr) []
                        
                        fragment [] [
                            h4 [] [ str "Registrer innbetalinger" ]                            
                            table []
                                    [colNoBorder [Attr (Class "green"); Attr (Style [PaddingLeft 0])]
                                     colNoBorder []
                                     colNoBorder [Align Center]
                                     colNoBorder [Align Center]
                                     colNoBorder [Align Right; NoPadding]]
                                    (state.AddedPayments
                                     |> List.map (fun payment ->
                                                        tableRow [] [                                                           
                                                            Icons.check
                                                            str <| playerName payment.MemberId 
                                                            Currency.currency [] payment.Amount
                                                            str (payment.Date |> Date.formatLong)
                                                            (SubmitButton.render
                                                                (fun o ->
                                                                    { o with
                                                                        Text = Icons.delete
                                                                        ButtonStyle = Danger
                                                                        Size = Sm
                                                                        Endpoint = SubmitButton.Delete <| sprintf "/api/payments/%O" payment.Id
                                                                        OnSubmit = Some (fun _ ->
                                                                            setState (fun state props ->
                                                                                { state with AddedPayments = state.AddedPayments
                                                                                                          |> List.filter (fun f -> f.Id <> payment.Id) })
                                                                            onDelete payment.Id.Value
                                                                            ) })) ]))                                                
                            form [Horizontal 3] [
                                state.Error => Alerts.danger
                                formRow [Horizontal 3]
                                        [str "Hvem"]
                                        [selectInput [OnChange (fun e ->
                                                                    let id = e.Value
                                                                    setFormValue (fun form ->
                                                                        { form with MemberId = Some <| Guid.Parse id }))]
                                        (state.Players |> List.map (fun p ->
                                            { Name = p.Details.Name; Value = p.Details.Id })) ]
                                        
                                formRow [Horizontal 3]
                                        [str "Dato" ]
                                        [dateInput [Value state.Form.Date
                                                    OnDateChange (fun date ->
                                                                        setFormValue (fun form ->
                                                                        { form with Date = date }))]]     
                                formRow [Horizontal 3]
                                        [str "Beløp" ]
                                        [textInput [
                                                    Validation [validation.["Amount"]]
                                                    OnChange (fun e ->
                                                                    let value = e.Value
                                                                    setFormValue (fun form ->
                                                                        { form with Amount = value; IsTouched = true }))                                                 
                                                    Placeholder "137"
                                                    Value state.Form.Amount
                                                    IsTouched state.Form.IsTouched] ]
                                        
                                formRow [Horizontal 3]
                                        [str "Kommentar" ]
                                        [textInput [OnChange (fun e ->
                                                                    let value = e.Value
                                                                    setFormValue (fun form ->
                                                                        { form with Comment = value }))
                                                    Placeholder "Eventuell kommentar"
                                                    Value state.Form.Comment ] ]
                                
                                formRow [Horizontal 3] [] [
                                    SubmitButton.render
                                        (fun o ->
                                            { o with
                                                  IsDisabled = validation |> Map.toList |>  List.exists (function | (_, Error e) -> true | _ -> false)
                                                  Size = ButtonSize.Normal
                                                  Text = str "Legg til"
                                                  Endpoint = SubmitButton.Post (sprintf "/api/payments",
                                                                                Some (fun () ->
                                                                                        Encode.Auto.toString(0,
                                                                                         {  Id = None
                                                                                            MemberId = state.Form.MemberId.Value
                                                                                            Date = state.Form.Date.Value
                                                                                            Amount = Number.tryParse state.Form.Amount |> Option.defaultValue 0
                                                                                            Comment = state.Form.Comment })))
                                                  OnSubmit = Some (fun res ->
                                                                        Decode.Auto.fromString<AddPayment> res
                                                                        |> function
                                                                        | Ok payment ->
                                                                            setState (fun state props ->
                                                                                { state with
                                                                                    AddedPayments = [payment]|> List.append state.AddedPayments 
                                                                                    Form = { state.Form with Amount = ""; Comment = ""; IsTouched = false }})
                                                                            onAdd ({ Id = payment.Id.Value
                                                                                     Member = state.Players
                                                                                              |> List.map (fun p -> p.Details)
                                                                                              |> List.find (fun p -> p.Id = payment.MemberId)                                                                                              
                                                                                     Amount = payment.Amount
                                                                                     Comment = payment.Comment
                                                                                     Date = payment.Date })
                                                                        | Error e -> setState (fun state props -> { state with Error = Some e })) })
                                    btn [OnClick !> handleClose ] [str "Lukk"]
                                ]
                           ]
                        ]
                    )
        }
