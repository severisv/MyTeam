// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing
open Fake.AppVeyor
open Fake.NuGetHelper
open System.IO

[<AutoOpen>]
module Npm =
  open System

  let npmFileName =
    match isUnix with
      | true -> "/usr/local/bin/npm"
      | _ -> "./packages/Npm.js/tools/npm.cmd"

  type InstallArgs =
    | Standard
    | Forced

  type NpmCommand =
    | Install of InstallArgs
    | Run of string

  type NpmParams = {
    Src: string
    NpmFilePath: string
    WorkingDirectory: string
    Command: NpmCommand
    Timeout: TimeSpan
  }

  let npmParams = {
    Src = ""
    NpmFilePath = npmFileName
    Command = Install Standard
    WorkingDirectory = "."
    Timeout = TimeSpan.MaxValue
  }

  let parseInsallArgs = function
    | Standard -> ""
    | Forced -> " --force"

  let parse command =
    match command with
    | Install installArgs -> sprintf "install%s" (installArgs |> parseInsallArgs)
    | Run str -> sprintf "run %s" str

  let run npmParams =
    let npmPath = Path.GetFullPath(npmParams.NpmFilePath)
    let arguments = npmParams.Command |> parse
    let result = ExecProcess (
                  fun info ->
                    info.FileName <- npmPath
                    info.WorkingDirectory <- npmParams.WorkingDirectory
                    info.Arguments <- arguments
                  ) npmParams.Timeout
    if result <> 0 then failwith (sprintf "'npm %s' failed" arguments)

  let Npm f =
    npmParams |> f |> run


[<AutoOpen>]
module AppVeyorHelpers =
  let execOnAppveyor arguments =
    let result =
      ExecProcess (fun info ->
        info.FileName <- "appveyor"
        info.Arguments <- arguments
        ) (System.TimeSpan.FromMinutes 2.0)
    if result <> 0 then failwith (sprintf "Failed to execute appveyor command: %s" arguments)
    trace "Published packages"

  let publishOnAppveyor folder =
    !! (folder + "*.nupkg")
    |> Seq.iter (fun artifact -> execOnAppveyor (sprintf "PushArtifact %s" artifact))
    
    
[<AutoOpen>]
module Settings =
  let buildDir = "./.build/"
  let packagingDir = buildDir + "MyTeam"
  let deployDir = "./.deploy/"
  let testDir = "./.test/"
  let projects = !! "src/**/*.xproj" -- "src/**/*.Tests.xproj"
  let testProjects = !! "src/**/*.Tests.csproj"
  let packages = !! "./**/packages.config"

  let getOutputDir proj =
    let folderName = Directory.GetParent(proj).Name
    sprintf "%s%s/" buildDir folderName

  let build proj =
    let outputDir = proj |> getOutputDir
    ignore

  let getVersion() =
    let buildCandidate = (environVar "APPVEYOR_BUILD_NUMBER")
    if buildCandidate = "" || buildCandidate = null then "1.0.0" else (sprintf "1.0.0.%s" buildCandidate)

[<AutoOpen>]
module Dnu = 
     let dnu command = 
            let dn = tryFindFileOnPath (if isUnix then "dnu" else "dnu.cmd")
            let errorCode = match dn with
                                | Some d -> Shell.Exec(d, command + " --configuration release", "src/MyTeam")
                                | None -> -1
            () |> ignore    

[<AutoOpen>]
module Targets =


    
   

  Target "Clean" (fun() ->
    CleanDirs [buildDir; deployDir; testDir]
  )

  Target "RestorePackages" (fun _ ->
    packages
    |> Seq.iter (RestorePackage (fun p -> {p with OutputPath = "./src/packages"}))
  )

  Target "Build" (fun() ->
        dnu "build" |> ignore 
  )

  Target "Web" (fun _ ->
    Npm (fun p ->
      { p with
          Command = Install Standard
          WorkingDirectory = "./src/FAKESimple.Web/"
      })

    Npm (fun p ->
      { p with
          Command = (Run "build")
          WorkingDirectory = "./src/FAKESimple.Web/"
      })
  )

  Target "CopyWeb" (fun _ ->
    let targetDir = packagingDir @@ "dist"
    let sourceDir = "./src/FAKESimple.Web/dist"
    CopyDir targetDir sourceDir (fun x -> true)
  )

  Target "BuildTest" (fun() ->
    testProjects
    |> MSBuildDebug testDir "Build"
    |> ignore
  )

  Target "Test" (fun() ->
    !! (testDir + "/*.Tests.dll")
        |> xUnit2 (fun p ->
            {p with
                ShadowCopy = false;
                HtmlOutputPath = Some (testDir @@ "xunit.html");
                XmlOutputPath = Some (testDir @@ "xunit.xml");
            })
  )

  Target "Package" (fun _ ->
    trace "Packing the web"
    dnu "pack" |> ignore 
    )

  Target "Publish" (fun _ ->
    match buildServer with
    | BuildServer.AppVeyor ->
        publishOnAppveyor deployDir
    | _ -> ()
  )
    

  Target "Default" (fun _ ->
    ()
  )

"Clean"
//==> "RestorePackages"
==> "Build"
//==> "Web"
//==> "CopyWeb"
//==> "BuildTest"
//==> "Test"
==> "Package"
//==> "Publish"
//==> "Create release"
//==> "Deploy"
==> "Default"

RunTargetOrDefault "Default"
