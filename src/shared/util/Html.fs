module MyTeam.Shared.Util.Html

open Fable.Helpers.React.Props
open MyTeam

let mergeClasses (a : IHTMLProp list) (b : IHTMLProp list) =
    let getClass (attr : IHTMLProp list) =
        attr
        |> List.filter (fun a -> a :? HTMLAttr)
        |> List.map (fun a -> 
               a :?> HTMLAttr
               |> function 
               | ClassName c -> c
               | Class c -> c
               | _ -> "")
        |> String.concat " "
        |> Strings.trim
    
    let excludeClasses : IHTMLProp list -> IHTMLProp list =
        List.filter (fun e -> 
            e :?> HTMLAttr
            |> function
            | ClassName c -> false
            | Class c -> false
            | _ -> true)
    
    excludeClasses a @ excludeClasses b @ [ Class(getClass a + " " + getClass b) ]
