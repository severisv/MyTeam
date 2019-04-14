module Client.Fines.Add

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Components
open Client.Util.ReactHelpers
open Thoth.Json
open Shared.Components
open Shared.Components.Base
open Shared.Components.Forms
open Shared.Domain.Members
open Shared.Features.Fines.Common
open Shared.Features.Fines.Add
open Client.Util

type AddFineState = {
    Players: MemberWithTeamsAndRoles list
    Rates: RemedyRate list
    Form: AddFine option
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
                     { Form = None; Players = []; Rates = []; Error = None; Success = [] }
                     (Some <| { ComponentDidMount =
                                    fun (_, _, setState) ->
                                        Http.get "/api/members" Decode.Auto.fromString<MemberWithTeamsAndRoles list>
                                                  { OnSuccess = fun result -> setState (fun state props ->
                                                        { state with Players = result })                                                                                   
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
                        form [] [
                            h4 [] [ str "Registrer bot" ]
                            state.Error => Alerts.danger
                            formRow []
                                    [ str "Hvem" ]
                                    [ textInput [OnChange ignore
                                                 Value "" ] ]
                            formRow []
                                    [ str "Dato" ]
                                    [ textInput [OnChange ignore
                                                 Value "" ] ]                                    
                            formRow []
                                    [ str "Hva" ]
                                    [ textInput [OnChange ignore
                                                 Value "" ] ]
                                    
                            formRow []
                                    [ str "Tillegg" ]
                                    [ textInput [OnChange ignore
                                                 Value "" ] ]
                                    
                            formRow []
                                    [ str "Kommentar" ]
                                    [ textInput [OnChange ignore
                                                 Value "" ] ]
                            
                            formRow [] [] [
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
