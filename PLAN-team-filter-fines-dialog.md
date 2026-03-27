# Plan: Team Filter in "Registrer bot" Dialog

## Context
The "Registrer bøter" (Register fines) dialog in `Add.fs` currently shows all active players in a flat dropdown with no team filtering. The club has multiple teams, and users should be able to filter which players appear in the "Hvem" (Who) selector by team. The default team should be the user's most significant team (first in their `TeamIds` list, which is populated from the DB).

## Files to Modify
- `src/client/src/features/fines/Add.fs` — main dialog component
- `src/client/src/features/fines/FineList.fs` — two call sites of `addFine`

No server changes needed — `/api/teams` already exists and `MemberWithTeamsAndRoles` already includes each player's `Teams: TeamId list`.

## Implementation Steps

### 1. Update `AddFineState` in `Add.fs`
Add two new fields:
```fsharp
type AddFineState = {
    Players: MemberWithTeamsAndRoles list
    Teams: Team list                      // NEW
    SelectedTeamId: TeamId option         // NEW
    Rates: RemedyRate list
    Form: AddFineForm
    Error: string option
    AddedFines: AddFine list
}
```

### 2. Add `user: User` parameter to `addFine`
Change signature from:
```fsharp
let addFine openLink onAdd onDelete =
```
To:
```fsharp
let addFine user openLink onAdd onDelete =
```

### 3. Initialize state with user's first team as default
In the komponent initial state block, set:
```fsharp
SelectedTeamId = user.TeamIds |> List.tryHead
Teams = []
```

### 4. Load teams in `ComponentDidMount`
Add a third HTTP call alongside the existing players/rates fetches:
```fsharp
Http.get "/api/teams" Decode.Auto.fromString<Team list>
    { OnSuccess = fun result ->
        setState (fun state props -> { state with Teams = result })
      OnError = fun _ ->
        setState (fun state props ->
            { state with Error = Some "Noe gikk galt ved lasting av lag." }) }
```

### 5. Add team filter dropdown in the form (above "Hvem")
```fsharp
formRow [Horizontal 3]
    [str "Lag"]
    [selectInput
        [OnChange (fun e ->
            let id = Guid.Parse e.Value
            setState (fun state props ->
                let filtered = state.Players |> List.filter (fun p -> p.Teams |> List.contains id)
                { state with
                    SelectedTeamId = Some id
                    Form = { state.Form with MemberId = filtered |> List.map (fun p -> p.Details.Id) |> List.tryHead } }))]
        (state.Teams |> List.map (fun t -> { Name = t.Name; Value = t.Id }))]
```

You'll also need to pre-select the correct option — the `selectInput` component may need a `Selected` prop or the list should be reordered so the user's team appears first. Check `Shared.Components.Forms.selectInput` for how to set a default value.

### 6. Compute filtered players before the "Hvem" row
```fsharp
let filteredPlayers =
    match state.SelectedTeamId with
    | Some teamId -> state.Players |> List.filter (fun p -> p.Teams |> List.contains teamId)
    | None -> state.Players
```

Then in the "Hvem" `selectInput`, replace `state.Players` with `filteredPlayers`:
```fsharp
(filteredPlayers |> List.map (fun p -> { Name = p.Details.Name; Value = p.Details.Id }))
```

### 7. Check imports
`Team` and `TeamId` come from `Club.fs` (domain). `open Shared.Domain` at the top of `Add.fs` should already cover this — verify that `Team` resolves, otherwise add `open Shared.Domain.Club` explicitly.

### 8. Update call sites in `FineList.fs`
Both calls to `Add.addFine` need `props.User` added as first argument:

Line ~87 (mobile button):
```fsharp
Add.addFine props.User
    (fun handleOpen -> btn [...] [...])
    (handleAdded year selectedMember setState)
    (handleDeleted setState)
```

Line ~224 (sidebar):
```fsharp
Add.addFine props.User
    (fun handleOpen -> linkButton [...] [...])
    (handleAdded year selectedMember setState)
    (handleDeleted setState)
```

## Verification
1. Open the fines page as a Botsjef user
2. Click "Ny bot" — dialog should show a "Lag" dropdown pre-selected to the user's primary team
3. The "Hvem" dropdown should only show players from that team
4. Changing "Lag" should update the "Hvem" player list and reset selection to first player of new team
5. Registering a fine should still work as before
