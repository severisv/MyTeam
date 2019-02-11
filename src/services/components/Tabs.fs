namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Views.BaseComponents

[<AutoOpen>]
module Tabs =  

    type Tab = {
        Text: string
        ShortText: string
        Url: string
        Icon: XmlNode option
    }

    let tabs attributes (items: Tab list) (isSelected: (string -> bool)) =
        if items.Length > 1 then
            div attributes [
                ul [_class "nav nav-pills mt-justified"] 
                    (items 
                     |> List.map (fun t -> 
                                    li [_class (isSelected t.Url =? ("active", ""))] [
                                        a [_href t.Url] [
                                            span [_class "hidden-xs" ] [t.Icon |> Option.defaultValue emptyText] 
                                            span [_class "hidden-xs"] [whitespace;encodedText t.Text] 
                                            span [_class "visible-xs"] [encodedText t.ShortText]
                                        ]
                                    ]))
                ]
        else emptyText         