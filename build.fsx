open Fake.ProcessHelper
// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
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
        shellExec executable args workingDir |> ignore


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


[<AutoOpen>]
module Settings =
  let webDir = "src/server/"


[<AutoOpen>]
module Targets =
  Target "Clean" (fun() ->
    CleanDirs ["./.deploy/"; "./src/server/wwwroot/compiled" ]
  )

  Target "NpmRestore" (fun _ ->
     npm "install" webDir
  )

  Target "CopyClientLibs" (fun _ ->
     npm "run copy-libs" webDir
  )

  Target "WebpackCompile" (fun _ ->
     npm "run build" webDir
  )

  Target "RestorePackages" (fun _ ->       
    dotnet "restore" webDir
  )

  Target "MigrateDatabase" (fun _ ->
     dotnet "ef database update" "src/database"         
  )

  Target "Build" (fun _ ->
     dotnet "build --configuration Release" webDir         
  )

  Target "Publish" (fun _ ->
     dotnet "publish --configuration Release -o ../../.deploy" webDir
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
==> "CopyClientLibs"
==> "WebpackCompile"
==> "Publish"

"Clean"
==> "RestorePackages"
==> "MigrateDatabase"
==> "Build"
==> "Publish"

"Publish"
==> "Deploy"
==> "Default"

RunTargetOrDefault "Default"
