@echo off
cls

node -v

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)

dotnet tool install  --tool-path .paket Paket
fake run build.fsx --parallel 3 %*