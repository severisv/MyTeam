module Client.Fines.List

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Components
open Client.Util.ReactHelpers
open Thoth.Json
open Shared
open Shared.Components
open Shared.Components.Base
open Shared.Components.Layout
open Shared.Components.Nav
open Shared.Components.Forms
open Shared.Components.Tables
open Shared.Components.Currency
open Shared.Domain.Members
open Shared.Features.Admin.AddPlayers
open Shared.Features.Fines.Common
open Shared.Features.Fines.List
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
                                    [ str "År" ]
                                    [ textInput [OnChange ignore
                                                 Value "" ] ]
                           
                            SubmitButton.render
                                (fun o ->
                                    { o with
                                          Size = ButtonSize.Normal
                                          Text = "Legg til"
                                          Endpoint = SubmitButton.Post (sprintf "/api/fines/", None)
                                          OnSubmit = Some <| ignore })
                            btn [ OnClick !> handleClose ] [ str "Avbryt" ]
                        ]
                    )
        }

type State = {
    Fines : Fine list
 }

let isSelected year selectedMember =
    (=) (createUrl year selectedMember)

let handleDeleted setState fineId =
    fun str -> setState (fun (state : State) props -> { state with
                                                            Fines = state.Fines
                                                                    |> List.filter (fun fine -> fineId <> fine.Id) })

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
                                                        str fine.Description
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
                                                                                Text = "Ja"
                                                                                Endpoint = SubmitButton.Delete <| sprintf "/api/fines/%O" fine.Id
                                                                                OnSubmit = Some (handleClose >> handleDeleted setState fine.Id) })
                                                                          btn [ Lg; OnClick !>handleClose ] [ str "Nei" ]
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
                                            addFine
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
