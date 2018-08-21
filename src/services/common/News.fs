namespace MyTeam.Common.News

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Shared.Components
open MyTeam.Views
open System

type ArticleName = string

type Article = {
  Name: ArticleName
  Image: string
  Headline: string
  Published: DateTime
}

type Author = {
  UrlName: string
  Name: string
}

type ArticleDetailed = {
    Details: Article
    Author: Author
    Content: string
    GameId: System.Guid option
}


type GetArticle = Database -> ClubId -> ArticleName -> ArticleDetailed option

module Queries =

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



module Components =
  let editLink (article: Article) (user: Users.User option) =
    user |> Option.bind (fun user ->
                            if user.IsInRole [Role.Admin;Role.Skribent;Role.Trener] then
                                Some <| a [ _class "pull-right edit-link"; _href <| sprintf "/nyheter/endre/%s" article.Name ] [ 
                                           !!(Icons.edit "Rediger artikkel")
                                ]
                            else None
                        )
    |> Option.defaultValue empty
    
 

  let image ctx url = 
      Images.getArticle ctx url (fun o -> { o with Format = Some Jpg; Quality = 85; Width = Some 1280  })

  let showArticle ctx user article subHeaderLink =
      div [ _class "news-item"] [ 
          div [ _class "news-imageWrapper" ] [
              img [ _src <| image ctx article.Details.Image ] ]                    
          editLink article.Details user
          h2 [] [ 
              encodedText article.Details.Headline ]
          
          p [ _class "news-author" ] [ 
              a [ _class "underline"; _href <| sprintf "/spillere/vis/%s" article.Author.UrlName ] [ 
                  encodedText article.Author.Name 
              ]          
              span [ _class "datetime" ] [ encodedText <| sprintf " %s" (Date.format article.Details.Published) ]            
              subHeaderLink |> Option.defaultValue empty                      
          ]
          hr [ _class "sm" ]
          div [ _class "news-content" ][ rawText article.Content ]
      ]     