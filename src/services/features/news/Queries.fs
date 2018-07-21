module MyTeam.News.Queries

open MyTeam.Domain
open MyTeam.News
open MyTeam

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


let getArticle : GetArticle =
    fun db clubId name ->
        let (ClubId clubId) = clubId
        query {
                    for article in db.Articles do
                    where (article.Name = name && article.ClubId = clubId)
                    select (article.Name, article.Headline, article.ImageUrl, article.Published, (article.Author.FirstName, article.Author.MiddleName, article.Author.LastName), article.Author.UrlName, article.Content, article.GameId)
                 }
                 |> Seq.map (fun (name,headline,image,published, authorName, authorUrlName, content, gameId) ->
                            {
                                Details = {         
                                            Name = name
                                            Headline = headline
                                            Image = image
                                            Published = published                                                                
                                }
                                Author = {
                                            UrlName = authorUrlName
                                            Name = authorName |> Members.fullName
                                }
                                Content = content
                                GameId = gameId |> toOption
                            }
                           
                 
                 )
                 |> Seq.tryHead


