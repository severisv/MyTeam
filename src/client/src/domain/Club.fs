namespace Shared.Domain

open System

type TeamId = Guid

type LeagueType =
    | Syver
    | Ellever

type Team =
    { Id: TeamId
      ShortName: string
      Name: string
      LeagueType: LeagueType }

type ClubIdentifier = ClubIdentifier of string
type ClubId = ClubId of Guid

type Club =
    { Id: ClubId
      ClubId: string
      ShortName: string
      Name: string
      Teams: Team list
      Favicon: string
      Logo: string }
