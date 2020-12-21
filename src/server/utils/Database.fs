namespace MyTeam

open Giraffe

type Database = MyTeam.Models.ApplicationDbContext

[<AutoOpen>]
module DatabaseExtensions =
    type Microsoft.AspNetCore.Http.HttpContext with
        member ctx.ConnectionString = getConnectionString ctx
        member ctx.Database = ctx.GetService<Database>()
                    
                    