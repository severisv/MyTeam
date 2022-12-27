@echo off
cls

node -v
call gcloud -v
dotnet --version
dotnet run --parallel 3 %*