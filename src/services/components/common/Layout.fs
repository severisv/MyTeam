namespace MyTeam.Views

open Giraffe.GiraffeViewEngine

[<AutoOpen>]
module LayoutComponents =  

    let sidebar attributes children = div ([_class "mt-sidebar"] @ attributes) children 
    let block attributes children = div ([_class "mt-container"] @ attributes) children 