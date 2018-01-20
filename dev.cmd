cd src
call code .
cd server
call npm install
call dotnet restore
call npm run watch