module MyTeam.Client

open Giraffe.ViewEngine
open MyTeam
open Shared
open Shared.Util

let propsVariableName = ReactHelpers.propsVariableName

let comp comp model =
    span [ attr ReactHelpers.componentDataAttribute (model |> Json.fableSerialize) ] [
        rawText
        <| Fable.ReactServer.renderToString (comp model)
    ]

let clientView id model =
    let json =
        model
        |> Json.fableSerialize
        |> Strings.replace "'" ""
        |> Strings.replace "\\n" ""


    div [ _id id ] [
        script [] [
            rawText
                $"""
                var {propsVariableName id} = '{json}';
                """
        ]
    ]



let internal renderIsomorphic containerId comp model =
    [ script [] [
          rawText (
              sprintf
                  """
                    var %s = '%s'
                    """
                  (propsVariableName containerId)
                  (model
                   |> Json.fableSerialize
                   |> Strings.replace "'" ""
                   |> Strings.replace "\\n" "")
          )
      ]

      div [ _id containerId ] [
          rawText <| Fable.ReactServer.renderToString comp
      ] ]
    |> RenderView.AsString.htmlNodes
    |> rawText



let isomorphicViewOld containerId comp model =
    renderIsomorphic containerId ((comp model [])) model

let isomorphicView containerId comp model =
    renderIsomorphic containerId (comp model) model
