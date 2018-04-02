namespace MyTeam.Views

open MyTeam
open Giraffe.GiraffeViewEngine

[<AutoOpen>]
module LayoutComponents =  
        

    let main attributes children = div ([_class "mt-main"] |> mergeAttributes attributes) children 
    let sidebar attributes children = div ([_class "mt-sidebar"] |> mergeAttributes attributes) children 
    let block attributes children = div ([_class "mt-container"] |> mergeAttributes attributes) children 
   