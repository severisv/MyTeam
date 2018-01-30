namespace MyTeam.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Models
open MyTeam.Models.Domain
open MyTeam.Validation
open Microsoft.EntityFrameworkCore
open ExpressionOptimizer

module Persistence =
    let setStatus : SetStatus =
        fun db clubId memberId status -> 
            let members = Queries.members db clubId
            let memb = members
                    |> Seq.filter(fun p -> p.Id = memberId)
                    |> Seq.head

            memb.Status <- enum<Enums.PlayerStatus>(int status)
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
                                |> Members.toRoleList
                                |> toggleRoleInList role
                                |> Members.fromRoleList
                       
            db.SaveChanges() |> ignore        
            UserId memb.UserName


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
            

                                                
    let (=>) result fn =
        result |> Result.bind(fn)    

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
                                         
            let members = Queries.members db clubId
            let existingMember = 
                        if form.FacebookId.HasValue then
                            members |> Seq.tryFind(fun m -> m.FacebookId = form.FacebookId)
                        else
                            members |> Seq.tryFind(fun m -> m.UserName = form.EmailAddress)

            match existingMember with
            | Some _ -> Error (ValidationErrors [validationError "" "Brukeren er lagt til fra før"])
            | None ->            

                let form = if form.FacebookId.HasValue then
                                { form with EmailAddress = getEmailFromFacebookId form.FacebookId }
                           else 
                              form                        

                match form with
                | { EmailAddress = ""; FacebookId = "" } -> Error (ValidationErrors [validationError "" "E-postadresse eller FacebookId må sendes inn"])
                | __ ->
                        form ==>
                        [
                           <@ form.EmailAddress @> >- [isValidEmail]
                           <@ form.FirstName @> >- [isRequired]
                           <@ form.LastName @> >- [isRequired]
                        ] 
                        => fun form -> 
                               let (ClubId clubId) = clubId
                               let memb = Player()

                               memb.ClubId <- clubId
                               memb.FirstName <- form.FirstName
                               memb.MiddleName <- form.MiddleName
                               memb.LastName <- form.LastName
                               memb.FacebookId <- form.FacebookId
                               memb.UserName <- form.EmailAddress
                               db.Players.Add(memb) |> ignore
                               db.SaveChanges() |> ignore
                               Ok ()
                        

