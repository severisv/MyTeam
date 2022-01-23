module Shared.Components.Tables

open Shared
open Shared.Util
open Fable.React
open Fable.React.Props

type TableProperty =
    | Attribute of IHTMLProp
    | Striped

let tableAttributes attr =
    attr
    |> List.map (function
        | Attribute a -> a
        | Striped -> Class "table--striped" :> IHTMLProp)
    |> List.distinct
    |> Html.mergeClasses [
        Class "table tablesorter"
       ]


type TableAlignment =
    | Left
    | Center
    | Right

type CellType =
    | Normal
    | Image

type ScreenSize =
    | ScreenXs
    | ScreenSm
    | ScreenMd
    | ScreenLg

type CellProperty =
    | Align of TableAlignment
    | NoSort
    | Attr of IHTMLProp
    | CellType of CellType
    | ExcludeWhen of bool
    | NoPadding
    | Hidden of ScreenSize
    | Visible of ScreenSize

type TableColumn =
    { Value: ReactElement list
      Props: CellProperty list }

let colAttributes =
    function
    | Some col ->
        col.Props
        |> List.map (function
            | Attr a -> a
            | Align a ->
                Class
                <| sprintf "table-align--%s" (a |> Strings.toLower)
                :> IHTMLProp
            | CellType a ->
                Class
                <| sprintf "table-td-%s" (a |> Strings.toLower)
                :> IHTMLProp
            | NoSort -> Class "nosort" :> IHTMLProp
            | NoPadding -> Class "nopadding" :> IHTMLProp
            | ExcludeWhen _ -> Class "" :> IHTMLProp
            | Hidden size ->
                let size =
                    size
                    |> Strings.toLower
                    |> Strings.replace "screen" ""

                Class($"hidden-%s{size}") :> IHTMLProp
            | Visible size ->
                let size =
                    size
                    |> Strings.toLower
                    |> Strings.replace "screen" ""

                Class($"visible-%s{size}") :> IHTMLProp)
        |> List.distinct
        |> Html.mergeClasses []
    | None -> []

let col props value = { Value = value; Props = props }


type TableRow = IHTMLProp list * ReactElement list
let tableRow attributes values = (attributes, values)

let table (attributes: TableProperty list) (columns: TableColumn list) (rows: TableRow list) =

    let columnIsVisible =
        function
        | Some col ->
            col.Props
            |> List.exists (function
                | ExcludeWhen value -> value
                | _ -> false)
            |> not
        | None -> true

    let getColumn i =
        if columns.Length > i then
            Some columns.[i]
        else
            None

    table
        (tableAttributes attributes)
        [ thead [] [
            tr
                []
                (columns
                 |> List.filter (Some >> columnIsVisible)
                 |> List.map (fun col -> th (colAttributes <| Some col) col.Value))
          ]
          tbody
              []
              (rows
               |> List.map (fun (attributes, row) ->
                   tr
                       attributes
                       (row
                        |> List.mapi (fun i value -> (getColumn i, value))
                        |> List.filter (fun (col, _) -> columnIsVisible col)
                        |> List.map (fun (col, value) -> td (colAttributes col) [ value ])))) ]
