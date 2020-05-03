module Client.Features.Games.Add


open Client.Components
open Client.Util
open Fable.Import
open Fable.React
open Fable.React
open Fable.React.Props
open Fable.React.Props
open Shared
open Shared
open Shared.Components
open Shared.Components.Base
open Shared.Components.Base
open Shared.Components.Datepicker
open Shared.Components.Forms
open Shared.Components.Links
open Shared.Components.Tables
open Shared.Domain
open Shared.Domain.Members
open Shared.Features.Fines.Common
open Shared.Util
open Shared.Util.ReactHelpers
open Shared.Util.ReactHelpers
open Shared.Validation
open System
open Thoth.Json
open Microsoft.FSharp.Reflection


type State =
    { Opponent : string
      Date : DateTime option
      Time : string
      Location : string
      Team : Guid
      GameType : GameType
      Description : string
      IsHomeGame: bool   
     }



type Props = {
    Teams: Team list
    GameTypes: GameType list
}

[<CLIMutable>]
type AddGame = {
      Id : Guid option       
      Opponent : string
      Date : DateTime
      Time : TimeSpan
      Location : string
      Team : Guid
      GameType : GameType
      Description : string
      IsHomeGame: bool   
}


 

let element =
    FunctionComponent.Of(fun (props: Props) ->
        let state =
            Hooks.useState<State>
                           { Date = None
                             Opponent = ""                              
                             Time = ""
                             Location = ""
                             Team = props.Teams |> List.map(fun t -> t.Id) |> List.head
                             GameType = props.GameTypes |> List.head
                             Description = ""
                             IsHomeGame = true  }

        let errorState =
            Hooks.useState<string option> None            

        let setFormValue (v: State -> State) = state.update v 
        let state = state.current       
        

        let validation =
                Map [
                    "Date", isSome "Dato" state.Date
                    "Opponent", isRequired "Motstander" state.Opponent
                    "Time", isRequired "Klokkeslett" state.Time                   
                    "Location", isRequired "Sted" state.Location                    
                ]

        fragment [] [
            h4 [] [ str "Ny kamp" ]
            errorState.current => Alerts.danger
            form [Horizontal 3] [
                div [Class "form-group"] [
                    label [Class "control-label col-sm-3"] [Icons.team "Lag"]                    
                    div [Class "col-sm-6"] [
                        selectInput [OnChange (fun e ->
                                                let id = e.Value
                                                setFormValue (fun form ->
                                                    { form with Team = Guid.Parse id }))]
                            (props.Teams |> List.map (fun p ->
                                { Name = p.Name; Value = p.Id   })) 
                    ]      
                    checkboxInput [Class "col-sm-3"] [str "Hjemme"]
                                         state.IsHomeGame   
                                         (fun value -> setFormValue (fun form ->
                                                        { form with IsHomeGame = value }))       
                ]
                

                formRow [Horizontal 3]
                        [Icons.users "Motstander"]
                        [textInput [
                                Validation [validation.["Opponent"]]
                                OnChange (fun e ->
                                                let value = e.Value
                                                setFormValue (fun form ->
                                                    { form with Opponent = value }))                                                 
                                Placeholder "Mercantile SFK"
                                Value state.Opponent] ]
                        
                formRow [Horizontal 3]
                        [Icons.calendar "Dato"]
                        [dateInput [Validation [validation.["Date"]]
                                    Value state.Date
                                    OnDateChange (fun date ->
                                                        setFormValue (fun form ->
                                                        { form with Date = date }))]]                                    
                formRow [Horizontal 3]
                        [Icons.clock]
                        [textInput [
                                Validation [validation.["Time"]]
                                OnChange (fun e ->
                                                let value = e.Value
                                                setFormValue (fun form ->
                                                    { form with Time = value }))                                                 
                                Placeholder "18:30"
                                Value state.Time] ]
                        
                formRow [Horizontal 3]
                        [Icons.mapMarker "Sted"]
                        [textInput [
                                Validation [validation.["Location"]]
                                OnChange (fun e ->
                                                let value = e.Value
                                                setFormValue (fun form ->
                                                    { form with Location = value }))                                                 
                                Placeholder "Ekeberg 2"
                                Value state.Location] ]

                formRow [Horizontal 3]
                            [Icons.trophy "Turnering"]
                            [selectInput [OnChange (fun e ->
                                                        let s = e.Value
                                                        setFormValue (fun form ->
                                                            { form with GameType = (Enums.fromString<GameType> typedefof<GameType> s) }))]
                            (props.GameTypes |> List.map (fun t -> { Name = string t; Value = t   })) ]


              

                formRow [Horizontal 3]
                    [Icons.description]
                    [textInput [
                            Validation []
                            OnChange (fun e ->
                                            let value = e.Value
                                            setFormValue (fun form ->
                                                { form with Description = value }))                                                 
                            Placeholder "Oppmøte 20 minutter før"
                            Value state.Description] ]                            
                        
              
                
                formRow [Horizontal 3] [] [
                    Send.sendElement
                        (fun o ->
                            { o with
                                  IsDisabled = validation |> Map.toList |>  List.exists (function | (_, Error e) -> true | _ -> false)
                                  SendElement = btn, [ButtonSize.Normal;Primary], [str "Lagre"]
                                  SentElement = btn, [ButtonSize.Normal;Success], [str "Lagret"]
                                  Endpoint = Send.Post (sprintf "/api/games",
                                                                Some (fun () ->
                                                                     Encode.Auto.toString(0,
                                                                         {  Id = None
                                                                            Opponent = state.Opponent
                                                                            Date = state.Date.Value
                                                                            Time = (Date.tryParseTime state.Time).Value
                                                                            Location = state.Location
                                                                            Team = state.Team
                                                                            GameType = state.GameType
                                                                            Description = state.Description
                                                                            IsHomeGame = state.IsHomeGame   })))
                                  OnSubmit = Some (fun res ->
                                                        Decode.Auto.fromString<AddGame> res
                                                        |> function
                                                        | Ok game ->
                                                            printf "%O" game
                                                            ()
                                                            
                                                        | Error e -> errorState.update(Some e ) )})
                ]
           ]
        ]
    )
  
hydrate2 "add-game" Decode.Auto.fromString<Props> element
