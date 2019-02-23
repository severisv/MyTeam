module Server.Features.Fines.Summary

open System
open System.Linq
open Giraffe.GiraffeViewEngine
open Shared.Domain
open Shared.Domain.Members
open MyTeam
open MyTeam.Views
open MyTeam.Views.BaseComponents
open Shared.Components
open Currency
open Common


type Summary = {
    Member : Member
    Total : int
    Remaining : int
}

type Amount = {
    MemberId: Guid
    Year: int
    Amount: int
}

let view (club : Club) (user : Users.User) year (ctx : HttpContext) =

     let (ClubId clubId) = club.Id
     let db = ctx.Database
     let isSelected = fun _ -> false
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
         year
         |> function
         | None -> years |> Seq.tryHead
         | Some year -> Some year
         |> Option.defaultValue DateTime.Now.Year

     
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
               Amount = values |> List.sumBy (fun (_, amount, _) -> amount)})
     
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
               Amount = values |> List.sumBy (fun (_, amount, _) -> amount)})
     
     let fines =    
         let memberIds = allFines |> Seq.map (fun a -> a.MemberId)

         let members =
             query {
                 for memb in db.Members do
                     where (memberIds.Contains memb.Id) }
             |> Common.Features.Members.selectMembers
             |> Seq.toList
        
                     
         allFines
         |> List.filter(fun a -> a.Year = year)
         |> List.map (fun p ->
             { Member = members |> List.find (fun m -> m.Id = p.MemberId)
               Total = p.Amount
               Remaining = (allFines
                            |> List.filter(fun a -> a.Year <= year && a.MemberId = p.MemberId)
                            |> List.sumBy (fun a -> a.Amount)) -
                           (payments
                            |> List.filter(fun a -> a.Year <= year && a.MemberId = p.MemberId)
                            |> List.sumBy (fun a -> a.Amount)) })
         |> List.sortByDescending (fun memb -> memb.Total)

     
     let paymentInformation =
         query {
             for info in db.PaymentInformation do
                 where (info.ClubId = clubId)
                 select info.Info }
         |> Seq.tryHead
                  
     let remainingForUser =
         Some <| (allFines
                 |> List.filter(fun a -> a.MemberId = user.Id)
                 |> List.sumBy (fun a -> a.Amount)) -
                 (payments
                 |> List.filter(fun a -> a.MemberId = user.Id)
                 |> List.sumBy (fun a -> a.Amount))
         |> Option.bind (fun a -> if a > 0 then Some a else None)
         
     let createUrl =
         sprintf "/intern/boter/%i"

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
                                                                Url = createUrl year })
                       Footer = None
                       IsSelected = isSelected })

                remainingForUser => fun amount ->
                    div [_class "fine-userDue" ]
                        [ div [_class "pull-left" ]
                            [ img [_src <| Images.getMember ctx (fun o -> { o with Width = Some 100; Height = Some 100 }) user.Image user.FacebookId ] ]
                          div []
                            [ h3 [ _class "text-left " ]
                                [ span [_class "no-wrap" ]
                                    [ encodedText "Du skylder: "
                                      !!(currency [Colored (fun _ -> Negative)] amount) ]
                                ]
                              h5 [_class "text-left" ]
                                 [ encodedText "Betalingsinformasjon:" ]
                              encodedText (paymentInformation |> Option.defaultValue "Det er ikke lagt til noen betalingsinformasjon.") ]]
                table []
                        [ col [CellType Image; NoSort] []
                          col [NoSort] []
                          col [NoSort; Align Center] [ !!(Icons.fine "Utestående"); whitespace; encodedText "Utestående" ]
                          col [NoSort; Align Right] [ !!(Icons.fine "Utestående"); whitespace; encodedText "Total" ] ]
                        (fines |> List.map (fun { Member = player; Total = total; Remaining = remaining } ->
                                            let playerLink = a [ _href <| sprintf "/intern/boter/vis/%i/%O" year player.Id ]
                                            tableRow [] [
                                                playerLink [ img [ _src <| Images.getMember ctx
                                                                              (fun o -> { o with Height = Some 50
                                                                                                 Width = Some 50 })
                                                                              player.Image player.FacebookId ] ]
                                                playerLink [ encodedText player.Name ]
                                                !!(currency [Colored (fun a -> if a <= 0 then Positive else Negative)] remaining)
                                                !!(currency [] total)
                                            ]))
                div [_class "text-right"
                     _style "padding-right: 2em;"
                    ] [ encodedText "Total"
                        b [_style "margin-left: 3em;"] [encodedText <| sprintf "%i,-" (fines |> List.sumBy (fun f -> f.Total))] ]
            ]
        ]
        (years.Length > 0 =?
            (sidebar [] [
                block [] [
                    navList ({ Header = "Sesonger"
                               Items = years |> List.map (fun year -> { Text = [ encodedText <| string year ]
                                                                        Url = createUrl year })
                               Footer = None
                               IsSelected = isSelected })
                ]
            ]
            , emptyText))
     ]
     |> layout club (Some user) (fun o -> { o with Title = "Botoversikt" }) ctx
     |> OkResult
