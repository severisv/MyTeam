@echo off
cls
if not EXIST "packages\FAKE\tools\Fake.exe" NuGet.exe "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"



REM call npm install bower -g
REM call npm install gulp -g
REM call npm install npm@5 -g

REM set PATH=%APPDATA%\npm;%PATH%

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)

"packages\FAKE\tools\Fake.exe" build.fsx %*