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
open Shared.Components.Links
open Shared.Domain.Members
open Client.Util
open Shared.Domain
open System
open Shared
open Feliz

[<CLIMutable>]
type AddPayment =
    { Id: Guid option
      MemberId: Guid
      Date: DateTime
      Amount: int
      Comment: string }

type Payment =
    { Id: Guid
      Member: Member
      Comment: string
      Amount: int
      Date: DateTime }

type AddPaymentForm =
    { MemberId: Guid option
      Date: DateTime option
      Amount: string
      Comment: string
      IsTouched: bool }


[<ReactComponent>]
let addPayment onAdd onDelete handleClose =

    let (form, setForm) =
        React.useState (
            { MemberId = None
              Date = Some DateTime.Now
              Amount = ""
              Comment = ""
              IsTouched = false }
        )

    let (players, setPlayers) = React.useState ([])
    let (addedPayments, setAddedPayments) = React.useState<AddPayment list> ([])
    let (error, setError) = React.useState (None)


    Browser.Dom.console.log form

    React.useEffect (
        (fun () ->
            Http.get
                "/api/members"
                Decode.Auto.fromString<MemberWithTeamsAndRoles list>
                { OnSuccess =
                    fun result ->
                        let result =
                            result
                            |> List.filter (fun p -> p.Details.Status = PlayerStatus.Aktiv)

                        setPlayers (result)

                        setForm (
                            { form with
                                MemberId =
                                    result
                                    |> List.map (fun r -> r.Details.Id)
                                    |> List.tryHead }
                        )
                  OnError = fun _ -> setError (Some "Noe gikk galt ved lasting av spillere. Prøv å laste siden på nytt") }),
        [||]
    )


    let setFormValue update = setForm (update form)

    let validation =
        Map [
            "Date", isSome "Dato" form.Date
            "MemberId", isSome "Hvem" form.MemberId
            "Amount",
            (isNumber "Beløp" form.Amount)
            |> Result.bind (fun _ -> isRequired "Beløp" form.Amount)
        ]


    let playerName id =
        players
        |> List.tryFind (fun p -> p.Details.Id = id)
        |> Option.map (fun p -> p.Details.Name)
        |> Option.defaultValue ""

    let colNoBorder attr =
        col
            ([ Attr(Style [ BorderBottom "0" ])
               NoSort ]
             |> List.append attr)
            []


    fragment [] [
        h4 [] [ str "Registrer innbetalinger" ]
        table
            []
            [ colNoBorder [
                  Attr(Class "green")
                  Attr(Style [ PaddingLeft 0 ])
              ]
              colNoBorder []
              colNoBorder [ Align Center ]
              colNoBorder [ Align Center ]
              colNoBorder [ Align Right; NoPadding ] ]
            (addedPayments
             |> List.map (fun payment ->
                 tableRow [] [
                     Icons.check
                     str <| playerName payment.MemberId
                     Currency.currency [] payment.Amount
                     str (payment.Date |> Date.formatLong)
                     (Send.sendElement (fun o ->
                         { o with
                             SendElement = linkButton, [], [ Icons.delete ]
                             SentElement = linkButton, [], []
                             Endpoint =
                                 Send.Delete
                                 <| sprintf "/api/payments/%O" payment.Id
                             OnSubmit =
                                 Some (fun _ ->
                                     setAddedPayments (
                                         addedPayments
                                         |> List.filter (fun f -> f.Id <> payment.Id)
                                     )

                                     onDelete payment.Id.Value) }))
                 ]))
        Shared.Components.Forms.form [ Horizontal 3 ] [
            error => Alerts.danger
            formRow
                [ Horizontal 3 ]
                [ str "Hvem" ]
                [ selectInput
                      [ OnChange (fun e ->
                            let id = e.Value
                            setFormValue (fun form -> { form with MemberId = Some <| Guid.Parse id })) ]
                      (players
                       |> List.map (fun p ->
                           { Name = p.Details.Name
                             Value = p.Details.Id })) ]

            formRow
                [ Horizontal 3 ]
                [ str "Dato" ]
                [ dateInput [
                      Value form.Date
                      OnDateChange(fun date -> setFormValue (fun form -> { form with Date = date }))
                  ] ]
            formRow
                [ Horizontal 3 ]
                [ str "Beløp" ]
                [ textInput [
                      Validation [ validation.["Amount"] ]
                      OnChange (fun e ->
                          let value = e.Value

                          setFormValue (fun form ->
                              { form with
                                  Amount = value
                                  IsTouched = true }))
                      Placeholder "137"
                      Value form.Amount
                      IsTouched form.IsTouched
                  ] ]

            // formRow
            //     [ Horizontal 3 ]
            //     [ str "Kommentar" ]
            //     [ textInput [
            //           OnChange (fun e ->
            //               let value = e.Value
            //               setFormValue (fun form -> { form with Comment = value }))
            //           Placeholder "Eventuell kommentar"
            //           Value form.Comment
            //       ] ]

            formRow
                [ Horizontal 3 ]
                []
                [ Send.sendElement (fun o ->
                      { o with
                          IsDisabled =
                              validation
                              |> Map.toList
                              |> List.exists (function
                                  | (_, Error e) -> true
                                  | _ -> false)
                          SendElement = btn, [ ButtonSize.Normal; Primary ], [ str "Legg til" ]
                          SentElement = btn, [ ButtonSize.Normal; Success ], []
                          Endpoint =
                              Send.Post(
                                  sprintf "/api/payments",
                                  Some (fun () ->
                                      Encode.Auto.toString (
                                          0,
                                          { Id = None
                                            MemberId = form.MemberId.Value
                                            Date = form.Date.Value
                                            Amount =
                                              Number.tryParse form.Amount
                                              |> Option.defaultValue 0
                                            Comment = form.Comment }
                                      ))
                              )
                          OnSubmit =
                              Some (fun res ->
                                  Decode.Auto.fromString<AddPayment> res
                                  |> function
                                      | Ok payment ->

                                          setAddedPayments ([ payment ] @ addedPayments)

                                          setForm
                                              { form with
                                                  Amount = ""
                                                  Comment = ""
                                                  IsTouched = false }

                                          onAdd (
                                              { Id = payment.Id.Value
                                                Member =
                                                  players
                                                  |> List.map (fun p -> p.Details)
                                                  |> List.find (fun p -> p.Id = payment.MemberId)
                                                Amount = payment.Amount
                                                Comment = payment.Comment
                                                Date = payment.Date }
                                          )
                                      | Error e -> setError (Some e)) })

                  btn [ OnClick !>handleClose ] [
                      str "Lukk"
                  ]

                  ]
        ]
    ]


let element openLink onAdd onDelete =
    Modal.modal
        { OpenButton = openLink
          Content = addPayment onAdd onDelete }
