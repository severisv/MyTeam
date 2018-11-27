@echo off
cls

node -v

cd ..

dotnet --version

dotnet tool install fake-cli --tool-path .\myteam\.fake  --version 5.10.1

cd myteam

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)

.\.fake\fake run build.fsx --parallel 3 %*