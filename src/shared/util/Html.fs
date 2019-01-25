module MyTeam.Shared.Util.Html

open Fable.Helpers.React.Props

let mergeClasses (a: IHTMLProp list) (b: IHTMLProp list) =
    let getClass (attr: IHTMLProp list) =
        attr
        |> List.filter (fun a -> a :? HTMLAttr) 
        |> List.map(fun a ->
                let htmlAttr = a :?> HTMLAttr
                match htmlAttr with
                | ClassName c -> 
                    c
                | Class c -> 
                    c
                | _ -> ""
        )


    a @ b @ [Class <| String.concat " " (getClass a @ getClass b)]
        
   
