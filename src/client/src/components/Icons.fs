module Shared.Components.Icons

open Fable.React
open Fable.React.Props
open Shared.Domain

type IconSize =
    | Normal
    | Large
    | ExtraLarge

let fa name = sprintf "fa fa-%s" name

let icon name title = i [ Class name; Title title ] []

let eventIcon eventType size =
    let className =
        sprintf
            "%s %s"
            (match eventType with
             | EventType.Kamp -> fa "trophy"
             | EventType.Trening -> "flaticon-couple40"
             | EventType.Diverse -> fa "beer"
             | _ -> fa "calendar")
            (match size with
             | Large -> "fa-2x"
             | ExtraLarge -> "fa-3x"
             | _ -> "")

    icon className (string eventType)



let playerStatusIcon (status: PlayerStatus) =
    let className =
        (match status with
         | PlayerStatus.Aktiv -> fa "user"
         | PlayerStatus.Veteran -> fa "trophy"
         | PlayerStatus.Trener -> "flaticon-football50"
         | _ -> fa "user-times")

    icon className (string status)

let gameType (gameType: GameType) =
    let className =
        gameType
        |> function
            | GameType.Treningskamp -> "fa fa-handshake-o"
            | GameType.Seriekamp -> fa "trophy"
            | GameType.Norgesmesterskapet -> "flaticon-football42"
            | GameType.Kretsmesterskapet -> "flaticon-football33"
            | GameType.``OBOS Cup`` -> "flaticon-trophy4"

    icon className (string gameType)

let add = icon <| fa "plus"
let addCircle = icon <| fa "plus-circle"
let assist = icon <| "flaticon-football119"
let arrowLeft = icon <| fa "arrow-left"
let arrowRight = icon <| fa "arrow-right"
let attendance = icon <| fa "check-square-o"
let award = icon "flaticon-sports24"
let ballInGoal = icon "flaticon-goal"
let barChart = icon <| fa "bar-chart"
let dollar = icon <| fa "usd"
let description = icon (fa "file-text-o") "Beskrivelse"
let clock = icon (fa "clock-o") "Klokkeslett"
let close = icon (fa "times") "Lukk"
let check = icon (fa "check") ""
let chevronDown = icon (fa "chevron-down") ""
let chevronUp = icon (fa "chevron-up") ""
let checkCircle = icon (fa "check-circle") ""
let calendar = icon <| fa "calendar"
let comment = icon (fa "comment") ""
let coach = icon <| "flaticon-football50"
let delete = icon (fa "trash") "Slett"
let edit = icon <| fa "edit"
let facebook = icon <| fa "facebook"
let fine = icon <| fa "money"
let info = icon <| fa "info"
let infoCircle = icon <| fa "info-circle"
let list = icon <| fa "bar-chart"
let game = icon <| fa "trophy"
let gamePlan = icon (fa "exchange") "Bytteplan"
let goal = icon <| fa "soccer-ball-o"
let injury = icon <| "flaticon-football93"
let mapMarker = icon <| fa "map-marker"
let money = icon <| fa "money"
let news = icon <| fa "newspaper-o"
let payment = icon <| fa "list"
let player = icon <| "flaticon-soccer18"
let previous = icon <| fa "history"
let refresh = icon <| fa "refresh"
let redCard = icon <| "icon icon-card-red"
let remove = icon (fa "times") "Fjern"
let signup = icon <| fa "calendar"
let squadList = icon <| fa "users"
let spinner = icon (fa "spinner fa-spin") ""
let team = icon <| "flaticon-football43"
let teamSelection = icon "flaticon-football133" "Laguttak"
let training = icon <| "flaticon-couple40"
let trophy = icon <| fa "trophy"
let time = icon <| fa "clock-o"
let upcoming = icon <| fa "calendar-o"
let user = icon <| fa "user"
let users = icon <| fa "users"
let warning = icon (fa "exclamation-triangle") ""
let whistle = icon <| "flaticon-football75"
let yellowCard = icon <| "icon icon-card-yellow"


let gameEvent eventType =
    match eventType with
    | Mål -> goal
    | ``Gult kort`` -> yellowCard
    | ``Rødt kort`` -> redCard
    <| string eventType
