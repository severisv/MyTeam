module Server.Features.Fines.Summary

open System
open System.Linq
open Giraffe.GiraffeViewEngine
open Shared.Domain
open Shared.Domain.Members
open Shared
open MyTeam
open MyTeam.Views
open MyTeam.Views.BaseComponents
open Shared.Components
open Currency
open Common

type SelectedYear =
    | AllYears
    | Year of int

type Summary = {
    Member : Member
    Total : int
    Remaining : int
 }

type Amount = {
    MemberId : Guid
    Year : int
    Amount : int
 }

let view (club : Club) (user : Users.User) (year : string option) (ctx : HttpContext) =

     let (ClubId clubId) = club.Id
     let db = ctx.Database
     let years =
           query {
               for fine in db.Fines do
                   where (fine.Rate.ClubId = clubId)
                   select fine.Issued.Year
                   distinct
           }
           |> Seq.toList
           |> List.sortDescending

     let year =
         match year with
         | None -> Year(years |> List.tryHead |> Option.defaultValue DateTime.Now.Year)
         | Some y when y = "total" -> AllYears
         | Some y when y |> isNumber -> Year <| Number.parse y
         | Some y -> failwithf "Valgt år kan ikke være %s" y


     let allFines =
         query {
             for fine in db.Fines do
                 where (fine.Rate.ClubId = clubId)
                 select (fine.MemberId, fine.Amount, fine.Issued.Year)
         }
         |> Seq.toList
         |> List.groupBy (fun (memberId, _, year) -> (memberId, year))
         |> List.map (fun ((memberId, year), values) ->
             { MemberId = memberId
               Year = year
               Amount = values |> List.sumBy (fun (_, amount, _) -> amount) })

     let payments =
         query {
             for payment in db.Payments do
                 where (payment.ClubId = clubId)
                 select (payment.MemberId, payment.Amount, payment.TimeStamp.Year)
         }
         |> Seq.toList
         |> List.groupBy (fun (memberId, _, year) -> (memberId, year))
         |> List.map (fun ((memberId, year), values) ->
             { MemberId = memberId
               Year = year
               Amount = values |> List.sumBy (fun (_, amount, _) -> amount) })

     let sum (year : SelectedYear) memberId =
            let filterBy =
                function
                | Year year ->
                    List.filter (fun a -> a.Year <= year && a.MemberId = memberId)
                | AllYears ->
                    List.filter (fun a -> a.MemberId = memberId)

            filterBy year >> List.sumBy (fun a -> a.Amount)

     let fines =
         let memberIds = allFines |> Seq.map (fun a -> a.MemberId)

         let members =
             query {
                 for memb in db.Members do
                     where (memberIds.Contains memb.Id) }
             |> Common.Features.Members.selectMembers
             |> Seq.toList


         let toAmount year p =
                 { Member = members |> List.find (fun m -> m.Id = p.MemberId)
                   Total = p.Amount
                   Remaining = (allFines |> sum year p.MemberId) -
                               (payments |> sum year p.MemberId) }


         allFines
         |> List.filter (fun a ->
                    match year with
                    | Year year -> a.Year = year
                    | AllYears -> true)
         |> List.map (toAmount year)
         |> List.groupBy (fun a -> a.Member)
         |> List.map (fun (memb, amounts) -> { Member = memb
                                               Remaining = amounts.[0].Remaining
                                               Total = amounts |> List.sumBy (fun a -> a.Total) })
         |> List.sortByDescending (fun memb -> memb.Total)

     let paymentInformation =
         query {
             for info in db.PaymentInformation do
                 where (info.ClubId = clubId)
                 select info.Info }
         |> Seq.tryHead

     let remainingForUser =
         Some <| (allFines |> sum AllYears user.Id) -
                 (payments |> sum AllYears user.Id)
         |> Option.bind (fun a -> if a > 0 then Some a else None)

     let createUrl =
         function
         | AllYears ->
             "/intern/boter/total"
         | Year y ->
             sprintf "/intern/boter/%i" y


     let isSelected url =
        createUrl year = url

     [
        mtMain [] [
            block []
                [ fineNav user ctx.Request.Path.Value ]

            block [] [
                navListMobile
                    ({ Header = "Sesonger"
                       Items = years |> List.map (fun year -> { Text = string year
                                                                Url = createUrl <| Year year })
                       Footer = Some <| { Text = "Total"; Url = createUrl AllYears }
                       IsSelected = isSelected })

                remainingForUser => fun amount ->
                    div [ _class "fine-userDue" ]
                        [ div [ _class "pull-left" ]
                            [ img [ _src <| Images.getMember ctx (fun o -> { o with Width = Some 100; Height = Some 100 }) user.Image user.FacebookId ] ]
                          div []
                            [ h3 [ _class "text-left " ]
                                [ span [ _class "no-wrap" ]
                                    [ encodedText "Du skylder: "
                                      !!(currency [ Colored(fun _ -> Negative) ] amount) ]
                                ]
                              h5 [ _class "text-left" ]
                                 [ encodedText "Betalingsinformasjon:" ]
                              encodedText (paymentInformation |> Option.defaultValue "Det er ikke lagt til noen betalingsinformasjon.") ] ]
                table []
                        [ col [ CellType Image; NoSort ] []
                          col [ NoSort ] []
                          col [ NoSort; Align Center ] [ !!(Icons.fine "Utestående"); whitespace; encodedText "Utestående" ]
                          col [ NoSort; Align Right ] [ !!(Icons.fine "Utestående"); whitespace; encodedText "Total" ] ]
                        (fines |> List.map (fun { Member = player; Total = total; Remaining = remaining } ->
                                            let playerLink =
                                                let year = year |> function
                                                            | AllYears -> "total"
                                                            | Year year -> string year
                                                a [ _href <| sprintf "/intern/boter/vis/%s/%O" year player.Id ]
                                            tableRow [] [
                                                playerLink [ img [ _src <| Images.getMember ctx
                                                                              (fun o -> { o with Height = Some 50
                                                                                                 Width = Some 50 })
                                                                              player.Image player.FacebookId ] ]
                                                playerLink [ encodedText player.Name ]
                                                !!(currency [ Colored(fun a -> if a <= 0 then Positive else Negative) ] remaining)
                                                !!(currency [] total)
                                            ]))
                div [ _class "text-right"
                      _style "padding-right: 2em;"
                    ] [ encodedText "Total"
                        b [ _style "margin-left: 3em;" ] [ encodedText <| sprintf "%i,-" (fines |> List.sumBy (fun f -> f.Total)) ] ]
            ]
        ]
        (years.Length > 0 =?
            (sidebar [] [
                block [] [
                    navList ({ Header = "Sesonger"
                               Items = years |> List.map (fun year -> { Text = [ encodedText <| string year ]
                                                                        Url = createUrl <| Year year })
                               Footer = Some <| { Text = [ encodedText "Total" ]; Url = createUrl AllYears }
                               IsSelected = isSelected })
                ]
            ]
            , emptyText))
     ]
     |> layout club (Some user) (fun o -> { o with Title = "Botoversikt" }) ctx
     |> OkResult
