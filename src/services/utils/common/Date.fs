module MyTeam.Date

open System


let format (datetime: DateTime) =
        datetime.ToString("ddd d MMMM")

let formatLong (datetime: DateTime) =
        datetime.ToString("dd.MM.yyyy")        

let formatShort (datetime: DateTime) =
        datetime.ToString("dd.MM")        


let formatTime (datetime: DateTime) =
    datetime.ToString(@"HH\:mm")

