namespace MyTeam.Views

open MyTeam.Models.Enums
open Giraffe.GiraffeViewEngine



[<AutoOpen>]
module IconComponents =

    type IconSize = 
        | Normal
        | Large
        | ExtraLarge

    let fa name = 
        sprintf "fa fa-%s" name

    let icon name title =
        i [_class name;_title title ] []

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

module Icons = 
    let attendance = icon <| fa "check-square-o"
    let coach = icon <| "flaticon-football50"
    let edit = icon <| fa "edit"
    let signup = icon <| fa "calendar"
    let previous = icon <| fa "history"
    let upcoming = icon <| fa "calendar-o"
    let squadList = icon <| fa "users"
    let fine = icon <| fa "money"
    let news = icon <| fa "newspaper-o"
    let payment = icon <| fa "list"
    let assist = icon <| "flaticon-football119"
    let game = icon <| fa "trophy"
    let goal = icon <| fa "soccer-ball-o"
    let player = icon <| "flaticon-soccer18"
    let team = icon <| "flaticon-football43"
    let training = icon <| "flaticon-couple40"
    let user = icon <| fa "user"
    let yellowCard = icon <| "icon icon-card-yellow"
    let redCard = icon <| "icon icon-card-red"