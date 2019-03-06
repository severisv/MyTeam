module Client.GamePlan

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.React
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Shared
open Client.Components
open Shared.Components
open Shared.Domain
open Shared.Components.Base
open Shared.Components.Layout
open Shared.Features.Games.GamePlan
open Shared.Domain.Members
open Thoth.Json
open Client.Util

type Time = int
type LineupId = System.Guid
type Lineups = List<LineupId * Time * List<string option>>

type State =
    { Lineups : Lineups
      FocusedPlayer : (LineupId * int) option 
      ErrorMessage: string option
      Players: Member list
    }

let matchLength = 90

let getPlayerIndex row col =
        match (row, col) with
        | (0, 0) -> Some 0
        | (0, 2) -> Some 1
        | (0, 4) -> Some 2
        | (2, 1) -> Some 3
        | (2, 3) -> Some 4
        | (3, 2) -> Some 5
        | (4, 0) -> Some 6
        | (4, 1) -> Some 7
        | (4, 3) -> Some 8
        | (4, 4) -> Some 9
        | (5, 2) -> Some 10
        | _ -> None


let save setState gameId (lineups: Lineups) = 
    let url = sprintf "/api/games/%O/gameplan" gameId
    promise { 
        let! res = postRecord url lineups []
        if not res.Ok then failwithf "Received %O from server: %O" res.Status res.StatusText
        setState(fun state props ->
                   { state with 
                      ErrorMessage = None
                   }
            ) 
    }
    |> Promise.catch(fun e -> 
           Browser.console.error <| sprintf "%O" e
           setState(fun state props ->
                   { state with 
                      ErrorMessage = Some "Tilkoblingsproblemer, endringene blir ikke lagret"
                   }
            ) 
    )
    |> Promise.start

let getName (players: Members.Member list)  (player: Members.Member) =
            let rec getName numberOfLettersFromLastName =
                let hashName (p: Members.Member) =
                    match numberOfLettersFromLastName with
                    | 0 -> p.FirstName
                    | _ -> sprintf "%s %s" p.FirstName p.LastName.[0 .. numberOfLettersFromLastName - 1]

                let name = hashName player
                match (players |> List.filter (fun p -> hashName p = name)).Length with
                | i when i < 2 -> name
                | _ when numberOfLettersFromLastName >= player.LastName.Length -> name
                | _ -> getName <| numberOfLettersFromLastName + 1     

            getName 0

let updateLineup setState save lineupId fn =
    setState(fun state props ->
                   let lineups = state.Lineups 
                                |> List.map(fun (id, time, lineup) -> 
                                            if id = lineupId then fn (id, time, lineup)
                                            else (id, time, lineup)
                                           )
                   save lineups
                   { state with 
                      Lineups = lineups
                   }
    )                 
        
let handleTimeChange setState save lineupId value =
        updateLineup
            setState 
            save
            lineupId
            (fun (id, time, lineup) -> (id, value, lineup))    

let handlePlayerChange setState save lineupId index value =
            updateLineup
                setState
                save
                lineupId
                (fun (id, time, lineup) -> 
                                    (id,
                                     time, 
                                     lineup 
                                     |> List.mapi (fun i playerName -> 
                                                        if i = index then value 
                                                        else playerName)
                                    ))

let duplicateLineup setState save lineupId =
    setState(fun state props ->
               let (_, time, lineup) = state.Lineups 
                                       |> List.find (fun (id, _, _) -> id = lineupId)
               let lineups = state.Lineups @ [ (System.Guid.NewGuid(), time, lineup) ]
                                |> List.sortBy (fun (_, time, _) -> time)
               save lineups                               
               { state with 
                  Lineups = lineups
                  FocusedPlayer = None
               }
    )

let removeLineup setState save lineupId =
    setState(fun state props ->        
               let lineups = state.Lineups 
                            |> List.filter (fun (id, _, __) -> id <> lineupId)    
               save lineups                        
               { state with 
                  Lineups = lineups
                  FocusedPlayer = None
               }
    )

