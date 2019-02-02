module MyTeam.Client.Util.ReactHelpers

open Fable.Import
open MyTeam

let render deserializeFn id comp = 
    Browser.document.getElementById id
    |> fun node ->
        
        if not <| isNull node then
            node.getAttribute (Interop.modelAttributeName)
            |> deserializeFn
            |> function
             | Ok model -> ReactDom.render (comp model [], node)
             | Error e -> failwithf "Json deserialization failed: %O" e