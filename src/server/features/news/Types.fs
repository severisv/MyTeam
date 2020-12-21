namespace MyTeam.News

open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Members
open System
open Server.Common.News

type PaginationOptions = {
    Skip: int
    Take: int
}


type PagedList<'a> =  {
  Items: List<'a>
  HasNext: bool
  PaginationOptions: PaginationOptions
} with
  member p.HasPrevious = p.PaginationOptions.Skip > 0


type Game = {
    Id: Guid
    Name: string
}

[<CLIMutable>]
type ArticleModel = {
    Id: Guid
    IsMatchReport: bool
    HideAuthor: bool
    GameId: Guid option
    Headline: string
    Content: string
    ImageUrl: string
}


type ListArticles = Database -> ClubId -> PaginationOptions -> PagedList<Article>

type GetClubDescription = Database -> ClubId -> string option
type UpdateArticle = Database -> ClubId -> ArticleName -> ArticleModel -> HttpResult<unit> 
type CreateArticle = Database -> ClubId -> User -> ArticleModel -> HttpResult<ArticleName> 

type DeleteArticle = Database -> ClubId -> ArticleName -> HttpResult<unit>