namespace MyTeam

open FSharp.Data.Sql
open Microsoft.AspNetCore.Http

type Guid = System.Guid
type ConnectionString = string

type DatabaseContext = SqlDataProvider<
                                Common.DatabaseProviderTypes.MSSQLSERVER,
                                "Server=(localdb)\\mssqllocaldb;Database=breddefotball;Trusted_Connection=True;MultipleActiveResultSets=true">

type Database = MyTeam.Models.ApplicationDbContext


module Db = 


    let get (connectionString : ConnectionString) = DatabaseContext.GetDataContext(connectionString)

    let getDb (ctx : HttpContext) =  ctx |> getService<Database> 
       

[<AutoOpen>]
module DatabaseExtensions =
    type Microsoft.AspNetCore.Http.HttpContext with
        member ctx.ConnectionString = getConnectionString ctx
        member ctx.Database = Db.getDb ctx
                    