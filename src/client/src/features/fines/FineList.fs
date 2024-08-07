module Client.Fines.List

open Fable.React
open Fable.React.Props
open Client.Components
open Fable.Import
open Shared.Util.ReactHelpers
open Thoth.Json
open Shared
open Shared.Components
open Shared.Components.Links
open Shared.Components.Base
open Shared.Components.Layout
open Shared.Components.Nav
open Shared.Components.Tables
open Shared.Components.Currency
open Shared.Domain.Members
open Shared.Features.Fines.Common
open Shared.Image
open System

let createUrl year memberId =
    let year =
        match year with
        | AllYears -> "total"
        | Year year -> string year

    let memberId =
        match memberId with
        | Member id -> sprintf "/%O" id
        | AllMembers -> ""

    sprintf "/intern/boter/vis/%s%s" year memberId

type ListModel =
    { ImageOptions: CloudinaryOptions
      Fines: Fine list
      Year: SelectedYear
      SelectedMember: SelectedMember
      User: User
      Path: string
      Years: int list
      Members: Member list }


type State = { Fines: Fine list }

let isSelected year selectedMember = (=) (createUrl year selectedMember)

let handleDeleted setState fineId =
    setState (fun (state: State) props ->
        { state with
            Fines =
                state.Fines
                |> List.filter (fun fine -> fineId <> fine.Id) })

let handleAdded year selectedMember setState fine =
    match (year, selectedMember) with
    | (Year year, _) when year <> fine.Issued.Year -> ()
    | (_, Member memberId) when memberId <> fine.Member.Id -> ()
    | _ ->
        setState (fun (state: State) _ ->
            { state with
                Fines =
                    state.Fines
                    |> List.append [ fine ]
                    |> List.sortByDescending (fun fine -> fine.Issued) })

let containerId = "list-fines"

let element props children =
    komponent<ListModel, State> props { Fines = props.Fines } None (fun (props, state, setState) ->
        let year = props.Year
        let selectedMember = props.SelectedMember
        let fines = state.Fines

        fragment [] [
            mtMain [] [
                block [] [
                    fineNav props.User props.Path
                ]

                block [] [
                    props.User.IsInRole [ Role.Botsjef ]
                    &? fragment [] [
                        div [ Class "clearfix hidden-lg hidden-md u-margin-bottom" ] [
                            Add.addFine
                                (fun handleOpen ->
                                    btn [ OnClick handleOpen
                                          Primary
                                          Class "pull-right" ] [
                                        Icons.add ""
                                        whitespace
                                        str "Ny bot"
                                    ])
                                (handleAdded year selectedMember setState)
                                (handleDeleted setState)
                        ]
                       ]

                    selectNav
                        []
                        ({ Items =
                            props.Members
                            |> List.map (fun m ->
                                { Text = string m.Name
                                  Url = createUrl year (Member m.Id) })
                           Footer =
                             Some
                             <| { Text = "- Alle spillere -"
                                  Url = createUrl year AllMembers }
                           IsSelected = isSelected year selectedMember })
                    navListMobile (
                        { Items =
                            props.Years
                            |> List.map (fun year ->
                                { Text = string year
                                  Url = createUrl (Year year) selectedMember })
                          Footer =
                            Some
                            <| { Text = "Total"
                                 Url = createUrl AllYears selectedMember }
                          IsSelected = isSelected year selectedMember }
                    )

                    table
                        []
                        [ col [ CellType Image; NoSort ] []
                          col [ NoSort ] []
                          col [ NoSort ] []
                          col [ NoSort; Align Center ] [
                              Icons.fine "Beløp"
                          ]
                          col [ NoSort; Align Center ] [
                              Icons.calendar "Utstedt dato"
                          ]
                          col [ NoSort
                                Align Center
                                ExcludeWhen(not <| props.User.IsInRole [ Role.Botsjef ]) ] [] ]
                        (fines
                         |> List.map (fun fine ->
                             printf "%O" fine.Comment

                             let playerLink =
                                 a [
                                     Href
                                     <| createUrl props.Year (Member fine.Member.Id)
                                 ]

                             tableRow [] [
                                 playerLink [
                                     img [
                                         Src
                                         <| Image.getMember
                                             props.ImageOptions
                                             (fun o ->
                                                 { o with
                                                     Height = Some 50
                                                     Width = Some 50 })
                                             fine.Member.Image
                                             fine.Member.FacebookId
                                     ]
                                 ]
                                 playerLink [ str fine.Member.Name ]
                                 fragment [] [
                                     str fine.Description
                                     Strings.hasValue fine.Comment
                                     &? Base.tooltip fine.Comment [ Style [ MarginLeft "0.5em" ] ] [ Icons.infoCircle "" ]
                                 ]
                                 currency [] fine.Amount
                                 fine.Issued
                                 |> (if props.Year = AllYears then
                                         Date.formatLong
                                     else
                                         Date.formatShort)
                                 |> str
                                 Modal.modal
                                     { OpenButton =
                                         fun handleOpen ->
                                             linkButton [ OnClick handleOpen ] [
                                                 Icons.delete
                                             ]
                                       Content =
                                         fun handleClose ->
                                             div [] [
                                                 h4 [] [
                                                     str
                                                     <| sprintf
                                                         "Er du sikker på at du vil slette '%s' til %s?"
                                                         fine.Description
                                                         fine.Member.FullName
                                                 ]
                                                 div [ Class "text-center" ] [
                                                     br []
                                                     Send.sendElement (fun o ->
                                                         { o with
                                                             SendElement = btn, [ Danger; Lg ], [ str "Slett" ]
                                                             SentElement = span, [], []
                                                             Endpoint = Send.Delete <| sprintf "/api/fines/%O" fine.Id
                                                             OnSubmit =
                                                                 Some(
                                                                     !>handleClose
                                                                     >> (fun _ -> handleDeleted setState fine.Id)
                                                                 ) })
                                                     btn [ Lg; OnClick !>handleClose ] [
                                                         str "Avbryt"
                                                     ]
                                                 ]
                                             ] }
                             ]))
                    div [ Class "fine-total" ] [
                        str "Total"
                        b [] [
                            str
                            <| sprintf "%i,-" (fines |> List.sumBy (fun f -> f.Amount))
                        ]
                    ]
                ]
            ]
            sidebar [] [
                props.User.IsInRole [ Role.Botsjef ]
                &? block [] [
                    navListBase [ Header <| str "Botsjef" ] [
                        Add.addFine
                            (fun handleOpen ->
                                linkButton [ OnClick handleOpen ] [
                                    Icons.add ""
                                    whitespace
                                    str "Ny bot"
                                ])
                            (handleAdded year selectedMember setState)
                            (handleDeleted setState)
                    ]
                   ]
                props.Years.Length > 0
                &? block [] [
                    navList (
                        { Header = "Sesonger"
                          Items =
                            props.Years
                            |> List.map (fun year ->
                                { Text = [ str <| string year ]
                                  Url = createUrl (Year year) selectedMember })
                          Footer =
                            Some
                            <| { Text = [ str "Total" ]
                                 Url = createUrl AllYears selectedMember }
                          IsSelected = isSelected year selectedMember }
                    )
                   ]
            ]
        ]



    )

hydrate containerId Decode.Auto.fromString<ListModel> element
