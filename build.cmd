@echo off
cls

node -v
dotnet --version

dotnet tool install fake-cli --tool-path .fake  --version 5.0.0-*

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)

.\.fake\fake run build.fsx --parallel 3 %*