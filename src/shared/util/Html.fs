module Shared.Util.Html

open Shared
open Fable.Helpers.React.Props

let mergeClasses (a : IHTMLProp list) (b : IHTMLProp list) =
    let getClass (attr : IHTMLProp list) =
        attr
        |> List.filter (fun a -> a :? HTMLAttr)
        |> List.map (fun a ->
            if a :? HTMLAttr then

               a :?> HTMLAttr
               |> function
               | ClassName c -> c
               | Class c -> c
               | _ -> ""
            else "")
        |> String.concat " "
        |> Strings.trim

    let excludeClasses : IHTMLProp list -> IHTMLProp list =
        List.filter (fun e ->
            if e :? HTMLAttr then
                e :?> HTMLAttr
                |> function
                | ClassName c -> false
                | Class c -> false
                | _ -> true
            else true
        )

    excludeClasses a @ excludeClasses b @ [ Class(getClass a + " " + getClass b) ]
