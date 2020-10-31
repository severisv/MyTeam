module MyTeam.Players.Pages.Edit

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Domain
open MyTeam.Views
open Shared.Domain.Members
open System.Linq
open Common
open Server
open Giraffe
open Microsoft.Extensions.Options


let view (club: Club) (user: User option) urlName (ctx: HttpContext) =
    let cloudinarySettings =
        ctx.GetService<IOptions<CloudinarySettings>>()

    let db = ctx.Database
    let (ClubId clubId) = club.Id

    let urlName = urlName |> Strings.toLower

    let (!!!) = Strings.defaultValue
    db.Members.Where(fun m -> m.ClubId = clubId && m.UrlName = urlName)
    |> Seq.toList
    |> List.map (fun p ->
        {| Id = p.Id
           FacebookId = !!!p.FacebookId
           FirstName = !!!p.FirstName
           MiddleName = !!!p.MiddleName
           LastName = !!!p.LastName
           FullName = sprintf "%s %s %s" p.FirstName p.MiddleName p.LastName
           UrlName = !!!p.UrlName
           Image = !!!p.ImageFull
           Status = PlayerStatus.fromInt p.Status
           Positions =
               !!!p.PositionsString
               |> Strings.split ','
           StartDate = p.StartDate |> Option.fromNullable
           BirthDate = p.BirthDate |> Option.fromNullable
           Email = !!!p.UserName
           Phone = !!!p.Phone |})
    |> List.tryHead
    |> HttpResult.fromOption (fun player ->

        let players =
            let statusInt = (player.Status |> PlayerStatus.toInt)

            (db.Members.Where(fun m -> m.ClubId = clubId && m.Status = statusInt))
            |> Features.Members.selectMembers
            |> Seq.toList


        [ mtMain [] [
            block [] [
                div [ _class "row" ] [
                    div [ _class "col-sm-6 col-md-6 col-lg-6 picture-frame" ] [
                        div [ _class "cloudinary-preview editPlayer-image" ] [
                            img [ _src
                                  <| Images.getMember ctx (fun o -> { o with Width = Some 1024 }) player.Image
                                         player.FacebookId ]
                        ]
                        div [ _class "form-file-upload-wrapper" ] [
                            input [ _name "file"
                                    _type "file"
                                    _class "btn btn-default cloudinary-fileupload pull-left"
                                    attr "data-cloudinary-field" "ImageFull" ]
        
                        ]
                        div [ _class "clearfix" ] []
                    ]
                    div [ _class "col-sm-6 col-md-6 col-lg-6 player-info" ] [
                        Client.view2 Client.Features.Players.Form.containerId Client.Features.Players.Form.element
                                      {   Id = player.Id
                                          FirstName = player.FirstName
                                          MiddleName = player.MiddleName
                                          LastName = player.LastName
                                          Positions = player.Positions
                                          BirthDate = player.BirthDate
                                          StartDate = player.StartDate
                                          Phone = player.Phone
                                          Image = player.Image                                                         
                                          UrlName = player.UrlName } 
                    ]
                ]
            ]
          ]
          sidebar player.Status players player.UrlName ]
        |> layout club user (fun o ->
               { o with
                     Title = player.FullName
                     Scripts = Images.uploadScripts cloudinarySettings.Value }) ctx)
