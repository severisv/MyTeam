module MyTeam.News.Queries

open MyTeam.Domain
open MyTeam.News

let getClubDescription : GetClubDescription =
    fun db clubId ->
        let (ClubId clubId) = clubId

        query {
                for club in db.Clubs do
                where (club.Id = clubId)
                select (club.Description)
           } |> Seq.tryHead

           

let getArticles : GetArticles = 
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
                                UrlName = name
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

