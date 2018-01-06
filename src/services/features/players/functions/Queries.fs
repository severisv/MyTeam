namespace Services.Players

open FSharp.Data.Sql
open System

module Queries =


    type Sql  = SqlDataProvider<
                    Common.DatabaseProviderTypes.MSSQLSERVER,
                    "Server=BEKK-SEVERINS\\SQLEXPRESS;Database=breddefotball;Trusted_Connection=True;MultipleActiveResultSets=true">
    let ctx = Sql.GetDataContext()

    let getPlayers : GetPlayers =
        fun clubId ->

            ctx.Dbo.Member
            |> Seq.filter (fun p -> p.ClubId = clubId)
            |> Seq.filter (fun p -> p.Discriminator = "Player")
            |> Seq.sortBy (fun p -> p.FirstName)
            |> Seq.map (fun p ->
                            {
                                Id = p.Id
                                FirstName = p.FirstName
                                LastName = p.LastName
                                MiddleName = p.MiddleName
                                UrlName = p.UrlName
                                Status = Status.Aktiv
                                Roles = []
                            }
                    )
            |> Seq.toList
