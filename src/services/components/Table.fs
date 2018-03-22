namespace MyTeam.Views

open MyTeam
open Giraffe
open Giraffe.GiraffeViewEngine

[<AutoOpen>]
module TableModule =  

    type TableAlignment = 
       | Left
       | Center

    type CellType = Normal | Image


    type TableProperty =
       | TableAlignment of TableAlignment
       | NoSort
       | ClassName of string
       | CellType of CellType

    type TableColumn = {
        Value: HtmlValue list
        Props: TableProperty list
    }


    type Table = {
        Rows: HtmlValue list list
        Columns: TableColumn list
    }

    let getAlignment props = 
        props |> List.filter (function 
                        | TableAlignment _ -> true
                        | _ -> false  
                       )
        |> List.tryHead
        |> function
            | Some (TableAlignment t) -> t
            | _ -> Left  

    let getSorting props = 
        props |> List.exists (fun p -> p = NoSort)
              |> function
                | true -> ""
                | false -> "nosort"                   
            

    let colClassName col =
        col.Props
        |> List.map (function
                        | NoSort -> "nosort"
                        | TableAlignment a -> sprintf "table-align--%s" (Enums.toString a |> toLower)
                        | ClassName s -> s
                        | CellType a -> sprintf "table-td-%s" (Enums.toString a |> toLower)

                    )
        |> List.distinct
        |> String.concat " "

    let col value props =
         { Value = value; Props = props } 

    let table attributes (model: Table) = 
        table ([_class "table tablesorter"] |> mergeAttributes attributes) [
                        thead [] [
                            tr [] (model.Columns 
                                  |> List.map(fun col ->    
                                            th [_class <| colClassName col] (col.Value |> List.map toXmlNode)       
                                  ))                           
                            
                        ]
                                    
                        tbody [] 
                                (model.Rows 
                                    |> List.map(fun row ->
                                        tr [] 
                                            (row
                                            |> List.mapi (fun i value ->
                                                    td 
                                                        [_class <| colClassName model.Columns.[i] ] 
                                                        [value |> toXmlNode]
                                            ))                    
                                        )
                                )
                    ]
