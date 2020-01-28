@echo off
cls

node -v

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)
dotnet --version
dotnet tool restore
fake run build.fsx --parallel 3 %*