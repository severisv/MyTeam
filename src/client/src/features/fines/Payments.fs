module Client.Fines.Payments

open Fable.React
open Fable.React.Props
open Client.Components
open Client.Components.Textarea
open Shared.Util.ReactHelpers
open Thoth.Json
open Shared
open Shared.Components
open Shared.Components.Base
open Shared.Components.Layout
open Shared.Components.Nav
open Shared.Components.Tables
open Shared.Components.Currency
open Shared.Components.Links
open Shared.Domain.Members
open Shared.Features.Fines.Common
open System
open Shared.Image
open Client.Fines.AddPayment

let createUrl year memberId =
        let year = match year with
                    | AllYears -> "total"
                    | Year year -> string year
        let memberId = match memberId with
                        | Member id -> sprintf "/%O" id
                        | AllMembers -> ""            
        sprintf "/intern/boter/innbetalinger/%s%s" year memberId




type PaymentsModel = {
    ImageOptions: CloudinaryOptions
    Payments: Payment list
    Year: SelectedYear
    SelectedMember: SelectedMember
    User: User
    Path: string
    Years: int list
    Members: Member list
    PaymentInformation: string
}


let paymentsView = "list-payments"


type State = {
    Payments : Payment list
 }

let isSelected year selectedMember =
    (=) (createUrl year selectedMember)

let handleDeleted setState paymentId =
        setState (fun (state : State) props ->
                { state with
                    Payments = state.Payments
                            |> List.filter (fun payment -> paymentId <> payment.Id) })

let handleAdded year selectedMember setState (payment: Payment) =
    match (year, selectedMember) with
    | (Year year, _) when year <> payment.Date.Year -> ()
    | (_, Member memberId) when memberId <> payment.Member.Id -> ()
    | _ ->
        setState (fun (state: State) _ ->
                    { state with
                        Payments = state.Payments
                                |> List.append [payment]
                                |> List.sortByDescending (fun fine -> fine.Date) })

let element props children =
        komponent<PaymentsModel, State>
             props
             { Payments = props.Payments }
             None
             (fun (props, state, setState) ->
                let year = props.Year
                let selectedMember = props.SelectedMember
                let fines = state.Payments
                fragment [] [
                    mtMain [] [
                        block []
                            [ fineNav props.User props.Path ]

                        block [] [
                            props.User.IsInRole [Role.Botsjef] &?
                                fragment [] [
                                    div [Class "clearfix hidden-lg hidden-md u-margin-bottom"] [
                                        AddPayment.element
                                            (fun handleOpen -> btn [OnClick handleOpen; Primary; Class "pull-right"] [ Icons.add ""; whitespace; str "Registrer innbetalinger" ])
                                            (handleAdded year selectedMember setState) (handleDeleted setState)
                                    ]]       
                            
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
                                  col [ NoSort; Align Center ] [ Icons.fine "Beløp" ]
                                  col [ NoSort; Align Center ] [ Icons.calendar "Dato" ]
                                  col [ NoSort; Align Center; ExcludeWhen(not <| props.User.IsInRole [ Role.Botsjef ]) ] []
                                ]
                                (fines |> List.map (fun payment ->
                                                    let playerLink = a [ Href <| createUrl props.Year (Member payment.Member.Id) ]
                                                    tableRow [] [
                                                        playerLink [ img [ Src <| Image.getMember props.ImageOptions
                                                                                      (fun o -> { o with Height = Some 50
                                                                                                         Width = Some 50 })
                                                                                      payment.Member.Image payment.Member.FacebookId ] ]
                                                        fragment [] [playerLink [ str payment.Member.Name ]
                                                                     Strings.hasValue payment.Comment &?
                                                                        Base.tooltip payment.Comment [Style [MarginLeft "0.5em"]] [Icons.infoCircle ""]]
                                                        currency [] payment.Amount
                                                        payment.Date |> (if props.Year = AllYears then Date.formatLong else Date.formatShort) |> str
                                                        Modal.modal
                                                            { OpenButton = fun handleOpen -> linkButton [OnClick handleOpen] [ Icons.delete ]
                                                              Content =
                                                                fun handleClose ->
                                                                    div [] [
                                                                      h4 [] [ str <| sprintf "Er du sikker på at du vil slette %i,- fra %s (%s) ?"
                                                                                         payment.Amount
                                                                                         payment.Member.FullName
                                                                                         (Date.formatLong payment.Date)]
                                                                      div [ Class "text-center" ] [
                                                                          br []
                                                                          Send.sendElement
                                                                            (fun o ->
                                                                            { o with
                                                                                SendElement = btn, [Lg;Danger], [str "Slett"]
                                                                                SentElement = btn, [Lg], [str "Slettet"]          
                                                                                Endpoint = Send.Delete <| sprintf "/api/payments/%O" payment.Id
                                                                                OnSubmit = Some (!> handleClose >> (fun _ -> handleDeleted setState payment.Id)) })
                                                                          btn [ Lg; OnClick !> handleClose ] [ str "Avbryt" ]
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
                                            AddPayment.element
                                                (fun handleOpen -> linkButton [OnClick handleOpen] [ Icons.add ""; whitespace; str "Registrer betalinger" ])
                                                (handleAdded year selectedMember setState) (handleDeleted setState)
                                        ]
                                        hr []
                                        h5 [] [str "Betalingsinformasjon"]
                                        Textarea.render { Value = props.PaymentInformation; Url = "/api/payments/information"  }

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
                ])

hydrate paymentsView Decode.Auto.fromString<PaymentsModel> element
