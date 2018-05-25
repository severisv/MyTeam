call code .
cd src\server
call npm install
call dotnet restore
call npm run copy-libs
call npm run watch