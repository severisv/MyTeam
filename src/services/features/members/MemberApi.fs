module MyTeam.Members.Api

open Shared.Domain
open Shared.Domain.Members
open MyTeam.Common.Features
open MyTeam
open Shared
open Shared.Features.Admin.AddPlayers
open System.Linq
open MyTeam.Validation
open MyTeam.Models.Domain
open Services.Utils
open Server.Common

let list clubId db =
    Members.list db clubId
    |> OkResult

[<CLIMutable>]
type SetStatus = { Status : Status }
let setStatus clubId id next (ctx : HttpContext) =
        let model = ctx.BindJson<SetStatus>()
        Persistence.setStatus ctx.Database clubId id model.Status
        |> Tenant.clearUserCache ctx clubId
        next ctx

[<CLIMutable>]
type ToggleRole = { Role : Role }
let toggleRole clubId id next (ctx : HttpContext) =
        let model = ctx.BindJson<ToggleRole>()
        Persistence.toggleRole ctx.Database clubId id model.Role
        |> Tenant.clearUserCache ctx clubId
        next ctx

[<CLIMutable>]
type ToggleTeam = { TeamId : TeamId }
let toggleTeam clubId id (ctx : HttpContext) model =
        Persistence.toggleTeam ctx.Database clubId id model.TeamId
        |> OkResult

let add clubId (ctx : HttpContext) model =

        let add : Add =
            fun db club form ->

                let clubId = club.Id

                let memberDoesNotExist db ((_, form) : string * AddMemberForm) =
                    let members = Queries.members db clubId
                    if not <| Strings.hasValue form.``E-postadresse`` then
                        Ok()
                    else
                       (if form.FacebookId.HasValue then
                            members |> Seq.tryFind (fun m -> m.FacebookId = form.FacebookId)
                        else None)
                        |> function
                         | Some user -> Some user
                         | None -> members |> Seq.tryFind (fun m -> m.UserName = form.``E-postadresse``)
                        |> Option.map (fun _ -> Error "Brukeren er lagt til fra før")
                        |> Option.defaultValue (Ok())

                let urlName (form : AddMemberForm) =
                    let (ClubId clubId) = clubId

                    let rec addNumberIfTaken str =
                        if db.Members.Any(fun m -> m.ClubId = clubId && m.UrlName = str) then
                            addNumberIfTaken <| sprintf "%s-1" str
                        else str
                    sprintf "%s%s%s-%s" form.Fornavn (form.Mellomnavn
                                                        |> isNullOrEmpty
                                                        =? ("", "-")) form.Mellomnavn form.Etternavn
                    |> replace "Ø" "O"
                    |> replace "ø" "o"
                    |> replace "æ" "ae"
                    |> replace "Æ" "Ae"
                    |> replace "Å" "Aa"
                    |> replace "å" "aa"
                    |> replace "É" "e"
                    |> replace "é" "e"
                    |> regexReplace "[^a-zA-Z0-9 -]" ""
                    |> addNumberIfTaken


                validate [ <@ form @> >- [ memberDoesNotExist db ]
                           <@ form.``E-postadresse`` @> >- [ isRequired; isValidEmail ]
                           <@ form.Fornavn @> >- [ isRequired ]
                           <@ form.Etternavn @> >- [ isRequired ] ]
                |> function
                 | Ok() ->
                     let (ClubId clubId) = clubId
                     db.Members.Add (
                            Member ( ClubId = clubId,
                                     FirstName = form.Fornavn,
                                     MiddleName = form.Mellomnavn,
                                     LastName = form.Etternavn,
                                     FacebookId = form.FacebookId,
                                     UserName = form.``E-postadresse``,
                                     UrlName = urlName form,
                                     MemberTeams = System.Linq.Enumerable.ToList (
                                                    club.Teams
                                                    |> Seq.map (fun team -> MemberTeam(TeamId = team.Id))
                                                )
                                        ) ) |> ignore

                     db.MemberRequests
                     |> Seq.tryFind (fun mr -> mr.ClubId = clubId && mr.Email = form.``E-postadresse``)
                     |> Option.map (fun mr ->
                         Email.send ctx.RequestServices
                             mr.Email
                             "Du er lagt til som spiller"
                             (sprintf "Du er nå lagt til som spiller i %s!\nDu kan melde deg på kamper og treninger på http://wamkam.no/intern" club.Name)
                             |> Async.RunSynchronously
                         db.MemberRequests.Remove mr
                     )
                     |> ignore

                     db.SaveChanges() |> ignore
                     Tenant.clearUserCache ctx club.Id (UserId form.``E-postadresse``)                   
                     OkResult()
                 | Error e -> ValidationErrors e

        add ctx.Database clubId model
