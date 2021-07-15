module MyTeam.News.Pages.Show

open Giraffe.ViewEngine
open MyTeam
open Shared
open MyTeam.Views
open Shared.Domain
open Shared.Domain.Members
open MyTeam.News
open MyTeam.News.Pages
open Server.Common.News
open Server
open Giraffe
open System.Linq
open System

let tryParseGuid (s: string) =
    match Guid.TryParse s with
    | true, i -> Some i
    | _ -> None

let redirectFromOldUrl club user =
    fun next (ctx: HttpContext) ->
        ctx.TryGetQueryStringValue "articleId"
        |> Option.bind tryParseGuid
        |> Option.bind
            (fun articleId ->
                ctx
                    .Database
                    .Articles
                    .Where(fun a -> articleId = a.Id)
                    .Select(fun a -> a.Name)
                |> Seq.tryHead)
        |> function
            | Some articleName -> redirectTo true (sprintf "/nyheter/vis/%s" articleName) next ctx
            | _ ->
                (setStatusCode 404
                 >=> ErrorHandling.logNotFound
                 >=> Views.Error.notFound club user)
                    next
                    ctx


let view (club: Club) (user: User option) name (ctx: HttpContext) =


    let db = ctx.Database

    Queries.getArticle db club.Id name
    |> function
        | None -> NotFound
        | Some article ->
            [ mtMain [] [
                block [] [
                    Common.News.Components.showArticle
                        ctx
                        user
                        article
                        (article.GameId
                         |> Option.map
                             (fun gameId ->
                                 a [ _href <| sprintf "/kamper/%O" gameId
                                     _class "pull-right u-font-normal" ] [
                                     i [ _class "fa fa-info-circle" ] []
                                     encodedText " Kampdetaljer"
                                 ]))
                ]
              ]
              sidebar [] [
                  Components.articleNav db club
                  Components.twitterFeed
              ] ]
            |> layout
                club
                user
                (fun o ->
                    { o with
                          MetaTitle = article.Details.Headline
                          Title = "Nyheter"
                          Scripts = [ Components.twitterScript ] })
                ctx
            |> OkResult
