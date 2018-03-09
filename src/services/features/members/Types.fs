namespace MyTeam.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members

type MemberWithTeamsAndRoles = {
    Details: Member
    Teams: TeamId list 
    Roles: Role list
}

[<CLIMutable>]
type AddMemberForm = {
    FacebookId: string 
    EmailAddress: string 
    FirstName: string
    MiddleName: string
    LastName: string
}

type ListMembers = Database -> ClubId -> MemberWithTeamsAndRoles list
type GetFacebookIds = Database -> ClubId -> string list
type SetStatus = Database -> ClubId -> MemberId -> Status -> UserId
type ToggleRole = Database -> ClubId -> MemberId -> Role -> UserId
type ToggleTeam = Database -> ClubId -> MemberId -> TeamId -> unit
type Add = Database -> ClubId -> AddMemberForm -> Result<unit, Error>