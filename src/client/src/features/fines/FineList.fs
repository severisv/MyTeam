module Client.Fines.List

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Components
open Shared.Util.ReactHelpers
open Thoth.Json
open Shared
open Shared.Components
open Shared.Components
open Shared.Components.Base
open Shared.Components.Layout
open Shared.Components.Nav
open Shared.Components.Tables
open Shared.Components.Currency
open Shared.Domain.Members
open Shared.Features.Fines.Common
open Shared.Features.Fines.List

type State = {
    Fines : Fine list
 }

let isSelected year selectedMember =
    (=) (createUrl year selectedMember)

let handleDeleted setState fineId =
        setState (fun (state : State) props ->
                { state with
                    Fines = state.Fines
                            |> List.filter (fun fine -> fineId <> fine.Id) })

let handleAdded year selectedMember setState fine =
    let add fine = setState (fun (state : State) props ->
                    { state with
                        Fines = state.Fines
                                |> List.append [fine]
                                |> List.sortByDescending (fun fine -> fine.Issued) })
    match (year, selectedMember) with
    | (Year year, _) when year <> fine.Issued.Year -> ()
    | (_, Member memberId) when memberId <> fine.Member.Id -> ()
    | _ -> add fine

let element props children =
        komponent<ListModel, State>
             props
             { Fines = props.Fines }
             None
             (fun (props, state, setState) ->
                let year = props.Year
                let selectedMember = props.SelectedMember
                let fines = state.Fines
                fragment [] [
                    mtMain [] [
                        block []
                            [ fineNav props.User props.Path ]

                        block [] [
                            props.User.IsInRole [Role.Botsjef] &?
                                fragment [] [
                                    div [Class "clearfix hidden-lg hidden-md u-margin-bottom"] [
                                        Add.addFine
                                            (fun handleOpen -> btn [OnClick handleOpen; Primary; Class "pull-right"] [ Icons.add ""; whitespace; str "Registrer bøter" ])
                                            (handleAdded year selectedMember setState) (handleDeleted setState)]]       
                            
                            selectNav []
                                ({ Items = props.Members |> List.map (fun m -> {  Text = string m.Name
                                                                                  Url = createUrl year (Member m.Id) })
                                   Footer = Some <| { Text = "- Alle spillere -"; Url = createUrl year AllMembers }
                                   IsSelected = isSelected year selectedMember })
                            navListMobile
                                ({ Items = props.Years |> List.map (fun year -> { Text = string year
                                                                                  Url = createUrl (Year year) selectedMember })
                                   Footer = Some <| { Text = "Total"; Url = createUrl AllYears selectedMember }
                                   IsSelected = isSelected year selectedMember })                         
                            
                            table []
                                [ col [ CellType Image; NoSort ] []
                                  col [ NoSort ] []
                                  col [ NoSort ] []
                                  col [ NoSort; Align Center ] [ Icons.fine "Beløp" ]
                                  col [ NoSort; Align Center ] [ Icons.calendar "Utstedt dato" ]
                                  col [ NoSort; Align Center; ExcludeWhen(not <| props.User.IsInRole [ Role.Botsjef ]) ] []
                                ]
                                (fines |> List.map (fun fine ->
                                                    let playerLink = a [ Href <| createUrl props.Year (Member fine.Member.Id) ]
                                                    tableRow [] [
                                                        playerLink [ img [ Src <| Image.getMember props.ImageOptions
                                                                                      (fun o -> { o with Height = Some 50
                                                                                                         Width = Some 50 })
                                                                                      fine.Member.Image fine.Member.FacebookId ] ]
                                                        playerLink [ str fine.Member.Name ]
                                                        fragment [] [str fine.Description
                                                                     Strings.hasValue fine.Comment &?
                                                                        Base.tooltip fine.Comment [Style [MarginLeft "0.5em"]] [Icons.infoCircle ""]]
                                                        currency [] fine.Amount
                                                        Date.formatShort fine.Issued |> str
                                                        Modal.render
                                                            { OpenButton = fun handleOpen -> linkButton handleOpen [ Icons.delete ]
                                                              Content =
                                                                fun handleClose ->
                                                                    div [] [
                                                                      h4 [] [ str <| sprintf "Er du sikker på at du vil slette '%s' til %s?" fine.Description fine.Member.FullName ]
                                                                      div [ Class "text-center" ] [
                                                                          br []
                                                                          SubmitButton.render
                                                                            (fun o ->
                                                                            { o with
                                                                                Text = str "Ja"
                                                                                Endpoint = SubmitButton.Delete <| sprintf "/api/fines/%O" fine.Id
                                                                                OnSubmit = Some (!> handleClose >> (fun _ -> handleDeleted setState fine.Id)) })
                                                                          btn [ Lg; OnClick !> handleClose ] [ str "Nei" ]
                                                                      ]
                                                                  ]
                                                            }
                                                    ]))
                            div [ Class "fine-total"
                                ] [ str "Total"
                                    b [] [ str <| sprintf "%i,-" (fines |> List.sumBy (fun f -> f.Amount)) ] ]
                        ]
                    ]
                    sidebar [] [
                        props.User.IsInRole [ Role.Botsjef ] &?
                                    block [] [
                                        navListBase [ Header <| str "Botsjef" ] [
                                            Add.addFine
                                                (fun handleOpen -> linkButton handleOpen [ Icons.add ""; whitespace; str "Registrer bøter" ])
                                                (handleAdded year selectedMember setState) (handleDeleted setState)
                                        ]
                                    ]
                        props.Years.Length > 0 &?
                            block [] [
                                navList ({ Header = "Sesonger"
                                           Items = props.Years |> List.map (fun year -> { Text = [ str <| string year ]
                                                                                          Url = createUrl (Year year) selectedMember })
                                           Footer = Some <| { Text = [ str "Total" ]; Url = createUrl AllYears selectedMember }
                                           IsSelected = isSelected year selectedMember })
                            ]
                    ]
                ]



        )

render Decode.Auto.fromString<ListModel> listView element
