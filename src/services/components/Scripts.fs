module MyTeam.Views.Scripts

open MyTeam
open Giraffe
open Giraffe.GiraffeViewEngine
open Microsoft.Extensions.Options

let documentReady fn = 
    script [] [
        rawText <| sprintf "document.addEventListener('DOMContentLoaded', function(){
            %s
        })" fn
]

