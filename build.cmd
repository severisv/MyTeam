@echo off
cls
NuGet.exe "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"
call npm install bower -g
call npm install gulp -g
"packages\FAKE\tools\Fake.exe" build.fsx %*