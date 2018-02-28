namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open MyTeam

[<AutoOpen>]
module Tabs =  

    type Tab = {
        Text: string
        ShortText: string
        Url: string
        Icon: XmlNode option
    }

    type Tabs = {
        Items: Tab list
        IsSelected: (string -> bool)
    }

    let tabs model =
        if model.Items.Length > 0 then
            div [_class "stats-teamNav"] [
                ul [_class "nav nav-pills mt-justified"] 
                    (model.Items |> List.map (fun t -> 
                                    li [_class (model.IsSelected t.Url =? ("active", ""))] [
                                        a [_href t.Url] [
                                            span [_class "hidden-xs" ] [t.Icon |?? emptyText] 
                                            span [_class "hidden-xs"] [whitespace;encodedText t.Text] 
                                            span [_class "visible-xs"] [encodedText t.ShortText]
                                        ]
                                    ]))
                ]
        else emptyText         