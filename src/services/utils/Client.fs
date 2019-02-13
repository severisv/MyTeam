module MyTeam.Client

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared

let view id model =
    div [ _id id
          attr Interop.modelAttributeName (model |> Json.fableSerialize) ] []
