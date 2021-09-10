@echo off
cls

node -v

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)
dotnet --version
dotnet tool restore
dotnet fake run build.fsx --parallel 3 %*