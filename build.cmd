@echo off
cls

node -v
call gcloud -v
dotnet --version
dotnet tool restore
dotnet fake run build.fsx --parallel 3 %*