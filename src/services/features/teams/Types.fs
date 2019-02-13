namespace MyTeam.Teams

open System
open MyTeam
open Shared
open Shared.Domain


type ListTeams = Database -> ClubId -> Team list
