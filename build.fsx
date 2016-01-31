// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AppVeyor
open System.IO

[<AutoOpen>]
module Helpers = 

    let shellExec cmdPath args target = 
        let result = ExecProcess (
                      fun info ->
                        info.FileName <- cmdPath
                        info.WorkingDirectory <- target
                        info.Arguments <- args
                      ) System.TimeSpan.MaxValue
        if result <> 0 then failwith (sprintf "'%s' failed" cmdPath + " " + args)

    let findOnPath name = 
        let executable = tryFindFileOnPath name
        match executable with
            | Some exec -> exec
            | None -> failwith (sprintf "'%s' can't find" name)

    let npm args workingDir = 
        let executable = findOnPath "npm.cmd"
        shellExec executable args workingDir
   
    let bower args workingDir = 
        let executable = findOnPath "bower.cmd"
        shellExec executable args workingDir

    let gulp args workingDir = 
        let executable = findOnPath "gulp.cmd"
        shellExec executable args workingDir
  
    let dnu args workingDir = 
        let executable = findOnPath "dnu.cmd"
        shellExec executable args workingDir

          
    let dnx args workingDir = 
            let executable = findOnPath "dnx.exe"
            shellExec executable args workingDir    
   
    let dnvm args workingDir = 
        let executable = findOnPath "dnvm.cmd"
        shellExec executable args workingDir    
            
                                                          
    type DnuCommands =
        | Restore
        | Build
        | Publish
     
    let Dnu command target = 
        match command with
            | Restore -> (dnu "restore" target)
            | Build -> (dnu "build --configuration release" target)
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
  
  Target "NpmRestore" (fun _ ->
     npm "install" webDir
  )

  Target "BowerRestore" (fun _ ->
        bower "install" webDir
        )
   
  Target "GulpCompile" (fun _ ->
     gulp "--production" webDir
  )

  Target "RestorePackages" (fun _ ->
     Dnu Restore ""
     |> ignore
  )
     
  Target "Build" (fun _ ->
     projects 
     |> Seq.iter (fun proj -> Dnu Build proj |> ignore)
  )

  Target "Test" (fun _ ->
     testProjects
     |> Seq.iter (fun proj -> dnx "test" proj |> ignore)
  )

  Target "Publish" (fun _ ->
     Dnu Publish "" |> ignore
  )

  Target "Default" (fun _ -> 
    ()
  )

"Clean"
==> "NpmRestore"
==> "BowerRestore"
==> "GulpCompile"
==> "RestorePackages"
==> "Build"
==> "Test"
==> "Publish"
==> "Default"

RunTargetOrDefault "Default"
