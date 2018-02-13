namespace MyTeam.Views

open Giraffe.GiraffeViewEngine

[<AutoOpen>]
module IconFunctions =

    let fa name = 
        sprintf "fa fa-%s" name

    let icon name title =
        i [_class name;_title title ] []


module Icons = 
    let attendance = icon <| fa "check-square-o"
    let coach = icon <| "flaticon-football50"
    let signup = icon <| fa "calendar"
    let previous = icon <| fa "history"
    let upcoming = icon <| fa "calendar-o"
    let squadList = icon <| fa "users"
    let fine = icon <| fa "money"
    let news = icon <| fa "newspaper"
    let payment = icon <| fa "list"
    let assist = icon <| "flaticon-football119"
    let goal = icon <| fa "soccer-ball-o"
    let player = icon <| "flaticon-soccer18"
    let training = icon <| "flaticon-couple40"
    let user = icon <| fa "user"
    let yellowCard = icon <| "icon icon-card-yellow"
    let redCard = icon <| "icon icon-card-red"