let handleFocus (e: FocusEvent) = 
        let target = e.target :?> Browser.HTMLInputElement
        target.select()

type GamePlan(props) =
    inherit Component<Model, State>(props)
    
    do 
        base.setInitState((props.GamePlan
                           |> Option.bind(fun g -> 
                                  Decode.Auto.fromString<Lineups>(g)
                                  |> function 
                                  | Ok s -> Some s
                                  | Error e -> failwithf "%O" e )
                           |> Option.defaultValue [ (System.Guid.NewGuid(), 0, [ 0 .. 10 ] |> List.map(fun _ -> None)) ]
                           |> fun lineups ->
                                { Lineups = lineups
                                  FocusedPlayer = None
                                  ErrorMessage = None
                                  Players = props.Players
                                            |> List.map (fun p -> { p with FirstName = getName props.Players p })
                              }))
    
    override this.render() =         
 
        let model = props
        let state = this.state                                       
        
        let handlePlayerFocus focusedPlayer = 
            this.setState(fun state _ -> { state with FocusedPlayer = focusedPlayer })
            
        let sortLineup _ = 
            this.setState(fun state _ -> { state with Lineups = state.Lineups |> List.sortBy (fun (_, time, _) -> time) })        

        let subs lineup = 
            state.Players |> List.filter(fun p -> lineup |> List.exists (fun l -> l = (Some <| p.FirstName)) |> not)  

        let save = save this.setState model.GameId  
        let handlePlayerChange = handlePlayerChange this.setState save
        let handleTimeChange = handleTimeChange this.setState save
        let removeLineup = removeLineup this.setState save
        let duplicateLineup = duplicateLineup this.setState save
                      
        let square (lineup: List<string option>) lineupId row col =
            getPlayerIndex row col
            |> function
            | Some playerIndex ->
                let subs = subs lineup
                let name = lineup.[playerIndex]
                let getPlayer name = state.Players |> List.tryFind (fun p -> p.FirstName = name)
                
                let playerImage (name: string) =
                    img [ 
                      Class "gameplan-playerImage" 
                      Src (Image.getMember model.ImageOptions (fun o -> { o with Height = Some 40; Width = Some 40})
                            |> fun img -> 
                                match getPlayer name with 
                                | Some p -> img p.Image p.FacebookId  
                                | None -> img "" "")
                        ] 
                fragment []
                    [                       
                        (match name with 
                          | Some n -> playerImage n
                          | None -> div [Class "gameplan-playerImage"] []
                        )                         
                        input [ 
                           Type "text"
                           Class (if state.FocusedPlayer = Some (lineupId, playerIndex) then "focused" else "")
                           Value (name |> Option.defaultValue "")
                           Placeholder "Ingen"
                           OnChange(fun e -> handlePlayerChange lineupId playerIndex (Strings.asOption e.Value))
                           OnFocus (fun e -> handleFocus e; handlePlayerFocus <| Some (lineupId, playerIndex))
                        ]
                        ul [ Class (if state.FocusedPlayer = Some (lineupId, playerIndex) then "visible" else "") ] 
                            (subs |> List.map (fun sub -> 
                                                let name = sub.FirstName
                                                li [] [
                                                    button [ OnClick (fun e ->
                                                                        e.preventDefault()
                                                                        handlePlayerChange lineupId playerIndex (Some <| sub.FirstName)
                                                                        handlePlayerFocus None) ] [
                                                        playerImage name
                                                        str name
                                                    ]
                                                ]
                                                )
                                            )                        
                ]    
            | None -> empty                             

        fragment [] [
        
            mtMain [ Class "gameplan"] [ 
                block [] ([ 
                    (state.ErrorMessage
                     |> function
                     | Some message -> Alerts.danger message
                     | None -> empty
                    )
                    h2 [] [
                        str <| sprintf "%s vs %s" model.Team model.Opponent
                    ]
                    br []
                    br []
                    div [] (state.Lineups
                     |> List.map(fun (lineupId, time, lineup) ->
                            div [Key <| string lineupId]
                                [ 
                                  div [ Class "text-center" ] [   
                                        input [ Type "text"
                                                Class "gp-time"
                                                Placeholder "tid"
                                                Value time
                                                OnChange (fun e -> handleTimeChange lineupId <| (Number.tryParse e.Value |> Option.defaultValue 0)) 
                                                OnFocus handleFocus
                                                OnBlur sortLineup ]
                                        str "min" 
                                    ]
                                  div [Class "clearfix"] [  
                                      button [ 
                                          Class "pull-right hidden-print" 
                                          Disabled (state.Lineups.Length < 2)
                                          OnClick (fun _ -> removeLineup lineupId)
                                          ]
                                        [ Icons.delete ]
                                      button [ 
                                          Class "pull-right hidden-print" 
                                          OnClick (fun _ -> duplicateLineup lineupId)
                                          ]
                                        [ Icons.add "" ]
                                  ]
                                  div []
                                      (                                        
                                        state.Lineups 
                                        |> List.findIndex (fun (id, _m, __) -> id = lineupId)
                                        |> fun i -> if i > 0 then Some state.Lineups.[i-1] else None
                                        |> function
                                        | Some (_, __, prevLineup) -> 
                                            let tryGet (list: string list) i = if list.Length > i then list.[i] else ""
                                            let ins = lineup |> List.except prevLineup |> List.choose id
                                            let outs = prevLineup |> List.except lineup |> List.choose id

                                            [0 .. (max ins.Length outs.Length) - 1] 
                                            |> List.map (fun i -> 
                                                div [ Class "text-center gp-subs" ] [ 
                                                  span [ Class "gameplan-sub-in" ] [ str <| tryGet ins i] 
                                                  str " -> " 
                                                  span [ Class "gameplan-sub-out" ] [ str <| tryGet outs i]
                                                ]
                                            )                                            
                                          
                                        | None -> [empty]
                                      )                                    
                                        
                                  br []                       
                                  div [ Class "gameplan-field" ]
                                    ([0 .. 5] |> List.map (fun row -> 
                                                                div [] 
                                                                    ([0 .. 4] 
                                                                    |> List.map (fun col -> div [Class "gp-square"] [ square lineup lineupId row col]) ) 
                                                           ) ) 
                                  hr []                                 
                            ]
                        )
                    )
                    div [ Class "text-center" ] [
                        br []
                        SubmitButton.render 
                            (fun o -> { o with 
                                          IsSubmitted = model.GamePlanIsPublished
                                          Text = "Publiser"
                                          SubmittedText = "Publisert"
                                          Endpoint = SubmitButton.Post (sprintf "/api/games/%O/gameplan/publish" model.GameId, None) })
                    ]                   
                ]
                )                                  
            ]
            sidebar [] [
                    block [] [
                        h4 [] [str "Spilletid"]
                        div []
                            (
                            let lineups = state.Lineups
                                          |> List.sortBy (fun (_, time, __) -> time)

                            lineups                                      
                             |> List.map (fun (lineupId, time, lineup) -> 
                                            lineup 
                                            |> List.choose id 
                                            |> List.map(fun p -> 
                                                let lineupIndex = lineups |> List.findIndex(fun (lid, _, _) -> lid = lineupId) 
                                                let nextTime = if lineupIndex + 1 < lineups.Length then 
                                                                    let (_, t, __) = lineups.[lineupIndex+1]
                                                                    t
                                                               else
                                                                    matchLength

                                                (p, nextTime - time)
                                            ) 
                                        )
                             |> List.reduce List.append
                             |> List.groupBy (fun (player, _) -> player)
                             |> List.map (fun (key, values) -> (key, values |> List.sumBy(fun (_, time) -> time)) )
                             |> List.sortBy (fun (player, _) -> player)                             
                             |> List.map (fun (player, time) -> 
                                div [] [
                                    str player
                                    str ": "  
                                    b [] [ str <| string time ]
                                ]
                                )            
                            )            
                        ]
            ]
        ]

let element = ofType<GamePlan, _, _>
ReactHelpers.render Decode.Auto.fromString<Model> clientView element 