module MyTeam.Views.Error

open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Shared.Components
open MyTeam.Views

let internal errorMessage icon msg =
    h2 [ _style "display: flex; align-items: center; justify-content:center; font-size: 2em" ] 
        [ span [ _style "font-size: 3em; margin-right: 0.25em" ] [ icon ]
          encodedText msg ]

let notFound club user next ctx =
    ([ block [] 
           [ errorMessage !!(Icons.injury "") " Auda... vi finner ikke siden du leter etter :(" ] ]
     |> layout club user (fun o -> { o with Title = "404" }) ctx
     |> htmlView) next ctx

let serverError club user next ctx =
    ([ block [] 
           [ errorMessage !!(Icons.injury "") 
                 " Auda... det oppstod en feil. Vi har meldt fra til de det gjelder." ] ]
     |> layout club user (fun o -> { o with Title = "500" }) ctx
     |> htmlView) next ctx

let unauthorized club user next ctx =
    ([ block [] 
           [ errorMessage !!(Icons.whistle "") " Offside! Denne siden har du ikke tilgang til." ] ]
     |> layout club user (fun o -> { o with Title = "Ingen tilgang" }) ctx
     |> htmlView) next ctx

let stackTrace club user (ex : System.Exception) next ctx =
    ([ block [] [ h3 [ _class "u-error"
                       _style "text-align:left" ] [ encodedText ex.Message ]
                  pre [] [ encodedText <| string ex ] ] ]
     |> layout club user (fun o -> { o with Title = "500" }) ctx
     |> htmlView) next ctx
