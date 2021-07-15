module MyTeam.Sponsors

open Fable.React
open Giraffe.ViewEngine
open MyTeam
open Shared.Components
open Shared.Domain
open Shared.Domain.Members
open MyTeam.Views
open Fable.React.Props
open MyTeam.Views.BaseComponents
open System.Linq

type Sponsors = string

type GetSponsors = Database -> ClubId -> Sponsors

let getSponsors : GetSponsors =
    fun db clubId ->
        let (ClubId clubId) = clubId

        query {
            for club in db.Clubs do
                where (club.Id = clubId)
                select (club.Sponsors)
        }
        |> Seq.tryHead
        |> Option.defaultValue ""

let show (club: Club) (user: User option) (ctx: HttpContext) =
    let db = ctx.Database

    getSponsors db club.Id
    |> fun sponsors ->
        [ block [] [
              (user
               => (fun user ->
                   if user.IsInRole [ Role.Admin ] then
                       a [ _class "pull-right edit-link"
                           _href "/støttespillere/endre" ] [
                           !!(Icons.edit "Rediger side")
                       ]
                   else
                       empty))
              div [ _class "richtext" ] [
                  rawText sponsors
              ]
          ] ]
        |> layout club user (fun o -> { o with Title = "Støttespillere" }) ctx
        |> OkResult


let edit (club: Club) (user: User option) (ctx: HttpContext) =
    let db = ctx.Database

    getSponsors db club.Id
    |> fun sponsors ->
        [ block [] [
              form [ _action "/støttespillere/endre"
                     _method "POST" ] [
                  textarea [ _name "Value"
                             _class "form-control tinymce"
                             _placeholder "Innhold" ] [
                      rawText sponsors
                  ]
                  br []
                  !!(btn [ Primary
                           ButtonSize.Normal
                           Type "submit" ] [
                      Helpers.str "Lagre"
                     ])
              ]
          ] ]
        |> layout
            club
            user
            (fun o ->
                { o with
                      Title = "Støttespillere"
                      Scripts = MyTeam.News.Pages.Components.tinyMceScripts })
            ctx
        |> OkResult


let editPost (club: Club) user (ctx: HttpContext) =

    let db = ctx.Database
    let (ClubId clubId) = club.Id

    let form =
        ctx.BindForm<Shared.Components.Input.StringPayload>()

    db.Clubs.Where(fun c -> c.Id = clubId)
    |> Seq.tryHead
    |> function
        | Some c ->
            c.Sponsors <- form.Value
            db.SaveChanges() |> ignore
            Redirect "/støttespillere"
        | None -> NotFound
