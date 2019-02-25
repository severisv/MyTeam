module Shared.Components.Nav

open Shared
open Shared.Components.Base
open Fable.Import.React
open Fable.Helpers.React.Props
open Fable.Helpers.React

type NavListBaseAttribute =
    | Header of ReactElement
    | Footer of ReactElement option
    interface IHTMLProp


let navListBase (attributes : IHTMLProp list) children =

     let props = attributes
                |> List.filter (fun p -> p :? NavListBaseAttribute)
                |> List.map (fun p -> p :?> NavListBaseAttribute)

     let otherProps = attributes
                      |> List.filter (fun e -> attributes |> List.contains e)

     let header =
         props
         |> List.map (function | Header h -> Some h | _ -> None)
         |> List.choose id
         |> List.tryHead

     let footer =
         props
         |> List.map (function | Footer f -> f | _ -> None)
         |> List.choose id
         |> List.tryHead

     ul ([ Class "nav nav-list" ] @ otherProps) ([
        header => fun header -> li [ Class "nav-header" ] [ header ]
     ] @
        (children |> List.map (fun e -> li [] [e])) @
            (footer
            |> Option.map (fun footer ->
                    [ 
                        li [] [ hr [] ]
                        li [] [ footer ]
                    ])
            |> Option.defaultValue [])
     )


type NavItem = {
    Text : ReactElement list
    Url : string
 }

type NavList = {
    Header : string
    Items : NavItem list
    Footer : NavItem option
    IsSelected : string -> bool }

let navList model =
    model.Items.Length > 0 &?
        navListBase [
                Header <| str model.Header
                Footer(model.Footer
                         |> Option.map (fun footer ->
                             a [ Href footer.Url; Class(if model.IsSelected footer.Url then "active" else "") ] footer.Text
                        ))
                ]
            (model.Items
             |> List.map (fun item ->
                                a [ Href <| item.Url; Class(if model.IsSelected <| item.Url then "active" else "") ] item.Text
                         ))


type SelectNavItem = {
    Text : string
    Url : string }

type SelectNavList = {
    Items : SelectNavItem list
    Footer : SelectNavItem option
    IsSelected : string -> bool }

let selectNav classes model =
    let cls = classes |> List.tryHead |> Option.defaultValue ""
    if model.Items.Length > 1 then
        div [ Class cls ] [
                select [ Class "linkSelect form-control pull-right" ]
                    ((model.Items |> List.map (fun item ->
                                        option [ Value item.Url; Selected <| model.IsSelected item.Url ] [
                                            str item.Text
                                        ])) @
                    match model.Footer with
                    | Some footer ->
                        [ option [ Value footer.Url; Selected <| model.IsSelected footer.Url ] [
                            str footer.Text ] ]
                    | None -> []) ]
    else empty


let navListMobile =
    selectNav [ "nav-list--mobile pull-right hidden-md hidden-lg" ]
