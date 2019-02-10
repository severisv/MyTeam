namespace MyTeam.News

open MyTeam
open MyTeam.Domain
open System
open MyTeam.Users
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
    Id: System.Guid
    Name: string
}

[<CLIMutable>]
type ArticleModel = {
    IsMatchReport: bool
    HideAuthor: bool
    GameId: System.Guid option
    Headline: string
    Content: string
    ImageUrl: string
}


type ListArticles = Database -> ClubId -> PaginationOptions -> PagedList<Article>

type GetClubDescription = Database -> ClubId -> string option

type ListRecentGames = Database -> ClubId -> DateTime -> Game list

type UpdateArticle = Database -> ClubId -> ArticleName -> ArticleModel -> HttpResult<unit> 
type CreateArticle = Database -> ClubId -> User -> ArticleModel -> HttpResult<ArticleName> 

type DeleteArticle = Database -> ClubId -> ArticleName -> HttpResult<unit>