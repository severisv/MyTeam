module MyTeam.Sponsors

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Common.News
open MyTeam.Components
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Shared.Components
open MyTeam.Users
open MyTeam.Views
open System
open Fable.Helpers.React.Props

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

let show (club : Club) (user : Users.User option) (ctx : HttpContext) =
    let db = ctx.Database
    getSponsors db club.Id
    |> fun sponsors -> 
        [ block [] [ (user => (fun user -> 
                      if user.IsInRole [ Role.Admin ] then 
                          a [ _class "pull-right edit-link"
                              _href "/støttespillere/endre" ] 
                              [ !!(Icons.edit "Rediger side") ]
                      else empty))
                     div [_class "richtext"] [
                        rawText sponsors ] ]
        ]
        |> layout club user (fun o -> { o with Title = "Støttespillere" }) ctx
        |> OkResult


let edit (club : Club) (user : Users.User option) (ctx : HttpContext) =
    let db = ctx.Database
    getSponsors db club.Id
    |> fun sponsors -> 
        [ 
          block [] [
                  form [ _action "/støttespillere/endre"
                         _method "POST" ] 
                      [       
                        textarea [   _name "Value"
                                     _class "form-control tinymce"
                                     _placeholder "Innhold" ] [ rawText sponsors ] 
                        br []
                        !!(btn Primary ButtonSize.Normal [Type "submit"] [Fable.Helpers.React.str "Lagre"])
                      ] ]
        ]
        |> layout club user (fun o -> { o with Title = "Støttespillere"; Scripts = MyTeam.News.Pages.Components.tinyMceScripts }) ctx
        |> OkResult


let editPost (club: Club) user (ctx: HttpContext) =

    let db = ctx.Database 
    let (ClubId clubId) = club.Id
    let form = ctx.BindForm<MyTeam.Shared.Components.Input.StringPayload>()
    db.Clubs
    |> Seq.tryFind (fun c -> c.Id = clubId)
    |> function 
    | Some c -> 
        c.Sponsors <- form.Value
        db.SaveChanges() |> ignore
        Redirect "/støttespillere"
    | None -> NotFound
    

    