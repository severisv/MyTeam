namespace MyTeam.Views

open MyTeam
open Giraffe.ViewEngine

module TableModule =

    type TableProperty =
        | Attribute of XmlAttribute
        | Striped


    let tableAttributes attr =
        attr
        |> List.map
            (function
            | Attribute a -> a
            | Striped -> _class "table--striped")
        |> List.distinct
        |> mergeAttributes [ _class "table tablesorter" ]


    type TableAlignment =
        | Left
        | Center
        | Right

    type CellType =
        | Normal
        | Image

    type CellProperty =
        | Align of TableAlignment
        | NoSort
        | Attr of XmlAttribute
        | CellType of CellType

    type TableColumn =
        { Value: XmlNode list
          Props: CellProperty list }

    let colAttributes col =
        col.Props
        |> List.map
            (function
            | Attr a -> a
            | Align a ->
                _class
                <| sprintf "table-align--%s" (a |> toLowerString)
            | CellType a ->
                _class
                <| sprintf "table-td-%s" (a |> toLowerString)
            | NoSort -> _class "nosort")
        |> List.distinct
        |> mergeAttributes []

    let col props value = { Value = value; Props = props }


    type TableRow = XmlAttribute list * XmlNode list
    let tableRow attributes values = (attributes, values)


    let table (attributes: TableProperty list) columns (rows: TableRow list) =
        table
            (tableAttributes attributes)
            [ thead [] [
                tr
                    []
                    (columns
                     |> List.map (fun col -> th (colAttributes col) col.Value))
              ]
              tbody
                  []
                  (rows
                   |> List.map
                       (fun (attributes, row) ->
                           tr
                               attributes
                               (row
                                |> List.mapi (fun i value -> td (colAttributes columns.[i]) [ value ])))) ]
