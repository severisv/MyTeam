// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing
open Fake.AppVeyor
open Fake.NuGetHelper
open System.IO


[<AutoOpen>]
module Dnu = 

    let dnu args target = 
        let executable = tryFindFileOnPath (if isUnix then "dnu" else "dnu.cmd")
        match executable with
            | Some dnu -> Shell.Exec(dnu, args, target)
            | None -> -1   
                                              
    type DnuCommands =
        | Restore
     
    let Dnu command target = 
        match command with
            | Restore -> (dnu "restore" target)


[<AutoOpen>]
module Settings =
  let buildDir = "./.build/"
  let deployDir = "./.deploy/"
  let projects = !! "src/*/project.json"
  let testProjects = !! "test/*/project.json"


  let getVersion() =
    let buildCandidate = (environVar "APPVEYOR_BUILD_NUMBER")
    if buildCandidate = "" || buildCandidate = null then "1.0.0" else (sprintf "1.0.0.%s" buildCandidate)



[<AutoOpen>]
module Targets =
  Target "Clean" (fun() ->
    CleanDirs [buildDir; deployDir]
  )

  Target "RestorePackages" (fun _ ->
     Dnu Restore ""
     |> ignore
  )

  Target "Default" (fun _ ->
    dnu "restore" ""
    |> ignore
  )

"Clean"
==> "RestorePackages"
==> "Default"

RunTargetOrDefault "Default"
