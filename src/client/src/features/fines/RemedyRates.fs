module Client.Fines.RemedyRates

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Components
open Client.Fines
open Shared.Util.ReactHelpers
open Thoth.Json
open Shared
open Shared.Components
open Shared.Components.Base
open Shared.Components.Layout
open Shared.Components.Nav
open Shared.Components.Tables
open Shared.Components.Currency
open Shared.Domain.Members
open Shared.Features.Fines.Common
open Shared.Features.Fines.RemedyRates

type State = {
    Rates : RemedyRate list
 }

let handleDeleted setState rateId =
        setState (fun (state : State) props ->
                { state with
                    Rates = state.Rates
                            |> List.filter (fun rate -> rateId <> rate.Id) })

let handleAdded setState remedyRate =
        setState (fun (state: State) _ ->
                    { state with
                        Rates = state.Rates
                                |> List.append [remedyRate]
                                |> List.sortBy (fun remedyRate -> remedyRate.Name) })

let handleEdited setState (remedyRate: RemedyRate) =
        setState (fun (state: State) _ ->
                    { state with
                        Rates = state.Rates
                                |> List.filter (fun rate -> rate.Id <> remedyRate.Id)
                                |> List.append [remedyRate]
                                |> List.sortBy (fun remedyRate -> remedyRate.Name) })

let element props children =
        komponent<RemedyRatesModel, State>
             props
             { Rates = [] }
             (Some { ComponentDidMount = fun (props, state, setState)  ->
                 setState(fun state props -> { state with Rates = props.Rates  })})
             (fun (props, state, setState) ->
              
                let rates = state.Rates
                fragment [] [
                    mtMain [] [
                        block []
                            [ fineNav props.User props.Path ]

                        block [] [
                            props.User.IsInRole [Role.Botsjef] &?
                                fragment [] [
                                    div [Class "clearfix hidden-lg hidden-md u-margin-bottom"] [
                                        AddRemedyRate.addRemedyRate
                                            (fun handleOpen -> btn [OnClick handleOpen; Primary; Class "pull-right"] [ Icons.add ""; whitespace; str "Legg til satser" ])
                                            (handleAdded setState) (handleDeleted setState)]]       
                            
                            table []
                                [
                                  col [] [str "Navn"]
                                  col [] [str "Beskrivelse"]
                                  col [NoSort] [str "Sats"]
                                  col [NoSort; NoPadding; ExcludeWhen(not <| props.User.IsInRole [Role.Botsjef]) ] []
                                  col [NoSort; NoPadding; ExcludeWhen(not <| props.User.IsInRole [Role.Botsjef]) ] []
                                ]
                                (rates |> List.map (fun rate ->
                                                    tableRow [] [
                                                        str rate.Name
                                                        str rate.Description
                                                        currency [] rate.Amount
                                                        EditRemedyRate.editRemedyRate
                                                            (fun handleOpen -> linkButton handleOpen [Icons.edit ""])
                                                            (handleEdited setState) rate
                                                        Modal.render
                                                            { OpenButton = fun handleOpen -> linkButton handleOpen [Icons.delete]
                                                              Content =
                                                                fun handleClose ->
                                                                    div [] [
                                                                      h4 [] [ str <| sprintf "Er du sikker på at du vil slette '%s'?" rate.Name ]
                                                                      div [Class "text-center"] [
                                                                          br []
                                                                          SubmitButton.render
                                                                            (fun o ->
                                                                            { o with
                                                                                ButtonStyle = Danger
                                                                                Text = str "Slett"
                                                                                Endpoint = SubmitButton.Delete <| sprintf "/api/remedyrates/%O" rate.Id
                                                                                OnSubmit = Some (!> handleClose >> (fun _ -> handleDeleted setState rate.Id)) })
                                                                          btn [Lg; OnClick !> handleClose ] [ str "Avbryt" ]
                                                                      ]
                                                                  ]
                                                            }                                                       
                                                    ]))                            
                        ]
                    ]
                    sidebar [] [
                        props.User.IsInRole [ Role.Botsjef ] &?
                                    block [] [
                                        navListBase [Header <| str "Botsjef" ] [
                                            AddRemedyRate.addRemedyRate
                                                (fun handleOpen -> linkButton handleOpen [Icons.add ""; whitespace; str "Legg til satser" ])
                                                (handleAdded setState) (handleDeleted setState)
                                        ]]                       
                    ]
                ])

render Decode.Auto.fromString<RemedyRatesModel> ratesView element
