namespace MyTeam.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open System
open MyTeam.Common.Features.Members
open Shared.Features.Admin.AddPlayers


type ListMembersDetailed = Database -> ClubId -> MemberDetails list
type GetFacebookIds = Database -> ClubId -> string list
type SetStatus = Database -> ClubId -> MemberId -> PlayerStatus -> UserId
type ToggleRole = Database -> ClubId -> MemberId -> Role -> UserId
type ToggleTeam = Database -> ClubId -> MemberId -> TeamId -> unit
type Add = Database -> Club -> AddMemberForm -> HttpResult<unit>