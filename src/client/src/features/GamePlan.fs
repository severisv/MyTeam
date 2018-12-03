module MyTeam.Client.GamePlan

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.React
open Fable.PowerPack
open MyTeam
open MyTeam.Client.Components
open MyTeam.Components
open MyTeam.Domain
open MyTeam.Shared.Components
open MyTeam.Shared.Components.Layout
open Shared.Features.Games.GamePlan
open Thoth.Json


type Time = int
type LineupId = System.Guid

type State =
    { Lineup : List<LineupId * Time * List<string option>>
      FocusedPlayer: int option
     }

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

let updateLineup setState lineupId fn =
    setState(fun state props ->
                   { state with 
                      Lineup = state.Lineup 
                                |> List.map(fun (id, time, lineup) -> 
                                            if id = lineupId then fn (id, time, lineup)
                                            else (id, time, lineup)
                                           )
                   }
    )
        


type GamePlan(props) =
    inherit Component<Model, State>(props)
    
    do 
        base.setInitState((props.GamePlan
                           |> Option.bind(fun g -> 
                                  Decode.Auto.fromString<State>(g)
                                  |> function 
                                  | Ok s -> Some s
                                  | Error _ -> None)
                           |> Option.defaultValue { Lineup = [ (System.Guid.NewGuid(), 0, [ None;None;None;None;None;None;None;None;None;None;None ]) ]; FocusedPlayer = None }))
    
    override this.render() = 
        
        let model = props
        let state = this.state
                                        
        let getName = getName model.Players
        

        
        let updateLineup = updateLineup this.setState

        let handleTimeChange lineupId value =
            updateLineup 
                lineupId
                (fun (id, time, lineup) -> (id, value, lineup))

        let handlePlayerChange lineupId index value =
            updateLineup 
                lineupId
                (fun (id, time, lineup) -> 
                                    (id,
                                     time, 
                                     lineup 
                                     |> List.mapi (fun i playerName -> 
                                                        if i = index then value 
                                                        else playerName)
                                    ))
     

        let handleFocus (e: FocusEvent) = 
                let target = e.target :?> Browser.HTMLInputElement
                target.select()
                
        let handlePlayerFocus focusedPlayer = 
            this.setState(fun state _ -> { state with FocusedPlayer = focusedPlayer })            

        let subs lineup = 
            model.Players |> List.filter(fun p -> lineup |> List.exists (fun l -> l = (Some <| getName p)) |> not)  
            
     
        let square (lineup: List<string option>) lineupId row col =
            let subs = subs lineup
            let playerSquare playerIndex =
                let name = lineup.[playerIndex]
                let player = model.Players |> List.tryFind (fun p -> Some p.Name = name)
                fragment []
                    [
                        img [ Class "gameplan-playerImage"
                              Src (match player with 
                                  | Some p -> Image.getMember model.ImageOptions p.Image p.FacebookId (fun o -> { o with Height = Some 40; Width = Some 40}) 
                                  | None -> Image.getMember model.ImageOptions "" "" (fun o -> { o with Height = Some 40; Width = Some 40})
                                  )
                            ]
                        input [ 
                           Type "text"
                           Class (if state.FocusedPlayer = Some playerIndex then "focused" else "")
                           Value (name |> Option.defaultValue "")
                           Placeholder "Ingen"
                           OnChange(fun e -> handlePlayerChange lineupId playerIndex (Strings.asOption e.Value))
                           OnFocus (fun e -> handleFocus e; handlePlayerFocus <| Some playerIndex)
                        ]
                        ul [ Class (if state.FocusedPlayer = Some playerIndex then "visible" else "") ] [ 
                            li [ ] (subs |> List.map (fun sub -> 
                                                let name = getName sub 
                                                button [ OnClick (fun e -> e.preventDefault(); handlePlayerChange lineupId playerIndex (Some <| getName sub); handlePlayerFocus None) ] [
                                                    str name
                                                ]
                                                ))
                        ]
                    ]             

            match (row, col) with
            | (0, 0) -> playerSquare 0
            | (0, 2) -> playerSquare 1
            | (0, 4) -> playerSquare 2
            | (2, 1) -> playerSquare 3
            | (2, 3) -> playerSquare 4
            | (3, 2) -> playerSquare 5
            | (4, 0) -> playerSquare 6
            | (4, 1) -> playerSquare 7
            | (4, 3) -> playerSquare 8
            | (4, 4) -> playerSquare 9
            | (5, 2) -> playerSquare 10
            | _ -> empty    

                                  

        fragment [] [
        
            mtMain [ Class "gameplan"] [ 
                block [] ([ 
                    h2 [] [
                        str <| sprintf "%s vs %s" model.Team model.Opponent
                    ]
                    br []
                    br []
                ] @
                    (state.Lineup
                     |> List.map(fun (lineupId, time, lineup) ->
                     
                            div []
                                [ div [ Class "text-center" ]
                                    [   
                                        input [ 
                                                Type "text"
                                                Class "gp-time"
                                                Placeholder "tid"
                                                Value time
                                                OnChange (fun e -> handleTimeChange lineupId <| (Number.tryParse e.Value |> Option.defaultValue 0)) 
                                                OnFocus handleFocus
                                              ]
                                        str "min" 
                                    ]
                                  button [ Class "pull-right hidden-print" ]
                                    [ i [ Class "fa fa-times" ]
                                        []
                                    ]
                                  button [ Class "pull-right hidden-print" ]
                                    [ i [ Class "fa fa-plus" ]
                                        []
                                    ]
                                  br []                       
                                  div [ Class "gameplan-field" ]
                                    ([0 .. 5] |> List.map (fun row -> 
                                                                div [Class "gp-row"] 
                                                                    ([0 .. 4] 
                                                                    |> List.map (fun col -> div [Class "gp-square"] [ square lineup lineupId row col]) ) 
                                                           ) ) 
                                  hr []
                                ]

                        ))
                )     
                             
            ]
            sidebar [] [
                    block [] [
                        h4 [] [str "Spilletid"]
                        div []
                            [ 
                                str "Aru: "  
                                b [] [ str "45" ] 
                            ]            
                        ]
            ]
        ]

let element model = ofType<GamePlan, _, _> model []
let node = Browser.document.getElementById(clientView)

if not <| isNull node then 
    node.getAttribute(Interop.modelAttributeName)
    |> Decode.Auto.fromString<Model>
    |> function 
    | Ok model -> ReactDom.render(element model, node)
    | Error e -> failwithf "Json deserialization failed: %O" e
