namespace MyTeam

open FSharp.Data.Sql
open Microsoft.AspNetCore.Http

type ConnectionString = string

type DatabaseContext = SqlDataProvider<
                                Common.DatabaseProviderTypes.MSSQLSERVER,
                                "Server=(local)\\SQLExpress;Database=breddefotball;Trusted_Connection=True;MultipleActiveResultSets=true">

module Database = 

    type Database = MyTeam.Models.ApplicationDbContext

    let get (connectionString : ConnectionString) = DatabaseContext.GetDataContext(connectionString)

    let getDb (ctx : HttpContext) =  ctx |> getService<Database> 
       

[<AutoOpen>]
module DatabaseExtensions =
    type Microsoft.AspNetCore.Http.HttpContext with
        member ctx.ConnectionString = getConnectionString ctx
        member ctx.Database = Database.getDb ctx
                    