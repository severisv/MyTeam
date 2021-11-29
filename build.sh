#!/bin/bash

node -v
gcloud -v
dotnet --version
dotnet tool restore
dotnet fake run build.fsx --parallel 3 $1 $2