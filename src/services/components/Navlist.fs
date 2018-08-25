namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open MyTeam

[<AutoOpen>]
module NavList =  

    type NavItem = {
        Text: XmlNode list
        Url: string
    }

    type NavList = {
        Header: string
        Items: NavItem list
        Footer: NavItem option
        IsSelected: (string -> bool)
    }

    let navList model =
        if model.Items.Length > 0 then
            ul [_class "nav nav-list"]
                (
                [ li [_class "nav-header"] [encodedText model.Header] ] @   
                (model.Items |> List.map (fun item  ->
                                    li [] [
                                        a [_href <| item.Url;_class (model.IsSelected <| item.Url =? ("active", "")) ] item.Text
                                    ] 
                                   )) @
                match model.Footer with 
                | Some footer ->   
                    [ 
                    li [] [hr [] ]
                    li [] [a [_href footer.Url;_class (model.IsSelected footer.Url =? ("active", ""))] footer.Text]
                    ]      
                | None -> [] 
                )  
        else emptyText             
    


    type SelectNavItem = {
        Text: string
        Url: string
    }

    type SelectNavList = {
        Header: string
        Items: SelectNavItem list
        Footer: SelectNavItem option
        IsSelected: (string -> bool)
    }

    let navListMobile model =
        if model.Items.Length > 1 then
            div [_class "nav-list--mobile pull-right"] [
                    select [_class "linkSelect form-control pull-right hidden-md hidden-lg"]  
                        (
                        (model.Items |> List.map (fun item ->
                                            option [_value item.Url; model.IsSelected item.Url =? (_selected, _empty) ] [
                                                encodedText item.Text
                                            ] 
                                           )) @
                        match model.Footer with
                        | Some footer ->                    
                            [ 
                                option [_value footer.Url; model.IsSelected footer.Url =? (_selected, _empty)] [
                                    encodedText footer.Text
                                ]
                            ]     
                        | None -> [])     
                ]
        else emptyText         