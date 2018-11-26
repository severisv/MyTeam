@echo off
cls

node -v
dotnet --version

dotnet tool install fake-cli --tool-path .fake  --version 5.10.1

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)

.\.fake\fake run build.fsx --parallel 3 %*