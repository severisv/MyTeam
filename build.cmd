@echo off
cls
NuGet.exe "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"
call npm install bower -g
call npm install gulp -g
call npm install npm@3

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)

"packages\FAKE\tools\Fake.exe" build.fsx %*
