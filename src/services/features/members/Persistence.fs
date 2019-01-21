namespace MyTeam.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Common.Features.Members
open MyTeam.Models.Domain
open MyTeam.Validation
open Microsoft.EntityFrameworkCore
open System.Linq
open MyTeam.Common.Features.Members

module Persistence =
    let setStatus : SetStatus =
        fun db clubId memberId status -> 
            let members = Queries.members db clubId
            let memb = members
                    |> Seq.filter(fun p -> p.Id = memberId)
                    |> Seq.head

            memb.Status <- (int status)
            db.SaveChanges() |> ignore
            UserId memb.UserName


    let toggleRole : ToggleRole =
        fun db clubId memberId role ->

            let toggleRoleInList role roleList =
                if roleList |> List.contains role then
                    roleList |> List.filter (fun r -> r <> role)
                else 
                    roleList |> List.append [role]      

            let members = Queries.members db clubId
            
            let memb = members
                    |> Seq.filter(fun p -> p.Id = memberId)
                    |> Seq.head

            memb.RolesString <- memb.RolesString 
                                |> toRoleList
                                |> toggleRoleInList role
                                |> fromRoleList
                       
            db.SaveChanges() |> ignore        
            UserId (memb.UserName =?? "")


    let toggleTeam : ToggleTeam =
        fun db clubId memberId teamId -> 
            
            let (ClubId clubId) = clubId

            let memb = db.Members.Include(fun m -> m.MemberTeams)
                    |> Seq.filter(fun m -> m.Id = memberId)
                    |> Seq.head

            if memb.ClubId <> clubId then failwith "Prøver å redigere spiller fra annen klubb - ingen tilgang"

            let memberTeam = memb.MemberTeams
                                |> Seq.filter (fun mt -> mt.TeamId = teamId)
                                |> Seq.tryHead

            match memberTeam with 
                | Some m ->
                    db.Remove(m) |> ignore
                | None ->
                    let memberTeam = MemberTeam()
                    memberTeam.TeamId <- teamId
                    memb.MemberTeams.Add(memberTeam)
            
            db.SaveChanges() |> ignore         
            

                                                
    let add : Add =
        fun db clubId form ->  

            let getEmailFromFacebookId facebookId =
                db.UserLogins 
                |> Seq.tryFind(fun l -> l.ProviderKey = facebookId)
                |> Option.bind(fun l -> 
                    db.Users 
                    |> Seq.tryFind(fun u -> u.Id = l.UserId)
                ) 
                |> Option.fold (fun _ v -> v.Email) ""
                                         
            let memberDoesNotExist db ((_, form): string * AddMemberForm) = 
                let members = Queries.members db clubId
                if form.FacebookId.HasValue then
                    members |> Seq.tryFind(fun m -> m.FacebookId = form.FacebookId)
                else
                    members |> Seq.tryFind(fun m -> m.UserName = form.EmailAddress)
                |> Option.map (fun _ -> Error "Brukeren er lagt til fra før")
                |?? Ok ()                            
           
            let facebookIdOrEmailIsPresent (_, form) =
                match form with
                  | { EmailAddress = ""; FacebookId = "" } -> Error "E-postadresse eller FacebookId må sendes inn"
                  | __ -> Ok()


            let urlName (form: AddMemberForm) =
                let (ClubId clubId) = clubId
                let rec addNumberIfTaken str =
                    if db.Members.Any(fun m -> m.ClubId = clubId && m.UrlName = str) then 
                        addNumberIfTaken <| sprintf "%s-1" str 
                    else str

                sprintf "%s%s%s-%s" form.FirstName (form.MiddleName |> isNullOrEmpty =? ("", "-")) form.MiddleName form.LastName
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


            let form = if form.FacebookId.HasValue then
                            { form with EmailAddress = getEmailFromFacebookId form.FacebookId }
                       else 
                          form                        
          
            form 
            |> Validation.map 
                [
                   <@ form @> >- [facebookIdOrEmailIsPresent]
                   <@ form @> >- [memberDoesNotExist db]
                   <@ form.EmailAddress @> >- [isValidEmail]
                   <@ form.FirstName @> >- [isRequired]
                   <@ form.LastName @> >- [isRequired]
                ] 
            |> Validation.bindToHttpResult 
                (fun form -> 
                       let (ClubId clubId) = clubId
                       let memb = Player()

                       memb.ClubId <- clubId
                       memb.FirstName <- form.FirstName
                       memb.MiddleName <- form.MiddleName
                       memb.LastName <- form.LastName
                       memb.FacebookId <- form.FacebookId
                       memb.UserName <- form.EmailAddress
                       memb.UrlName <- urlName form
                       db.Players.Add(memb) |> ignore
                       db.SaveChanges() |> ignore
                       OkResult ()
                )
                    
