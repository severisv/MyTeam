namespace MyTeam

open Microsoft.AspNetCore.Http

type Database = MyTeam.Models.ApplicationDbContext
type Guid = System.Guid

module Db = 

    let getDb (ctx : HttpContext) =  ctx |> getService<Database> 
       

[<AutoOpen>]
module DatabaseExtensions =
    type Microsoft.AspNetCore.Http.HttpContext with
        member ctx.ConnectionString = getConnectionString ctx
        member ctx.Database = Db.getDb ctx
                    