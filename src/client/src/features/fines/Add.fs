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
open Shared.Domain.Members
open Shared.Features.Fines.Common
open Shared.Features.Fines.Add
open Client.Util
open Shared.Domain
open System

type AddFineForm = {
    MemberId: Guid option
    Date: DateTime
    RateId: Guid option
    ExtraRate: string
    Comment: string
}

type AddFineState = {
    Players: MemberWithTeamsAndRoles list
    Rates: RemedyRate list
    Form: AddFineForm
    Error: string option
    Success: string list    
}

let addFine =
    Modal.render
        { OpenButton = fun handleOpen -> linkButton handleOpen [ Icons.add ""; whitespace; str "Registrer bot" ]
          Content =
            fun handleClose ->
                 komponent<unit, AddFineState>
                     ()
                     { Form =
                         {   MemberId = None
                             Date = System.DateTime.Now
                             RateId = None
                             ExtraRate = ""
                             Comment = ""  }
                       Players = []; Rates = []; Error = None; Success = [] }
                     (Some <| { ComponentDidMount =
                                    fun (_, _, setState) ->
                                        Http.get "/api/members" Decode.Auto.fromString<MemberWithTeamsAndRoles list>
                                                  { OnSuccess = fun result -> setState (fun state props ->
                                                        { state with Players = result |> List.filter(fun p -> p.Details.Status = PlayerStatus.Aktiv) })                                                                                   
                                                    OnError = fun _ -> setState (fun state props ->
                                                        { state with Error = Some "Noe gikk galt ved lasting
                                                          av spillere. Prøv å laste siden på nytt" }) }
                                        Http.get "/api/fines/remedyrates" Decode.Auto.fromString<RemedyRate list>
                                                  { OnSuccess = fun result -> setState (fun state props ->
                                                        { state with Rates = result })                                                                                   
                                                    OnError = fun _ -> setState (fun state props ->
                                                        { state with Error = Some "Noe gikk galt ved lasting av bøtesatser. Prøv å laste siden på nytt" }) }
                                                
                                     })
                     (fun (props, state, setState) ->
                        let setFormValue update =
                            setState (fun state props -> { state with Form = update state.Form })
                                                    
                        form [Horizontal 3] [
                            h4 [] [ str "Registrer bot" ]
                            state.Error => Alerts.danger
                            formRow [Horizontal 3]
                                    [str "Hvem"]
                                    [selectInput [Value state.Form.MemberId
                                                  OnChange (fun e ->
                                                                let id = e.Value
                                                                setFormValue (fun form ->
                                                                    { form with MemberId = Some <| Guid.Parse id }))]
                                    (state.Players |> List.map (fun p ->
                                        { Name = p.Details.Name; Value = p.Details.Id   })) ]
                            formRow [Horizontal 3]
                                    [str "Dato" ]
                                    [dateInput [OnChange ignore
                                                Value state.Form.Date ] ]                                    
                            formRow [Horizontal 3]
                                    [str "Hva" ]
                                    [selectInput [Value state.Form.MemberId
                                                  OnChange (fun e ->
                                                                let id = e.Value
                                                                setFormValue (fun form ->
                                                                    { form with RateId = Some <| Guid.Parse id }))]
                                    (state.Rates |> List.map (fun r -> { Name = r.Name; Value = r.Id   })) ]
                                    
                            formRow [Horizontal 3]
                                    [str "Tillegg" ]
                                    [textInput [
                                                Validation [isNumber "Tillegg" state.Form.ExtraRate]
                                                OnChange (fun e ->
                                                                let value = e.Value
                                                                setFormValue (fun form ->
                                                                    { form with ExtraRate = value }))                                                 
                                                Placeholder "Eventuelt tillegg til normalsats" ] ]
                                    
                            formRow [Horizontal 3]
                                    [str "Kommentar" ]
                                    [textInput [OnChange (fun e ->
                                                                let value = e.Value
                                                                setFormValue (fun form ->
                                                                    { form with Comment = value }))
                                                Placeholder "Eventuell kommentar" ] ]
                            
                            formRow [Horizontal 3] [] [
                                SubmitButton.render
                                    (fun o ->
                                        { o with
                                              Size = ButtonSize.Normal
                                              Text = "Legg til"
                                              Endpoint = SubmitButton.Post (sprintf "/api/fines/", None)
                                              OnSubmit = Some <| ignore })
                                btn [ OnClick !> handleClose ] [ str "Avbryt" ]
                            ]
                           
                        ]
                    )
        }
