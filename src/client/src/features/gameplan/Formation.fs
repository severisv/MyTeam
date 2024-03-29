module Client.GamePlan.Formation

type ElleverFormation =
    | FourFourTwo
    | FourThreeThree
    | FiveThreeTwo
    override this.ToString() =
        match this with
        | FourFourTwo -> "4-4-2"
        | FourThreeThree -> "4-3-3"
        | FiveThreeTwo -> "5-3-2"

type SjuerFormation =
    | ThreeTwoOne
    | TwoThreeOne
    override this.ToString() =
        match this with
        | ThreeTwoOne -> "3-2-1"
        | TwoThreeOne -> "2-3-1"

type Formations =
    | Sjuer of SjuerFormation
    | Ellever of ElleverFormation
    override this.ToString() =
        match this with
        | Sjuer v -> string v
        | Ellever v -> string v


let getPlayerIndex formation row col =
    match formation with
    | Ellever formation ->
        match formation with
        | FourFourTwo ->
            match (row, col) with
            | (0, 1) -> Some 0
            | (0, 3) -> Some 1
            | (2, 0) -> Some 2
            | (2, 1) -> Some 3
            | (2, 3) -> Some 4
            | (2, 4) -> Some 5
            | (5, 0) -> Some 6
            | (5, 1) -> Some 7
            | (5, 3) -> Some 8
            | (5, 4) -> Some 9
            | (7, 2) -> Some 10
            | _ -> None
        | FourThreeThree ->
            match (row, col) with
            | (0, 0) -> Some 0
            | (0, 2) -> Some 1
            | (0, 4) -> Some 2
            | (2, 1) -> Some 3
            | (2, 3) -> Some 4
            | (3, 2) -> Some 5
            | (5, 0) -> Some 6
            | (5, 1) -> Some 7
            | (5, 3) -> Some 8
            | (5, 4) -> Some 9
            | (7, 2) -> Some 10
            | _ -> None
        | FiveThreeTwo ->
            match (row, col) with
            | (0, 1) -> Some 0
            | (0, 3) -> Some 1
            | (3, 1) -> Some 3
            | (3, 3) -> Some 4
            | (3, 2) -> Some 5
            | (4, 0) -> Some 2
            | (4, 4) -> Some 6
            | (5, 1) -> Some 7
            | (5, 2) -> Some 8
            | (5, 3) -> Some 9
            | (7, 2) -> Some 10
            | _ -> None
    | Sjuer formation ->
        match formation with
        | ThreeTwoOne ->
            match (row, col) with
            | (0, 2) -> Some 0
            | (2, 1) -> Some 1
            | (2, 3) -> Some 2
            | (4, 0) -> Some 3
            | (4, 2) -> Some 4
            | (4, 4) -> Some 5
            | (5, 2) -> Some 6
            | _ -> None
        | TwoThreeOne ->
            match (row, col) with
            | (0, 2) -> Some 0
            | (2, 0) -> Some 1
            | (2, 2) -> Some 2
            | (2, 4) -> Some 3
            | (4, 1) -> Some 4
            | (4, 3) -> Some 5
            | (5, 2) -> Some 6
            | _ -> None
