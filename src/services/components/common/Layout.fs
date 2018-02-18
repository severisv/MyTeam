namespace MyTeam.Views

open Giraffe.GiraffeViewEngine

[<AutoOpen>]
module LayoutComponents =  

    let mergeAttributes (a: XmlAttribute list) (b: XmlAttribute list) =
        a @ b |> List.groupBy (function
                                | KeyValue (key, _) -> key
                                | Boolean key -> key)
              |> List.map (fun (key, values) ->
                            let values = values |> List.map(function
                                                    | KeyValue (_, value) -> value
                                                    | Boolean key -> key)
                                                |> String.concat " "
                            KeyValue (key, values)                                            
                          )                        

    let main attributes children = div ([_class "mt-main"] |> mergeAttributes attributes) children 
    let sidebar attributes children = div ([_class "mt-sidebar"] |> mergeAttributes attributes) children 
    let block attributes children = div ([_class "mt-container"] |> mergeAttributes attributes) children 
    let row attributes children = div ([_class "row"] @ attributes) children 