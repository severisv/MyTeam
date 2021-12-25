module MyTeam.Client

open Giraffe.ViewEngine
open MyTeam
open Shared

let comp id model =
    div [ _id id
          attr Interop.modelAttributeName (model |> Json.fableSerialize) ] []

let view containerId comp model =
    [ script [] [
        rawText (
            sprintf
                """
                    var __INIT_STATE__ = '%s'
                    """
                (model
                 |> Json.fableSerialize
                 |> Strings.replace "\\" "\\\\"
                 |> Strings.replace "'" "")
        )
      ]

      div [ _id containerId ] [
          rawText
          <| Fable.ReactServer.renderToString (comp model [])
      ] ]
    |> RenderView.AsString.htmlNodes
    |> rawText

let view2 containerId comp model =
    [ script [] [
        rawText (
            sprintf
                """
                    var __INIT_STATE__ = '%s'
                    """
                (model
                 |> Json.fableSerialize
                 |> Strings.replace "\\" "\\\\"
                 |> Strings.replace "'" "")
        )
      ]

      div [ _id containerId ] [
          rawText
          <| Fable.ReactServer.renderToString (comp (model))
      ] ]
    |> RenderView.AsString.htmlNodes
    |> rawText
