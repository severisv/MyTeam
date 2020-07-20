module MyTeam.Client

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared

let viewOld id model =
    div
        [ _id id
          attr Interop.modelAttributeName (model |> Json.fableSerialize) ] []

let view containerId comp model =

    [ script []
          [ rawText
              (sprintf """
                    var __INIT_STATE__ = '%s'
                    """ (model 
                        |> Json.fableSerialize
                        |> Strings.replace "\\" "\\\\")
               ) ]

      div [ _id containerId ]
          [ rawText
            <| Fable.ReactServer.renderToString (comp model []) ] ]
    |> renderHtmlNodes
    |> rawText

let view2 containerId comp model =

    [ script []
        [ rawText
              (sprintf """
                    var __INIT_STATE__ = '%s'
                    """ (model 
                        |> Json.fableSerialize
                        |> Strings.replace "\\" "\\\\")
               ) ]

      div [ _id containerId ]
          [ rawText
            <| Fable.ReactServer.renderToString (comp (model)) ] ]
    |> renderHtmlNodes
    |> rawText
