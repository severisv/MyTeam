module MyTeam.Date

open System


let format (datetime: DateTime) =
        let dayOfWeek = ["søn"; "man"; "tir"; "ons"; "tor"; "fre"; "lør"].[int datetime.DayOfWeek]
        let month = [ "januar"; "februar"; "mars"; "april"; "mai"; "juni"; "juli"; "august"; "september"; "oktober"; "november"; "desember" ].[datetime.Month - 1]

        sprintf "%s. %i %s" dayOfWeek datetime.Day month

let formatLong (datetime: DateTime) =
        sprintf "%02i.%02i.%04i" datetime.Day datetime.Month datetime.Year

let formatShort (datetime: DateTime) =
        sprintf "%02i.%02i" datetime.Day datetime.Month


let formatTime (datetime: DateTime) =
    sprintf "%02i:%02i" datetime.Hour datetime.Minute
