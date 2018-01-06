namespace Services

open FSharp.Data.Sql

type ConnectionString = string

type DatabaseContext = SqlDataProvider<
                                Common.DatabaseProviderTypes.MSSQLSERVER,
                                "Server=(localdb)\\mssqllocaldb;Database=breddefotball;Trusted_Connection=True;MultipleActiveResultSets=true">

    module Database = 
        let get (connectionString : string) = DatabaseContext.GetDataContext(connectionString)
       
