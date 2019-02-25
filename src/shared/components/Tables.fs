module Shared.Components.Tables

open Shared
open Shared.Util
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import.React
open Shared.Components.Base

type TableProperty =
    | Attribute of IHTMLProp
    | Striped   

let tableAttributes attr =
    attr
    |> List.map (function
                    | Attribute a -> a
                    | Striped -> Class "table--striped" :> IHTMLProp)
    |> List.distinct
    |> Html.mergeClasses [Class "table tablesorter"]


type TableAlignment = 
   | Left
   | Center
   | Right

type CellType = Normal | Image

type CellProperty =
   | Align of TableAlignment
   | NoSort
   | Attr of IHTMLProp
   | CellType of CellType
   | ExcludeWhen of bool

type TableColumn = {
    Value: ReactElement list
    Props: CellProperty list
}

let colAttributes col =
    col.Props  
    |> List.map (function
                 | Attr a -> a
                 | Align a -> Class <| sprintf "table-align--%s" (a |> Strings.toLower)  :> IHTMLProp 
                 | CellType a -> Class <| sprintf "table-td-%s" (a |> Strings.toLower)  :> IHTMLProp
                 | NoSort -> Class "nosort"  :> IHTMLProp
                 | ExcludeWhen _ -> Class "" :> IHTMLProp)
    |> List.distinct
    |> Html.mergeClasses []

let col props value =
     { Value = value; Props = props } 


type TableRow = IHTMLProp list * ReactElement list
let tableRow attributes values = (attributes, values)

let table (attributes: TableProperty list) columns (rows: TableRow list) =
    
    let columnIsVisible a = a.Props |> List.exists(function | ExcludeWhen value -> value | _ -> false ) |> not
        
    table (tableAttributes attributes) [
                    thead [] [
                        tr [] (columns
                              |> List.filter columnIsVisible
                              |> List.map(fun col ->    
                                        th (colAttributes col) col.Value    
                              ))           
                    ]   
                    tbody [] 
                            (rows
                                |> List.map(fun (attributes, row) ->
                                    tr attributes 
                                        (row
                                        |> List.mapi (fun i value -> (columns.[i], value))
                                        |> List.filter (fun (col, _) -> columnIsVisible col)
                                        |> List.map (fun (col, value) ->
                                                    td (colAttributes col) [value] )                                                                                       
                                        )            
                                    )
                            )
                ]
