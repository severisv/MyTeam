@echo off
cls
NuGet.exe "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"
NuGet.exe "Install" "Node.js" "-OutputDirectory" "packages" "-ExcludeVersion"
SET NodeJsPath=%~dp0packages\Node.js
SET PATH=%PATH%;%NodeJsPath%
NuGet.exe "Install" "Npm.js" "-OutputDirectory" "packages" "-ExcludeVersion"
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}"
"packages\FAKE\tools\Fake.exe" build.fsx %*
