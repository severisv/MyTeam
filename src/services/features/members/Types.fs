namespace MyTeam.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open System

type MemberDetails = {
    Details: Member
    Phone: string
    Email: string
    BirthDate: DateTime option
} 
with member m.BirthYear = m.BirthDate |> Option.map (fun b -> b.Year)


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
type ListMembersDetailed = Database -> ClubId -> MemberDetails list
type GetFacebookIds = Database -> ClubId -> string list
type SetStatus = Database -> ClubId -> MemberId -> Status -> UserId
type ToggleRole = Database -> ClubId -> MemberId -> Role -> UserId
type ToggleTeam = Database -> ClubId -> MemberId -> TeamId -> unit
type Add = Database -> ClubId -> AddMemberForm -> HttpResult<unit>