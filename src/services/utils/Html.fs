namespace MyTeam.Views

open MyTeam
open Giraffe.GiraffeViewEngine

[<AutoOpen>]
module Html =
    let mergeAttributes (a : XmlAttribute list) (b : XmlAttribute list) =
        a @ b
        |> List.groupBy (function 
               | KeyValue(key, _) -> key
               | Boolean key -> key)
        |> List.map (fun (key, values) -> 
               let values =
                   values
                   |> List.map (function 
                          | KeyValue(_, value) -> value
                          | Boolean key -> key)
                   |> String.concat " "
               KeyValue(key, values))
    
    let withClass className (c : XmlNode) =
        match c with
        | ParentNode((elementName, attributes), children) -> 
            ParentNode
                ((elementName, 
                  mergeAttributes (attributes |> Seq.toList) 
                      [ _class className ] |> List.toArray), children)
        | VoidElement(elementName, attributes) -> 
            VoidElement
                (elementName, 
                  mergeAttributes (attributes |> Seq.toList) 
                      [ _class className ] |> List.toArray)
        | Text r -> 
            Text(r |> replace "class=\"" (sprintf "class=\"%s " className))
