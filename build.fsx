
#r "paket:
    nuget Fake.Core.Targets prerelease
    nuget Fake.Core.Globbing prerelease
    nuget Fake.Core.Process prerelease
    nuget Fake.DotNet.Cli prerelease
    nuget Fake.JavaScript.Npm prerelease
    nuget Fake.IO.Zip prerelease"

#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.JavaScript
open Fake.IO
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.Core.Globbing.Operators


let appName = "myteam"
let webDir = __SOURCE_DIRECTORY__ + "/src/server/"
let publishDirectory = __SOURCE_DIRECTORY__ + "/dist"
let artifactsDirectory = __SOURCE_DIRECTORY__ + "/artifacts"
let databaseDirectory = __SOURCE_DIRECTORY__ + "/src/database"

let cleanDirs = Shell.cleanDirs


Target.Create "Clean" <| fun _ ->
    cleanDirs [publishDirectory; artifactsDirectory]


let npmOptions = (fun (p: Npm.NpmParams) -> { p with WorkingDirectory = webDir })

Target.Create "Restore-frontend" <| fun _ ->
    Npm.install npmOptions


Target.Create "Copy-client-libs" <| fun _ ->
    Npm.run "copy-libs" npmOptions

Target.Create "Build-frontend" <| fun _ ->
    Npm.run "build" npmOptions


Target.Create "Restore-backend" <| fun _ ->       
    DotNet.restore id webDir

Target.Create "Migrate-database" <| fun _ ->
    DotNet.exec 
        (fun o ->  
          { o with  
              WorkingDirectory = databaseDirectory
          }) 
         "ef database update" ""|> ignore        


Target.Create "Build-backend" <| fun _ ->
    DotNet.build id webDir         


Target.Create "Publish" <| fun _ ->     
    DotNet.publish 
        (fun o ->  
          { o with  
              OutputPath = Some publishDirectory
          }) 
        webDir



Target.Create "Create-Artifact" <| fun _ ->       
    Directory.ensure artifactsDirectory
    let artifactFilename = sprintf "%s/%s.zip" artifactsDirectory appName
    !! (sprintf "%s/**/*.*" publishDirectory)
    |> Zip.zip publishDirectory artifactFilename

// Target.create "Deploy" (fun _ ->
//  let currentDir = FileSystemHelper.currentDirectory
//  let appName = EnvironmentHelper.environVar "DEPLOY_ENV_NAME"
//  let password = EnvironmentHelper.environVar "DEPLOY_PWD"
//  let args = sprintf "-source:IisApp='%s\.deploy' -dest:IisApp='%s',ComputerName='https://%s.scm.azurewebsites.net/msdeploy.axd',UserName='$%s',Password='%s',IncludeAcls='False',AuthType='Basic' -verb:sync -enableLink:contentLibExtension -enableRule:AppOffline -retryAttempts:2" currentDir appName appName appName password
//  msdeploy args "" |> ignore
// )


"Clean"
==> "Restore-frontend"
==> "Copy-client-libs"
==> "Build-frontend"
==> "Publish"

"Clean"
==> "Restore-backend"
==> "Build-backend"
==> "Migrate-database"
==> "Publish"

"Publish"
==> "Create-Artifact"

Target.RunOrDefault "Create-Artifact"