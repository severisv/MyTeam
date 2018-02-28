namespace MyTeam.Views

open MyTeam
open Giraffe
open Giraffe.GiraffeViewEngine

[<AutoOpen>]
module TableModule =  

    type TableAlignment = 
       | Left
       | Center


    type TableColumn = {
        Value: HtmlValue list
        Align: TableAlignment
    }

    type TableType = Normal | Image

    type Table = {
        Type: TableType
        Rows: HtmlValue list list
        Columns: TableColumn list
    }


    let alignmentClass alignment =
        sprintf "table-align--%s" (Enums.toString alignment |> toLower)

    let table attributes (model: Table) = 
        table ([_class "table tablesorter"] |> mergeAttributes attributes) [
                        thead [] [
                            tr [] (model.Columns 
                                  |> List.map(fun col ->                                       
                                    th [_class <| alignmentClass col.Align] (col.Value |> List.map toXmlNode)       
                                  ))                           
                            
                        ]
                                    
                        tbody [] 
                                (model.Rows |> List.map(fun row ->
                                                    tr [] 
                                                        (row
                                                        |> List.mapi (fun i value ->
                                                                td 
                                                                    [_class <| sprintf "%s table-td-%s" (alignmentClass model.Columns.[i].Align) (Enums.toString model.Type |> toLower) ] 
                                                                    [value |> toXmlNode]
                                                        ))                                                           
                                                        
                                        )
                                )
                    ]
