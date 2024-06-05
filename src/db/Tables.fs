namespace db

open System
open Microsoft.EntityFrameworkCore
open System.ComponentModel.DataAnnotations


module Tables =


    [<CLIMutable>]
    type Player =
        { Id: Guid
          FirstName: string
          MiddleName: string option
          LastName: string
          UrlName: string option
          ClubId: Guid
          UserName: string option
          FacebookId: string option
          RolesString: string option
          Status: int
          BirthDate: Nullable<DateTime>
          StartDate: Nullable<DateTime>
          Phone: string option
          ImageFull: string option
          ProfileIsConfirmed: bool

         }



    type DataContext(options: DbContextOptions<DataContext>) =
        inherit DbContext(options)

        member this.Players: DbSet<Player> = this.Set()


        override __.OnModelCreating(builder: ModelBuilder) =
            ``base``.OnModelCreating(builder)

            builder.Entity<Player>().ToTable("Member")
            |> ignore
