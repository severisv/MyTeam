namespace MyTeam.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members

type Member = {
    Id: MemberId
    FirstName: string
    MiddleName: string
    LastName: string
    UrlName: string
    Status: Status
    Roles: string list
    TeamIds: TeamId list
} with
    member x.FullName = sprintf "%s %s" x.FirstName x.LastName 

type ListMembers = ConnectionString -> ClubId -> Member list
type GetFacebookIds = ConnectionString -> ClubId -> string list
type SetStatus = ConnectionString -> ClubId -> MemberId -> Status -> unit
