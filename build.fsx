open Fake.ProcessHelper
// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AppVeyor
open Fake.FileSystemHelper
open Fake.EnvironmentHelper
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
        printf "running npm command with node version: "
        shellExec executable "--v" workingDir
        shellExec executable args workingDir


    let bower args workingDir =
        let executable = findOnPath "bower.cmd"
        shellExec executable args workingDir

    let gulp args workingDir =
        let executable = findOnPath "gulp.cmd"
        shellExec executable args workingDir

    let dotnet args workingDir =
        let executable = findOnPath "dotnet.exe"
        shellExec executable args workingDir


    let rec execMsdeploy executable args workingDir attempt attempts =
        try
            shellExec executable args workingDir
        with | _ ->
            if attempt > 0 then
                printf "MsDeploy attempt %i: \n" (1+attempts-attempt)
                execMsdeploy executable args workingDir (attempt-1) attempts
            else
                printf "MsDeploy failed after %i attempts \n" attempts
                reraise()

    let msdeploy args workingDir =
        let currentDir = FileSystemHelper.currentDirectory
        let executable = sprintf "%s\webdeploy\msdeploy.exe" currentDir
        execMsdeploy executable args workingDir 3 3


    type DotnetCommands =
        | Restore
        | Build
        | Publish
        | Test

    let Dotnet command target =
        match command with
            | Restore -> (dotnet "restore" target)
            | Build -> (dotnet "build --configuration Release" target)
            | Publish -> (dotnet "publish --configuration Release -o ../../.deploy" target)
            | Test -> (dotnet "test" target)


[<AutoOpen>]
module Settings =
  let deployDir = "./.deploy/"
  let webDir = "src/MyTeam/"
  let projectFiles = !! "src/*/project.json" ++ "test/*/project.json"
  let projects = projectFiles |> Seq.map(fun p -> Directory.GetParent(p).FullName)
  let testProjectFiles = !! "test/*/project.json"
  let testProjects = testProjectFiles |> Seq.map(fun p -> Directory.GetParent(p).FullName)


[<AutoOpen>]
module Targets =
  Target "Clean" (fun() ->
    CleanDirs [deployDir; "./src/MyTeam/wwwroot/compiled"]
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

  Target "WebpackCompile" (fun _ ->
     npm "run build" webDir
  )

  Target "RestorePackages" (fun _ ->
     Dotnet Restore ""
     |> ignore
  )

  Target "Build" (fun _ ->
     projects
     |> Seq.iter (fun proj -> Dotnet Build proj |> ignore)
  )

  Target "Test" (fun _ ->
     testProjects
     |> Seq.iter (fun proj -> Dotnet Test proj |> ignore)
  )

  Target "Publish" (fun _ ->
     Dotnet Publish "src/MyTeam" |> ignore
  )

  Target "Deploy" (fun _ ->
     let currentDir = FileSystemHelper.currentDirectory
     let appName = EnvironmentHelper.environVar "DEPLOY_ENV_NAME"
     let password = EnvironmentHelper.environVar "DEPLOY_PWD"
     let args = sprintf "-source:IisApp='%s\.deploy' -dest:IisApp='%s',ComputerName='https://%s.scm.azurewebsites.net/msdeploy.axd',UserName='$%s',Password='%s',IncludeAcls='False',AuthType='Basic' -verb:sync -enableLink:contentLibExtension -enableRule:AppOffline -retryAttempts:2" currentDir appName appName appName password
     msdeploy args "" |> ignore
  )

  Target "Default" (ignore)

"Clean"
==> "NpmRestore"
==> "BowerRestore"
==> "GulpCompile"
==> "WebpackCompile"
==> "Publish"

"Clean"
==> "RestorePackages"
==> "Build"
==> "Test"
==> "Publish"

"Publish"
==> "Deploy"
==> "Default"

RunTargetOrDefault "Default"
