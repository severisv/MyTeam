@echo off
cls
if not EXIST "packages\FAKE\tools\Fake.exe" NuGet.exe "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"



if not defined bower call npm install bower -g
if not defined gulp call npm install gulp -g

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)

node -v

"packages\FAKE\tools\Fake.exe" build.fsx %*