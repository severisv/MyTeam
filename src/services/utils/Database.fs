namespace MyTeam

open FSharp.Data.Sql
open Microsoft.AspNetCore.Http

type Guid = System.Guid
type ConnectionString = string

#if RELEASE
type DatabaseContext = SqlDataProvider<
                                Common.DatabaseProviderTypes.MSSQLSERVER,
                                "Server=(local)\\SQL2017;Database=master;User ID=sa;Password=Password12!">

#endif
#if DEBUG
type DatabaseContext = SqlDataProvider<
                                Common.DatabaseProviderTypes.MSSQLSERVER,
                                "Server=(localdb)\\mssqllocaldb;Database=breddefotball;Trusted_Connection=True;MultipleActiveResultSets=true">
#endif


type Database = MyTeam.Models.ApplicationDbContext


module Db = 


    let get (connectionString : ConnectionString) = DatabaseContext.GetDataContext(connectionString)

    let getDb (ctx : HttpContext) =  ctx |> getService<Database> 
       

[<AutoOpen>]
module DatabaseExtensions =
    type Microsoft.AspNetCore.Http.HttpContext with
        member ctx.ConnectionString = getConnectionString ctx
        member ctx.Database = Db.getDb ctx
                    