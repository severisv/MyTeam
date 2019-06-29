namespace MyTeam.Members

open MyTeam
open Shared.Domain
open Shared.Domain.Members
open MyTeam.Common.Features.Members
open Client.Admin.AddPlayers

type ListMembersDetailed = Database -> ClubId -> MemberDetails list
type SetStatus = Database -> ClubId -> MemberId -> PlayerStatus -> UserId
type ToggleRole = Database -> ClubId -> MemberId -> Role -> UserId
type ToggleTeam = Database -> ClubId -> MemberId -> TeamId -> unit
type Add = Database -> Club -> AddMemberForm -> HttpResult<unit>
