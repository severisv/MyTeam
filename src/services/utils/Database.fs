namespace MyTeam

open FSharp.Data.Sql

type ConnectionString = string

type DatabaseContext = SqlDataProvider<
                                Common.DatabaseProviderTypes.MSSQLSERVER,
                                "Server=(local)\\SQLExpress;Database=breddefotball;Trusted_Connection=True;MultipleActiveResultSets=true">

module Database = 
    let get (connectionString : ConnectionString) = DatabaseContext.GetDataContext(connectionString)
       

[<AutoOpen>]
module DatabaseExtensions =
    type Microsoft.AspNetCore.Http.HttpContext with
        member ctx.ConnectionString = getConnectionString ctx
                    