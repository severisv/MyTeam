module MyTeam.Client

open Giraffe.ViewEngine
open MyTeam
open Shared
open Shared.Util

let propsVariableName = ReactHelpers.propsVariableName


let comp id model =
    let json = model |> Json.fableSerialize

    div [ _id id

         ] [
        script [] [
            rawText
                $"""
                var {propsVariableName id} = '{json}';
                """
        ]
    ]

let view containerId comp model =
    [ script [] [
        rawText (
            sprintf
                """
                    var %s = '%s'
                    """
                (propsVariableName containerId)
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
                    var %s = '%s'
                    """
                (propsVariableName containerId)
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
