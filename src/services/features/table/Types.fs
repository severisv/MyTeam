namespace MyTeam.Table

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Table


type Table = {
    Rows: TableRow list
    UpdatedDate: System.DateTime
    Title: string
}

type Year = int
type GetYears = Database -> TeamId -> Year list
type GetTable = Database -> TeamId -> Year -> Table option

