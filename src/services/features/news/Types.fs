namespace MyTeam.News

open MyTeam
open MyTeam.Domain
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

type ListArticles = Database -> ClubId -> PaginationOptions -> PagedList<Article>

type GetArticle = Database -> ClubId -> ArticleName -> ArticleDetailed option

type GetClubDescription = Database -> ClubId -> string option


