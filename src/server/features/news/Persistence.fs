module MyTeam.News.Persistence

open Shared.Domain
open Shared.Domain.Members
open MyTeam.News
open MyTeam
open System
open System.Linq

let saveArticle: UpdateArticle =
    fun db clubId articleName model ->
        let (ClubId clubId) = clubId

        db.Articles.Where(fun a -> a.ClubId = clubId && a.Name = articleName)
        |> Seq.tryHead
        |> function
            | Some a ->
                a.Headline <- model.Headline
                a.Content <- model.Content
                a.ImageUrl <- model.ImageUrl
                a.HideAuthor <- model.HideAuthor

                a.GameId <-
                    model.IsMatchReport
                    =? (model.GameId |> toNullable, Nullable())

                db.SaveChanges() |> ignore
                OkResult()

            | None -> NotFound


let createArticle: CreateArticle =
    fun db clubId user model ->
        let (ClubId clubId) = clubId

        let articleName =

            let rgx = Text.RegularExpressions.Regex("[^a-zA-Z0-9 -]")

            let rec appendNumberIfTaken name =
                let isTaken n =
                    query {
                        for a in db.Articles do
                            exists (a.Name = n)
                    }

                if isTaken name then
                    appendNumberIfTaken (name + "-1")
                else
                    name

            model.Headline.Replace(" ", "-")
            |> toLower
            |> fun value -> rgx.Replace(value, "")
            |> appendNumberIfTaken

        let article = MyTeam.Models.Domain.Article()
        article.Headline <- model.Headline
        article.Content <- model.Content
        article.ImageUrl <- model.ImageUrl

        article.GameId <-
            model.IsMatchReport
            =? (model.GameId |> toNullable, Nullable())

        article.AuthorId <- Nullable(user.Id)
        article.Published <- DateTime.Now
        article.ClubId <- clubId
        article.Name <- articleName
        article.HideAuthor <- model.HideAuthor

        db.Articles.Add(article) |> ignore
        db.SaveChanges() |> ignore
        OkResult articleName


let deleteArticle: DeleteArticle =
    fun db clubId articleName ->
        let (ClubId clubId) = clubId

        db.Articles.Where(fun a -> a.ClubId = clubId && a.Name = articleName)
        |> Seq.tryHead
        |> function
            | Some a ->
                db.Remove(a) |> ignore
                db.SaveChanges() |> ignore
                OkResult()

            | None -> NotFound
