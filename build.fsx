
#r "paket:
    nuget Fake.Core.Targets prerelease
    nuget Fake.Core.Globbing prerelease
    nuget Fake.Core.Process prerelease
    nuget Fake.DotNet.Cli prerelease
    nuget Fake.JavaScript.Npm prerelease
    nuget Fake.Azure.Kudu prerelease
    nuget Fake.IO.Zip prerelease"

#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.JavaScript
open Fake.IO
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.Core.Globbing.Operators
open Fake.Azure


let appName = "myteam"
let webDir = __SOURCE_DIRECTORY__ + "/src/server/"
let publishDirectory = __SOURCE_DIRECTORY__ + "/dist"
let artifactsDirectory = __SOURCE_DIRECTORY__ + "/artifacts"
let artifact = sprintf "%s/%s.zip" artifactsDirectory appName
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
    !! (sprintf "%s/**/*.*" publishDirectory)
    |> Zip.zip publishDirectory artifact


Target.Create "Deploy" <| fun _ ->
    let username = Environment.environVar "DEPLOY_ENV_NAME"
    let password = Environment.environVar "DEPLOY_PWD"

    printf "\n ---------------- %s \n" username
    printf "\n ---------------- %s \n" password
    printf "\n ---------------- %s \n" artifact

    Kudu.zipDeploy ({ 
                    Url = System.Uri <| sprintf "https://%s.scm.azurewebsites.net/" username
                    UserName = username
                    Password = password
                    PackageLocation = artifact
                  }) 

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
==> "Deploy"

Target.RunOrDefault "Deploy"