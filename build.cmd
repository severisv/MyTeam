@echo off
cls

node -v

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)
dotnet --version
dotnet tool install Paket
fake run build.fsx --parallel 3 %*