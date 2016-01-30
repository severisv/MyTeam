// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AppVeyor
open System.IO

[<AutoOpen>]
module Dnu = 

    let shellExec cmdPath args target = 
        let result = ExecProcess (
                      fun info ->
                        info.FileName <- cmdPath
                        info.WorkingDirectory <- target
                        info.Arguments <- args
                      ) System.TimeSpan.MaxValue
        if result <> 0 then failwith (sprintf "'%s' failed" cmdPath + " " + args)

    let dnu args target = 
        let executable = tryFindFileOnPath (if isUnix then "dnu" else "dnu.cmd")
        match executable with
            | Some dnu -> shellExec dnu args target
            | None -> failwith ("can't find dnu")
            
    let dnx args target = 
        let executable = tryFindFileOnPath (if isUnix then "dnx" else "dnx.exe")
        match executable with
            | Some dnx -> shellExec dnx args target
            | None -> failwith ("can't find dnx") 
            
    let dnvm args target = 
        let executable = tryFindFileOnPath (if isUnix then "dnvm" else "dnvm.cmd")
        match executable with
            | Some dnvm -> shellExec dnvm args target
            | None -> failwith ("can't find dnvm")            
                                                        
    type Commands =
        | Restore
        | Build
        | Test
        | Publish
     
    let Cmd command target = 
        match command with
            | Restore -> (dnu "restore" target)
            | Build -> (dnu "build --configuration release" target)
            | Test -> (dnx "test" target)
            | Publish -> (dnu "publish --configuration release -o .deploy src/MyTeam" target)


[<AutoOpen>]
module Settings =
  let buildDir = "./.build/"
  let deployDir = "./.deploy/"
  let webDir = "src/MyTeam/"
  let projectFiles = !! "src/*/project.json" ++ "test/*/project.json"
  let projects = projectFiles |> Seq.map(fun p -> Directory.GetParent(p).FullName)
  let testProjectFiles = !! "test/*/project.json"
  let testProjects = testProjectFiles |> Seq.map(fun p -> Directory.GetParent(p).FullName)


[<AutoOpen>]
module Targets =
  Target "Clean" (fun() ->
    CleanDirs [buildDir; deployDir]
  )

  Target "RestorePackages" (fun _ ->
     Cmd Restore ""
     |> ignore
  )

   
  Target "Build" (fun _ ->
     projects 
     |> Seq.iter (fun proj -> Cmd Build proj |> ignore)
  )

  Target "Test" (fun _ ->
     testProjects
     |> Seq.iter (fun proj -> Cmd Test proj |> ignore)
  )

  Target "Publish" (fun _ ->
     Cmd Publish "" |> ignore
  )

  Target "Default" (fun _ -> 
    ()
  )

"Clean"
==> "RestorePackages"
==> "Build"
==> "Test"
==> "Publish"
==> "Default"

RunTargetOrDefault "Default"
