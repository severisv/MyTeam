module Client.GamePlan.View

open Browser.Types
open Fable.React
open Fable.React.Props
open Fable.Import
open Fetch.Types
open Shared
open Client.Components
open Shared.Components
open Shared.Domain
open Shared.Components.Base
open Shared.Components.Layout
open Shared.Domain.Members
open Shared.Components.Input
open Thoth.Json
open Shared.Util
open Client.GamePlan.Formation
open Client.Util
open System
open Shared.Image
open Fable.React.Props



type Model = {
    GameId: Guid
    Team: string
    Opponent: string
    GamePlanIsPublished: bool
    GamePlan: string option
    Players: Member list
    ImageOptions: CloudinaryOptions
    Formation: Formations
}

let clientView = "gameplan"
let modelAttribute = "model"


type Time = int
type LineupId = System.Guid

type Lineup = {
    Id: LineupId
    Time: Time
    Players: List<string option>
}

type GamePlanState = {
    Lineups: Lineup list
    Formation: Formations
    MatchLength: int
}
       
type State =
    { GamePlan : GamePlanState
      FocusedPlayer : (LineupId * int) option 
      ErrorMessage: string option
      Players: Member list }

let save setState gameId (lineups: GamePlanState) = 
    let url = sprintf "/api/games/%O/gameplan" gameId
    promise { 
        let! res = Http.sendRecord HttpMethod.POST url lineups []
        if not res.Ok then failwithf "Received %O from server: %O" res.Status res.StatusText
        setState(fun state props ->
                   { state with 
                      ErrorMessage = None
                   }) }
    |> Promise.catch(fun e -> 
           Browser.Dom.console.error(sprintf "%O" e)
           setState(fun state props ->
                   { state with 
                      ErrorMessage = Some "Tilkoblingsproblemer, endringene blir ikke lagret"
                   }))
    |> Promise.start

let getName (players: Members.Member list) (player: Members.Member) =
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
                   let gamePlan = { state.GamePlan with
                                        Lineups = state.GamePlan.Lineups
                                                    |> List.map(fun line -> 
                                                                if line.Id = lineupId then fn line
                                                                else line) }
                   save gamePlan
                   { state with GamePlan = gamePlan }
    )                 
        
let handleTimeChange setState save lineupId value =
        updateLineup
            setState 
            save
            lineupId
            (fun line -> { line with Time = value })   

let handlePlayerChange setState save lineupId index value =
            updateLineup
                setState
                save
                lineupId
                (fun line -> 
                        { line with
                                Players = line.Players
                                         |> List.mapi (fun i playerName -> 
                                            if i = index then value 
                                            else playerName)
                        })

let duplicateLineup setState save lineupId =
    setState(fun state props ->
               let line = state.GamePlan.Lineups |> List.find (fun line -> line.Id = lineupId)
               let gamePlan = { state.GamePlan with
                                    Lineups = state.GamePlan.Lineups @ [  { Id = System.Guid.NewGuid()
                                                                            Time = line.Time
                                                                            Players = line.Players } ]
                                              |> List.sortBy (fun line -> line.Time)}                                                          
                                
               save gamePlan                               
               { state with
                  GamePlan = gamePlan
                  FocusedPlayer = None })

let removeLineup setState save lineupId =
    setState(fun state props ->        
               let gamePlan =
                    { state.GamePlan with
                        Lineups = state.GamePlan.Lineups 
                            |> List.filter (fun line -> line.Id <> lineupId)}                
               save gamePlan                        
               { state with 
                  GamePlan = gamePlan
                  FocusedPlayer = None
               })
    
let setFormation setState save formation =
    setState(fun state props ->        
               let gamePlan =
                    { state.GamePlan with
                        Formation = formation }        
               save gamePlan                        
               { state with 
                  GamePlan = gamePlan
               })  
 
let handleFocus (e: FocusEvent) = 
        let target = e.target :?> HTMLInputElement
        target.select()

