module Server.Features.Fines.Common

open MyTeam
open Shared.Domain
open Shared
open Shared.Features.Fines.Common
open System

let getYears (db: Database) clubId =
           let (ClubId clubId) = clubId
           query {
               for fine in db.Fines do
                   where (fine.Rate.ClubId = clubId)
                   select fine.Issued.Year
                   distinct
           }
           |> Seq.toList
           |> List.sortDescending

let getSelectedYear (year: string option) years =
     match year with
     | None -> Year(years |> List.tryHead |> Option.defaultValue DateTime.Now.Year)
     | Some y when y = "total" -> AllYears
     | Some y when y |> Number.isNumber -> Year <| Number.parse y
     | Some y -> failwithf "Valgt år kan ikke være %s" y