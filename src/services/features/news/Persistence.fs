module MyTeam.News.Persistence

open MyTeam.Domain
open MyTeam.News
open MyTeam
open System

let saveArticle : UpdateArticle =
    fun db clubId articleName model ->
        let (ClubId clubId) = clubId

        db.Articles
        |> Seq.tryFind (fun a -> a.ClubId = clubId && a.Name = articleName)
        |> function
        | Some a -> 
              a.Headline <- model.Headline
              a.Content <- model.Content
              a.ImageUrl <- model.ImageUrl
              a.GameId <- model.IsMatchReport =? (Nullable(), model.GameId |> toNullable)           
              db.SaveChanges() |> ignore
              OkResult()

        | None -> NotFound                                                  


let deleteArticle : DeleteArticle =
    fun db clubId articleName ->
        let (ClubId clubId) = clubId

        db.Articles
        |> Seq.tryFind (fun a -> a.ClubId = clubId && a.Name = articleName)
        |> function
        | Some a -> 
              db.Remove(a) |> ignore    
              db.SaveChanges() |> ignore
              OkResult()

        | None -> NotFound                                                  


           

