module MyTeam.Client

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared

let view id model =
    div [ _id id
          attr Interop.modelAttributeName (model |> Json.fableSerialize) ] []

let view2 containerId comp model =
    [
        script []
            [ rawText (sprintf """
                    var __INIT_STATE__ = '%s'
                    """
                    (model |> Json.fableSerialize)) ]
    
        div [_id containerId] [
            rawText <| Fable.Helpers.ReactServer.renderToString (comp model [])         
        ]
    ]