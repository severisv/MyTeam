@echo off
cls

dotnet tool install fake-cli --tool-path .fake  --version 5.0.0-*

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)

node -v
dotnet --version

.\.fake\fake run build.fsx --parallel 3 %*