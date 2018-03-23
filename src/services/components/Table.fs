namespace MyTeam.Views

open MyTeam
open Giraffe.GiraffeViewEngine

[<AutoOpen>]
module TableModule =  

    type TableAlignment = 
       | Left
       | Center

    type CellType = Normal | Image


    type TableProperty =
       | Align of TableAlignment
       | NoSort
       | ClassName of string
       | CellType of CellType

    type TableColumn = {
        Value: XmlNode list
        Props: TableProperty list
    }
                   
    let colClassName col =
        col.Props
        |> List.map (function
                        | NoSort -> "nosort"
                        | Align a -> sprintf "table-align--%s" (Enums.toString a |> toLower)
                        | ClassName s -> s
                        | CellType a -> sprintf "table-td-%s" (Enums.toString a |> toLower)

                    )
        |> List.distinct
        |> String.concat " "

    let col props value =
         { Value = value; Props = props } 

    let table attributes columns rows = 
        table ([_class "table tablesorter"] |> mergeAttributes attributes) [
                        thead [] [
                            tr [] (columns 
                                  |> List.map(fun col ->    
                                            th [_class <| colClassName col] col.Value    
                                  ))           
                        ]   
                        tbody [] 
                                (rows 
                                    |> List.map(fun row ->
                                        tr [] 
                                            (row
                                            |> List.mapi (fun i value ->
                                                    td 
                                                        [_class <| colClassName columns.[i] ] 
                                                        [value]
                                            ))                    
                                        )
                                )
                    ]
