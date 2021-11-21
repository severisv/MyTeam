module Shared.Components.Tabs

open Shared
open Shared.Components.Base
open Fable.React
open Fable.React
open Fable.React.Props

type Tab =
    { Text: string
      ShortText: string
      Url: string
      Icon: ReactElement option }

let tabs attributes (items: Tab list) (isSelected: (string -> bool)) =
    if items.Length > 1 then
        div
            attributes
            [ ul
                  [ Class "nav nav-pills" ]
                  (items
                   |> List.map




                       (fun t ->
                           li [ Class(
                                    if isSelected t.Url then
                                        "active"
                                    else
                                        ""
                                ) ] [
                               a [ Href t.Url ] [
                                   span [ Class "hidden-xs" ] [
                                       t.Icon |> Option.defaultValue empty
                                   ]
                                   span [ Class "hidden-xs" ] [
                                       whitespace
                                       str t.Text
                                   ]
                                   span [ Class "visible-xs" ] [
                                       str t.ShortText
                                   ]
                               ]
                           ])) ]
    else
        empty
