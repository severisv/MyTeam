// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open System.IO

    
[<AutoOpen>]
module Targets =
 
  Target "Build" (fun() ->
        let dnu = tryFindFileOnPath (if isUnix then "dnu" else "dnu.cmd")
        let errorCode = match dnu with
                          | Some d -> Shell.Exec(d, "build", "src/MyTeam")
                          | None -> -1
        ()     
        
      
  )

      
  Target "Package" (fun _ ->
    trace "Packing the web"
    )

    

  Target "Default" (fun _ ->
    ()
  )

"Build"
==> "Package"
==> "Default"

RunTargetOrDefault "Default"
