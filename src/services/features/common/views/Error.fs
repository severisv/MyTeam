module MyTeam.Views.Error

open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Views


let notFound club user next ctx =
    ([    
        main [] [
            block [] [
                h2 [_style "display: flex; align-items: center; justify-content:center; font-size: 2em"] [ 
                        i [_style "font-size: 3em; margin-right: 0.25em"; _class "flaticon-football93" ] [] 
                        encodedText " Auda... vi finner ikke siden du leter etter :(" ]
            ]    
        ]            
     ] 
     |> layout club user 
                (fun o -> { 
                            o with 
                                Title = "404"
                        }
                ) ctx
     |> htmlView) next ctx