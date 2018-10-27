module MyTeam.Shared.Util.Html

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import.React

let mergeClasses (a: IHTMLProp list) (b: IHTMLProp list) =
    let getClass (attr: IHTMLProp list) =
        attr |> 
        List.map(fun a ->
                let htmlAttr = a :?> HTMLAttr
                match htmlAttr with
                | ClassName c -> 
                    c
                | Class c -> 
                    c
                | _ -> ""
        )


    a @ b @ [Class <| String.concat " " (getClass a @ getClass b)]
        
   
