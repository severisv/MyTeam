start rider ./MyTeam.sln
cd src\client
call npm install
cd..
cd server
call npm install
call dotnet restore
call npm run copy-libs
start dotnet watch run
start npm run watch
call npm run watch-fable