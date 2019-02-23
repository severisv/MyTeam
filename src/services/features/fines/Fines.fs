module Server.Features.Fines.Common

open Shared.Domain.Members
open MyTeam
open MyTeam.Views
open Shared.Components

let fineNav (user : Users.User) (currentPath : string) =
    tabs []
       ([
        { Text = " Oversikt"
          ShortText = " Oversikt"
          Url = "/intern/boter"
          Icon = Some !!(Icons.barChart "") }
        { Text = " Alle bøter"
          ShortText = " Bøter"
          Url = "/intern/boter/vis"
          Icon = Some !!(Icons.fine "") }
        { Text = " Bøtesatser"
          ShortText = " Bøtesatser"
          Url = "/intern/boter/satser"
          Icon = Some !!(Icons.dollar "") }
       ] @ (if user.IsInRole [ Role.Botsjef ] then
                [ { Text = " Innbetalinger"
                    ShortText = " Innbetalinger"
                    Url = "/intern/innbetalinger"
                    Icon = Some !!(Icons.list "") } ]
            else []
                )) (currentPath.Contains)

