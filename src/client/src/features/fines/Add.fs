module Client.Fines.Add

open Fable.Helpers.React
open Fable.Helpers.React.Props
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
open Shared.Features.Fines.Common
open Shared.Features.Fines.Add
open Client.Util
open Shared.Domain
open System
open Shared

type AddFineForm = {
    MemberId: Guid option
    Date: DateTime option
    RateId: Guid option
    ExtraRate: string
    Comment: string
}

type AddFineState = {
    Players: MemberWithTeamsAndRoles list
    Rates: RemedyRate list
    Form: AddFineForm
    Error: string option
    AddedFines: AddFine list    
}

let getRateName state =
        state.Rates
           |> List.tryFind(fun r -> Some r.Id = state.Form.RateId)
           |> Option.map(fun r -> r.Name)
           |> Option.defaultValue ""
           
let getAmount state =
        (state.Rates
           |> List.tryFind(fun r -> Some r.Id = state.Form.RateId)
           |> Option.map(fun r -> r.Amount)
           |> Option.defaultValue 0)
        +
        (Number.tryParse state.Form.ExtraRate |> Option.defaultValue 0) 

let addFine openLink onAdd onDelete =
    Modal.render
        { OpenButton = openLink
          Content =
            fun handleClose ->
                 komponent<unit, AddFineState>
                     ()
                     { Form =
                         {   MemberId = None
                             Date = Some System.DateTime.Now
                             RateId = None
                             ExtraRate = ""
                             Comment = ""  }
                       Players = []; Rates = []; Error = None; AddedFines = [] }
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
                                        Http.get "/api/fines/remedyrates" Decode.Auto.fromString<RemedyRate list>
                                                  { OnSuccess = fun result -> setState (fun state props ->
                                                        { state with Rates = result; Form = { state.Form with RateId = result |> List.map(fun r -> r.Id) |> List.tryHead } })                                                                                   
                                                    OnError = fun _ -> setState (fun state props ->
                                                        { state with Error = Some "Noe gikk galt ved lasting av bøtesatser.
                                                          Prøv å laste siden på nytt" }) }
                                     })
                     (fun (props, state, setState) ->
                        let setFormValue update =
                            setState (fun state props -> { state with Form = update state.Form })
                                                        
                        let validation =
                            Map [
                                "Date", isSome "Dato" state.Form.Date
                                "MemberId", isSome "Hvem" state.Form.MemberId
                                "RateId", isSome "Hva" state.Form.RateId
                                "ExtraRate", isNumber "Tillegg" state.Form.ExtraRate
                            ]
                         
                        
                        let playerName id =
                            state.Players
                            |> List.tryFind (fun p -> p.Details.Id = id)
                            |> Option.map(fun p -> p.Details.Name)
                            |> Option.defaultValue ""
                            
                        let colNoBorder attr = col ([Attr (Style [BorderBottom "0"]); NoSort] |> List.append attr) []
                        
                        fragment [] [
                            h4 [] [ str "Registrer bøter" ]                            
                            table []
                                    [colNoBorder [Attr (Class "green"); Attr (Style [PaddingLeft 0])]
                                     colNoBorder []
                                     colNoBorder [Align Center]
                                     colNoBorder [Align Center]
                                     colNoBorder [Align Right; NoPadding]]
                                    (state.AddedFines
                                     |> List.map (fun fine ->
                                                        tableRow [] [                                                           
                                                            Icons.check
                                                            str <| playerName fine.MemberId 
                                                            str (fine.RateName |> Strings.truncate 18)
                                                            Currency.currency [] fine.Amount
                                                            (SubmitButton.render
                                                                (fun o ->
                                                                    { o with
                                                                        Text = Icons.delete
                                                                        ButtonStyle = Danger
                                                                        Size = Sm
                                                                        Endpoint = SubmitButton.Delete <| sprintf "/api/fines/%O" fine.Id
                                                                        OnSubmit = Some (fun _ ->
                                                                            setState (fun state props ->
                                                                                { state with AddedFines = state.AddedFines
                                                                                                          |> List.filter (fun f -> f.Id <> fine.Id) })
                                                                            onDelete fine.Id.Value
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
                                            { Name = p.Details.Name; Value = p.Details.Id   })) ]
                                        
                                formRow [Horizontal 3]
                                        [str "Dato" ]
                                        [dateInput [Value state.Form.Date
                                                    OnDateChange (fun date ->
                                                                        setFormValue (fun form ->
                                                                        { form with Date = date }))]]                                    
                                formRow [Horizontal 3]
                                        [str "Hva" ]
                                        [selectInput [OnChange (fun e ->
                                                                    let id = e.Value
                                                                    setFormValue (fun form ->
                                                                        { form with RateId = Some <| Guid.Parse id }))]
                                        (state.Rates |> List.map (fun r -> { Name = sprintf "%s (%i,-)" r.Name r.Amount
                                                                             Value = r.Id   })) ]
                                        
                                formRow [Horizontal 3]
                                        [str "Tillegg" ]
                                        [textInput [
                                                    Validation [validation.["ExtraRate"]]
                                                    OnChange (fun e ->
                                                                    let value = e.Value
                                                                    setFormValue (fun form ->
                                                                        { form with ExtraRate = value }))                                                 
                                                    Placeholder "Eventuelt tillegg til normalsats"
                                                    Value state.Form.ExtraRate] ]
                                        
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
                                                  Endpoint = SubmitButton.Post (sprintf "/api/fines",
                                                                                Some {  Id = None
                                                                                        MemberId = state.Form.MemberId.Value
                                                                                        Date = state.Form.Date.Value
                                                                                        RateId = state.Form.RateId.Value
                                                                                        RateName = getRateName state
                                                                                        Amount = getAmount state
                                                                                        Comment = state.Form.Comment })
                                                  OnSubmit = Some (fun res ->
                                                                        Decode.Auto.fromString<AddFine> res
                                                                        |> function
                                                                        | Ok fine ->
                                                                            setState (fun state props ->
                                                                                { state with
                                                                                    AddedFines = [fine]|> List.append state.AddedFines 
                                                                                    Form = { state.Form with ExtraRate = ""; Comment = "" } })
                                                                            onAdd ({ Id = fine.Id.Value
                                                                                     Member = state.Players
                                                                                              |> List.map (fun p -> p.Details)
                                                                                              |> List.find (fun p -> p.Id = fine.MemberId)                                                                                              
                                                                                     Description = fine.RateName
                                                                                     Amount = fine.Amount
                                                                                     Issued = fine.Date })
                                                                        | Error e -> setState (fun state props -> { state with Error = Some e })) })
                                    btn [OnClick !> handleClose ] [str "Lukk"]
                                ]
                           ]
                        ]
                    )
        }
