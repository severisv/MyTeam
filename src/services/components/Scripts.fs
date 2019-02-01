module MyTeam.Views.Scripts

open Giraffe.GiraffeViewEngine

let documentReady fn = 
    script [] [
        rawText <| sprintf "document.addEventListener('DOMContentLoaded', function(){
            %s
        })" fn
]

