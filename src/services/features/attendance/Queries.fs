namespace MyTeam.Attendance

open System.Linq
open MyTeam
open MyTeam.Domain
open MyTeam.Models.Enums
open System

module Queries =

    type PlayerAttendance = {
        FacebookId: string
        FirstName: string
        LastName: string
        UrlName: string
        Image: string
        Games: int
        Trainings: int
        NoShows: int
    }   
    with member p.Name = sprintf "%s %s" p.FirstName p.LastName    

    type SelectedYear = 
    | AllYears
    | Year of int

    type Model = {
        SelectedYear: SelectedYear
        Attendance: PlayerAttendance list
        Years: int list
    }

    type GetAttendance = Database -> ClubId -> string option -> Model


    let get : GetAttendance = 
        fun db clubId year ->
            let (ClubId clubId) = clubId

            let years = 
                   query {
                            for e in db.EventAttendances do
                            where (                
                                    e.Event.ClubId = clubId &&
                                    (e.Event.Type = (int EventType.Trening) || e.Event.Type = (int EventType.Kamp))
                                  )
                            select e.Event.DateTime.Year
                            distinct
                   }
                   |> Seq.toList
                   |> List.sortDescending                                        
        
            let selectedYear = 
                match year with
                | Some y when isNumber y -> Year (int y)
                | Some y when y = "total" -> AllYears
                | Some y -> failwithf "Year må være en string, fikk %s" y
                | None -> Year (match years |> List.tryHead with
                                    | Some (y: int) -> y
                                    | None -> DateTime.Now.Year)

            let attendance = 
                let inOneHour = DateTime.Now.AddHours(1.0)
                let attendanceQuery =
                    match selectedYear with
                    | Year y ->                     
                        query {
                            for a in db.EventAttendances do
                            where (a.Event.DateTime.Year = y)
                        }
                    | AllYears ->
                        query {
                            for a in db.EventAttendances do
                            select a                          
                        }    

                let attendances = 
                    query {
                        for a in attendanceQuery do
                        where (
                                a.Event.ClubId = clubId &&
                                a.Event.DateTime < inOneHour &&
                                (a.Event.Type = (int EventType.Trening) || a.Event.Type = (int EventType.Kamp)) &&
                                a.Event.Voluntary <> true
                            )
                        select (a, a.Event.Type)
                    } 
                    |> Seq.toList

                let playerIds = (attendances |> Seq.map (fun (a,_) -> a.MemberId) |> Seq.distinct)
                let players = 
                    query {
                        for p in db.Members do
                        where (playerIds.Contains(p.Id) && p.Status <> (int PlayerStatus.Trener))
                        select (p.Id, p.FacebookId, p.FirstName, p.LastName, p.UrlName, p.ImageFull)
                    } 
                    |> Seq.toList              
                    |> List.map 
                        (fun (id, facebookId, firstName, lastName, urlName, imageFull) ->
                            let attendances = attendances |> List.filter (fun (a, _) -> a.MemberId = id)
                            {
                                FacebookId = facebookId
                                FirstName = firstName
                                LastName = lastName
                                UrlName = urlName
                                Image = imageFull
                                Games = attendances 
                                        |> List.filter (fun (a, eventType) -> eventType = (int EventType.Kamp) && a.IsSelected) 
                                        |> List.length
                                Trainings = attendances 
                                            |> List.filter (fun (a, eventType) -> eventType = (int EventType.Trening) && a.DidAttend) 
                                            |> List.length
                                NoShows = attendances 
                                            |> List.filter (fun (a, eventType) -> eventType = (int EventType.Trening) && a.IsAttending = Nullable true && not a.DidAttend) 
                                            |> List.length
                            }
                        )    

                query {
                    for p in players do
                    where (p.Games + p.Trainings > 0)
                    sortByDescending p.Trainings
                    thenByDescending p.Games
                    thenByDescending p.NoShows
                } |> Seq.toList    
        
            {
                SelectedYear = selectedYear
                Attendance = attendance
                Years = years
            }

