module MyTeam.Shared.Components.Icons

open MyTeam.Shared.Domain

open Fable.Helpers.React
open Fable.Helpers.React.Props


type IconSize = 
    | Normal
    | Large
    | ExtraLarge

let fa name = 
    sprintf "fa fa-%s" name

let icon name title =
    i [Class name;Title title ] []

let eventIcon eventType size =
        let className =
            sprintf "%s %s"
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
            | GameType.Treningskamp -> "icon-handshake"
            | GameType.Seriekamp -> fa "trophy"
            | GameType.Norgesmesterskapet -> "flaticon-football42"
            | GameType.Kretsmesterskapet -> "flaticon-football33"
            | GameType.``OBOS Cup`` -> "flaticon-trophy4"      
            | _ -> fa "trophy"
            
        icon className (string gameType)     

let add = icon <| fa "plus"
let assist = icon <| "flaticon-football119"
let attendance = icon <| fa "check-square-o"
let ballInGoal = icon "flaticon-goal"
let calendar = icon <| fa "calendar"
let coach = icon <| "flaticon-football50"
let delete = icon (fa "trash") "Slett"
let edit = icon <| fa "edit"
let fine = icon <| fa "money"
let game = icon <| fa "trophy"
let gamePlan = icon (fa "exchange") "Bytteplan"
let goal = icon <| fa "soccer-ball-o"
let injury = icon <| "flaticon-football93"
let mapMarker = icon <| fa "map-marker"
let news = icon <| fa "newspaper-o"
let payment = icon <| fa "list"
let player = icon <| "flaticon-soccer18"
let previous = icon <| fa "history"
let refresh = icon <| fa "refresh"
let redCard = icon <| "icon icon-card-red"
let signup = icon <| fa "calendar"
let squadList = icon <| fa "users"
let team = icon <| "flaticon-football43"
let training = icon <| "flaticon-couple40"
let trophy = icon <| fa "trophy"
let time = icon <| fa "clock-o"
let upcoming = icon <| fa "calendar-o"
let user = icon <| fa "user"
let whistle = icon <| "flaticon-football75"
let yellowCard = icon <| "icon icon-card-yellow"