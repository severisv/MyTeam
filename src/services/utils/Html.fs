namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open System

[<AutoOpen>]
module Html =
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