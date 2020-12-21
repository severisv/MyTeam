module MyTeam.News.Queries

open System
open Shared.Domain
open MyTeam.News
open MyTeam
open Shared
open Server.Common.News

let getClubDescription : GetClubDescription =
    fun db clubId ->
        let (ClubId clubId) = clubId

        query {
                for club in db.Clubs do
                where (club.Id = clubId)
                select (club.Description)
           } |> Seq.tryHead

           

let listArticles : ListArticles = 
    fun db clubId paginationOptions ->
        let (ClubId clubId) = clubId

        let articles = 
            query {
                    for article in db.Articles do
                    where (article.ClubId = clubId)
                    sortByDescending article.Published
                    skip paginationOptions.Skip
                    take (paginationOptions.Take + 1)
                    select (article.Name, article.Headline, article.ImageUrl, article.Published)
                 }
                 |> Seq.map (fun (name,headline,image,published) ->
                            {
                                Name = name
                                Headline = headline
                                Image = image
                                Published = published
                            }
                 
                 )
                 |> Seq.toList

        let hasNext = articles.Length > paginationOptions.Take

        {
            Items = articles |> List.truncate paginationOptions.Take
            PaginationOptions = paginationOptions
            HasNext = hasNext
        }


let listRecentGames (db: Database) clubId articleId (date: DateTime) =
        let (ClubId clubId) = clubId
        let fourteenDaysBefore = date.AddDays(-14.0)
        query {
                for g in db.Games do
                where (g.ClubId = clubId &&
                       g.DateTime <= date &&
                       g.DateTime >= fourteenDaysBefore &&
                       (g.Report = null || g.Report.Id = articleId))
                sortBy (g.DateTime)
                select (g.Id, g.Team.ShortName, g.Opponent)
            }
            |> Seq.map(fun (id, name, opponent) 
                        -> 
                            {
                                Id = id
                                Name = sprintf "%s vs %s" name opponent
                            }
            )
            |> Seq.toList
