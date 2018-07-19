namespace MyTeam.News

open MyTeam
open MyTeam.Domain
open System


type Article = {
  UrlName: string
  Image: string
  Headline: string
  Published: DateTime
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

type GetArticles = Database -> ClubId -> PaginationOptions -> PagedList<Article>

type GetClubDescription = Database -> ClubId -> string option


