@echo off
cls
if not EXIST "packages\FAKE\tools\Fake.exe" NuGet.exe "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)

node -v
dotnet --version

"packages\FAKE\tools\Fake.exe" build.fsx %*