module MyTeam.Members.Persistence

open Microsoft.EntityFrameworkCore
open MyTeam
open MyTeam.Common.Features.Members
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Models.Domain
open MyTeam.Validation
open Shared.Features.Admin.AddPlayers
open System.Linq


let setStatus : SetStatus =
    fun db clubId memberId status ->
        let members = Queries.members db clubId

        let memb =
            members
            |> Seq.filter (fun p -> p.Id = memberId)
            |> Seq.head
        memb.Status <- (int status)
        db.SaveChanges() |> ignore
        UserId memb.UserName

let toggleRole : ToggleRole =
    fun db clubId memberId role ->
        let toggleRoleInList role roleList =
            if roleList |> List.contains role then roleList |> List.filter (fun r -> r <> role)
            else roleList |> List.append [ role ]

        let members = Queries.members db clubId

        let memb =
            members
            |> Seq.filter (fun p -> p.Id = memberId)
            |> Seq.head
        memb.RolesString <- memb.RolesString
                            |> toRoleList
                            |> toggleRoleInList role
                            |> fromRoleList
        db.SaveChanges() |> ignore
        UserId(memb.UserName =?? "")

let toggleTeam : ToggleTeam =
    fun db clubId memberId teamId ->
        let (ClubId clubId) = clubId

        let memb =
            db.Members.Include(fun m -> m.MemberTeams)
            |> Seq.filter (fun m -> m.Id = memberId)
            |> Seq.head
        if memb.ClubId <> clubId then
            failwith "Prøver å redigere spiller fra annen klubb - ingen tilgang"
        let memberTeam =
            memb.MemberTeams
            |> Seq.filter (fun mt -> mt.TeamId = teamId)
            |> Seq.tryHead
        match memberTeam with
        | Some m -> db.Remove(m) |> ignore
        | None ->
            let memberTeam = MemberTeam()
            memberTeam.TeamId <- teamId
            memb.MemberTeams.Add(memberTeam)
        db.SaveChanges() |> ignore

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
             db.Players.Add (
                            Player (
                             ClubId = clubId,
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
                                )
                           ) |> ignore
             db.SaveChanges() |> ignore
             OkResult()
         | Error e -> ValidationErrors e
