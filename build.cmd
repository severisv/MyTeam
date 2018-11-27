@echo off
cls

node -v

if exist setdeploycredentials.cmd (call setdeploycredentials.cmd)

fake run build.fsx --parallel 3 %*