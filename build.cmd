@echo off
cls
if not EXIST "packages\FAKE\tools\Fake.exe" NuGet.exe "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"



call npm install bower -g
call npm install gulp -g
call npm install npm@5 -g

set PATH=%APPDATA%\npm;%PATH%

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)

"packages\FAKE\tools\Fake.exe" build.fsx %*