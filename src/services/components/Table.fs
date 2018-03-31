namespace MyTeam.Views

open MyTeam
open Giraffe.GiraffeViewEngine

[<AutoOpen>]
module TableModule =  

    type TableAlignment = 
       | Left
       | Center

    type CellType = Normal | Image


    type CellProperty =
       | Align of TableAlignment
       | NoSort
       | Attr of XmlAttribute
       | CellType of CellType

    type TableColumn = {
        Value: XmlNode list
        Props: CellProperty list
    }
                   
    let colAttributes col =
        col.Props
        |> List.map (function
                        | Attr a -> a
                        | Align a -> _class <| sprintf "table-align--%s" (a |> toLowerString)
                        | CellType a -> _class <| sprintf "table-td-%s" (a |> toLowerString)
                        | NoSort -> _class "nosort"
                    )
        |> List.distinct
        |> mergeAttributes []

    let col props value =
         { Value = value; Props = props } 

    let table attributes columns rows = 
        table ([_class "table tablesorter"] |> mergeAttributes attributes) [
                        thead [] [
                            tr [] (columns 
                                  |> List.map(fun col ->    
                                            th (colAttributes col) col.Value    
                                  ))           
                        ]   
                        tbody [] 
                                (rows 
                                    |> List.map(fun row ->
                                        tr [] 
                                            (row
                                            |> List.mapi (fun i value ->
                                                    td 
                                                        (colAttributes columns.[i]) 
                                                        [value]
                                            ))                    
                                        )
                                )
                    ]
