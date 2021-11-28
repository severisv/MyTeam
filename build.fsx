#r "paket:
    nuget Fake.Core.Target
    nuget Fake.Core.Globbing
    nuget Fake.Core.Process
    nuget Fake.DotNet.Cli
    nuget Fake.JavaScript.Npm
    nuget FSharp.Core"

#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.JavaScript
open Fake.IO
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.Core.Globbing.Operators
open System


let webDir = __SOURCE_DIRECTORY__ + "/src/server/"
let clientDir = __SOURCE_DIRECTORY__ + "/src/client"
let publishDirectory = __SOURCE_DIRECTORY__ + "/dist"
let artifactsDirectory = __SOURCE_DIRECTORY__ + "/artifacts"
let databaseDirectory = __SOURCE_DIRECTORY__ + "/src/database"

let gcpProjectName = "breddefotball"

let dockerRepository = $"eu.gcr.io/{gcpProjectName}/server"

let commitSha =
    lazy
        (let result =
            [ "rev-parse"; "HEAD" ]
            |> CreateProcess.fromRawCommand "git"
            |> CreateProcess.redirectOutput
            |> Proc.run

         if result.ExitCode <> 0 then
             failwith "Failed getting git commit SHA"

         result.Result.Output.Trim())


// Lazily install DotNet SDK in the correct version if not available
let dotnetCliPath =
    lazy (DotNet.install DotNet.Versions.FromGlobalJson)

let inline dotnetOptions arg =
    DotNet.Options.lift dotnetCliPath.Value arg


Target.create "Clean"
<| fun _ ->
    Shell.cleanDirs [ publishDirectory
                      artifactsDirectory ]

let npmOptions =
    (fun (p: Npm.NpmParams) -> { p with WorkingDirectory = webDir })

Target.create "Restore-frontend"
<| fun _ ->
    Npm.install npmOptions
    DotNet.restore dotnetOptions (clientDir + "/src")
    Npm.install (fun p -> { p with WorkingDirectory = clientDir })


Target.create "Copy-client-libs"
<| fun _ ->
    Shell.copyDir
        (sprintf "%s/wwwroot/compiled/lib/tinymce" webDir)
        (sprintf "%s/node_modules/tinymce" webDir)
        (fun _ -> true)

    Npm.run "copy-libs" npmOptions

Target.create "Build-frontend"
<| fun _ ->
    Npm.run "build" npmOptions
    Npm.run "build" (fun p -> { p with WorkingDirectory = clientDir })


Target.create "Restore-backend"
<| fun _ -> DotNet.restore dotnetOptions webDir

Target.create "Migrate-database"
<| fun _ ->
    DotNet.exec
        (fun o ->
            { o with WorkingDirectory = databaseDirectory }
            |> dotnetOptions)
        "ef database update"
        ""
    |> ignore


Target.create "Build-backend"
<| fun _ -> DotNet.build dotnetOptions webDir


Target.create "Publish"
<| fun _ ->
    DotNet.publish
        (fun o ->
            { o with OutputPath = Some publishDirectory }
            |> dotnetOptions)
        webDir

Target.create "Write-Asset-Hashes"
<| fun _ ->

    let calculateFileHash path =
        use hashImp =
            System.Security.Cryptography.HashAlgorithm.Create("MD5")

        use stream = System.IO.File.OpenRead path
        let hash = hashImp.ComputeHash stream

        BitConverter
            .ToString(hash)
            .Replace("-", String.Empty)

    let writeToAppsettings scriptName hash =
        let configValue = sprintf "\"%s\": \"%s\"" scriptName

        !!(sprintf "%s/**/%s" publishDirectory "appsettings.json")
        |> Seq.iter (fun appsettings ->
            let text = System.IO.File.ReadAllText appsettings
            System.IO.File.WriteAllText(appsettings, text.Replace(configValue "", configValue hash)))


    [ ("MainCss", "site.bundle.css")
      ("LibJs", "lib.bundle.js")
      ("FableJs", "main.js")
      ("MainJs", "app.js") ]
    |> List.iter (fun (scriptName, fileName) ->
        !!(sprintf "%s/**/%s" publishDirectory fileName)
        |> Seq.iter (fun path ->
            calculateFileHash path
            |> writeToAppsettings scriptName))


let docker cmd =
    let result =
        cmd
        |> CreateProcess.fromRawCommand "docker"
        |> Proc.run

    if result.ExitCode <> 0 then
        failwith $"""Docker command failed: {cmd |> String.concat " "}"""

Target.create "Build-docker-image"
<| fun _ ->
    let tag = commitSha.Value

    docker [ "build"
             "-t"
             $"{dockerRepository}:{tag}"
             "." ]


Target.create "Push-docker-image"
<| fun _ ->
    let tag = commitSha.Value

    docker [ "push"
             $"{dockerRepository}:{tag}" ]


Target.create "Deploy" <| fun _ -> printf "Deploy"


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
==> "Write-Asset-Hashes"

"Write-Asset-Hashes"
==> "Build-docker-image"
==> "Push-docker-image"
==> "Deploy"

Target.runOrDefault "Deploy"
