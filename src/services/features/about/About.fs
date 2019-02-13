module MyTeam.About

open Giraffe.GiraffeViewEngine
open Shared.Components
open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Members
open MyTeam.Views
open Fable.Helpers.React.Props
open MyTeam.Views.BaseComponents


type About = string

type GetAbout = Database -> ClubId -> About

let getAbout : GetAbout =
    fun db clubId -> 
        let (ClubId clubId) = clubId
        query { 
            for club in db.Clubs do
                where (club.Id = clubId)
                select (club.About)
        }
        |> Seq.tryHead
        |> Option.defaultValue ""

let show (club : Club) (user : Users.User option) (ctx : HttpContext) =
    let db = ctx.Database
    getAbout db club.Id
    |> fun about -> 
        [ block [] [ (user => (fun user -> 
                      if user.IsInRole [ Role.Admin ] then 
                          a [ _class "pull-right edit-link"
                              _href "/om/endre" ] 
                              [ !!(Icons.edit "Rediger side") ]
                      else empty))
                     div [_class "richtext"] [
                        rawText about
                        ]
                      ] ]
        |> layout club user (fun o -> { o with Title = "Om klubben" }) ctx
        |> OkResult


let edit (club : Club) (user : Users.User option) (ctx : HttpContext) =
    let db = ctx.Database
    getAbout db club.Id
    |> fun about -> 
        [ 
          block [] [
                  form [ _action "/om/endre"
                         _method "POST" ] 
                      [       
                        textarea [   _name "Value"
                                     _class "form-control tinymce"
                                     _placeholder "Innhold" ] [ rawText about ] 
                        br []
                        !!(btn [Primary; Type "submit"] [Fable.Helpers.React.str "Lagre"])
                      ] ]
        ]
        |> layout club user (fun o -> { o with Title = "Om klubben"; Scripts = MyTeam.News.Pages.Components.tinyMceScripts }) ctx
        |> OkResult


let editPost (club: Club) user (ctx: HttpContext) =

    let db = ctx.Database 
    let (ClubId clubId) = club.Id
    let form = ctx.BindForm<Shared.Components.Input.StringPayload>()
    db.Clubs
    |> Seq.tryFind (fun c -> c.Id = clubId)
    |> function 
    | Some c -> 
        c.About <- form.Value
        db.SaveChanges() |> ignore
        Redirect "/om"
    | None -> NotFound
    

    