type GamePlan(props) =
    inherit Component<Model, State>(props)
    
    do 
        base.setInitState((props.GamePlan
                           |> Option.bind(fun g -> 
                                  Decode.Auto.fromString<GamePlanState>(g)
                                  |> function 
                                  | Ok s -> Some s
                                  | Error e -> failwithf "%O" e )
                           |> Option.defaultValue {
                               Lineups = [{ Id = System.Guid.NewGuid(); Time = 0; Players = [ 0 .. 10 ] |> List.map(fun _ -> None)}]
                               Formation = props.Formation
                               MatchLength = match props.Formation with
                                             | Sjuer _ -> 60
                                             | Ellever _ -> 90
                           }
                           |> fun gamePlan ->
                                { GamePlan = gamePlan
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
            this.setState(fun state _ -> { state with GamePlan = { state.GamePlan with Lineups = state.GamePlan.Lineups |> List.sortBy (fun line-> line.Time) }})        

        let subs lineup = 
            state.Players |> List.filter(fun p -> lineup |> List.exists (fun l -> l = (Some <| p.FirstName)) |> not)  

        let save = save this.setState model.GameId  
        let handlePlayerChange = handlePlayerChange this.setState save
        let handleTimeChange = handleTimeChange this.setState save
        let removeLineup = removeLineup this.setState save
        let setFormation = setFormation this.setState save
        let duplicateLineup = duplicateLineup this.setState save
                      
        let square (lineup: List<string option>) lineupId row col =
            getPlayerIndex state.GamePlan.Formation row col
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
                           OnClick(fun e -> e.stopPropagation())
                           OnChange(fun e -> handlePlayerChange lineupId playerIndex (Strings.asOption e.Value))
                           OnFocus (fun e -> handlePlayerFocus <| Some (lineupId, playerIndex))
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
        
            mtMain [ Class "gameplan"; OnClick (fun _ -> this.setState(fun state props -> { state with FocusedPlayer = None  }))] [ 
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
                    div [Style [TextAlign TextAlignOptions.Right]] [
                        radio setFormation
                            ((match props.Formation with
                             | Sjuer _ -> [ThreeTwoOne; TwoThreeOne] |> List.map Sjuer
                             | Ellever _ -> [FourFourTwo; FourThreeThree] |> List.map Ellever                                
                            ) |> List.map (fun v -> { Label = string v; Value = v }))
                            (Some state.GamePlan.Formation)
                    ]
                    div [] (state.GamePlan.Lineups
                     |> List.map(fun lineup ->
                            div [Key <| string lineup.Id]
                                [ 
                                  div [ Class "text-center" ] [   
                                        input [ Type "text"
                                                Class "gp-time"
                                                Placeholder "tid"
                                                Value lineup.Time
                                                OnChange (fun e -> handleTimeChange lineup.Id <| (Number.tryParse e.Value |> Option.defaultValue 0)) 
                                                OnFocus handleFocus
                                                OnBlur sortLineup ]
                                        str "min" 
                                    ]
                                  div [Class "clearfix"] [  
                                      button [ 
                                          Class "pull-right hidden-print" 
                                          Disabled (state.GamePlan.Lineups.Length < 2)
                                          OnClick (fun _ -> removeLineup lineup.Id)
                                          ]
                                        [ Icons.delete ]
                                      button [ 
                                          Class "pull-right hidden-print" 
                                          OnClick (fun _ -> duplicateLineup lineup.Id)
                                          ]
                                        [ Icons.add "" ]
                                  ]
                                  div []
                                      (                                        
                                        state.GamePlan.Lineups 
                                        |> List.findIndex (fun line -> line.Id = lineup.Id)
                                        |> fun i -> if i > 0 then Some state.GamePlan.Lineups.[i-1] else None
                                        |> function
                                        | Some prevLineup -> 
                                            let tryGet (list: string list) i = if list.Length > i then list.[i] else ""
                                            let ins = lineup.Players |> List.except prevLineup.Players |> List.choose id
                                            let outs = prevLineup.Players |> List.except lineup.Players |> List.choose id

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
                                                                    |> List.map (fun col -> div [Class "gp-square"] [ square lineup.Players lineup.Id row col]) ) 
                                                           ) ) 
                                  hr []                                 
                            ]
                        )
                    )
                    div [ Class "text-center" ] [
                        br []
                        Send.sendElement 
                            (fun o -> { o with 
                                          IsSent = model.GamePlanIsPublished
                                          SendElement = btn, [Lg;Primary], [str "Publiser"]
                                          SentElement = btn, [Lg;Success], [str "Publisert"]                         
                                          Endpoint = Send.Post (sprintf "/api/games/%O/gameplan/publish" model.GameId, None) })
                    ]                   
                ]
                )                                  
            ]
            sidebar [] [
                    block [] [
                        h4 [] [str "Spilletid"]
                        div []
                            (
                            let lineups = state.GamePlan.Lineups
                                          |> List.sortBy (fun line -> line.Time)

                            lineups                                      
                             |> List.map (fun line -> 
                                            line.Players 
                                            |> List.choose id 
                                            |> List.map(fun p -> 
                                                let lineupIndex = lineups |> List.findIndex(fun l -> l.Id = line.Id) 
                                                let nextTime = if lineupIndex + 1 < lineups.Length then 
                                                                    let t = lineups.[lineupIndex+1].Time
                                                                    t
                                                               else
                                                                    state.GamePlan.MatchLength
                                                (p, nextTime - line.Time)) 
                                        )
                             |> List.reduce List.append
                             |> List.append (state.Players |> List.map(fun p -> (p.FirstName, 0)))
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