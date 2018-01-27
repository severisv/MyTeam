namespace MyTeam.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Database

type Member = {
    Id: MemberId
    FirstName: string
    MiddleName: string
    LastName: string
    UrlName: string
    Status: Status
    Roles: Role list
    TeamIds: TeamId list
} with
    member x.FullName = sprintf "%s %s" x.FirstName x.LastName 

type ListMembers = Database -> ClubId -> Member list
type GetFacebookIds = Database -> ClubId -> string list
type SetStatus = Database -> ClubId -> MemberId -> Status -> UserId
type ToggleRole = Database -> ClubId -> MemberId -> Role -> UserId
type ToggleTeam = Database -> ClubId -> MemberId -> TeamId -> unit
