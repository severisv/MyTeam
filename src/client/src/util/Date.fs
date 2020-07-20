module Shared.Date

open System

let format (datetime: DateTime) =
#if FABLE_COMPILER
    let datetime = datetime.ToLocalTime()
#endif
    let dayOfWeek =
        [ "søn"
          "man"
          "tir"
          "ons"
          "tor"
          "fre"
          "lør" ].[int datetime.DayOfWeek]

    let month =
        [ "januar"
          "februar"
          "mars"
          "april"
          "mai"
          "juni"
          "juli"
          "august"
          "september"
          "oktober"
          "november"
          "desember" ].[datetime.Month - 1]

    sprintf "%s. %i %s" dayOfWeek datetime.Day month

let formatLong (datetime: DateTime) =
#if FABLE_COMPILER
    let datetime = datetime.ToLocalTime()
#endif
    sprintf "%02i.%02i.%04i" datetime.Day datetime.Month datetime.Year

let formatShort (datetime: DateTime) =
#if FABLE_COMPILER
    let datetime = datetime.ToLocalTime()
#endif
    sprintf "%02i.%02i" datetime.Day datetime.Month


let formatTime (datetime: DateTime) =
#if FABLE_COMPILER
    let datetime = datetime.ToLocalTime()
#endif
    sprintf "%02i:%02i" datetime.Hour datetime.Minute


let tryParse (dateString: string) =
    let couldParse, parsedDate = System.DateTime.TryParse dateString
    if couldParse then Some parsedDate else None

let tryParseTime (timeString: string) =
    try
        timeString.Split(':')
        |> Array.toList
        |> List.map int
        |> function
        | [ hours; minutes ] when hours
                                  >= 0
                                  && hours <= 23
                                  && minutes >= 0
                                  && minutes <= 60 -> TimeSpan(int hours, int minutes, 0) |> Some
        | _ -> None
    with _ -> None
