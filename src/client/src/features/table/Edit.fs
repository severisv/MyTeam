module Client.Table.Edit

open Fable.React
open Fable.React.Props
open Fable.Import
open Client.Components
open Shared.Components
open Shared.Components.Base
open Shared.Components.Forms
open Thoth.Json
open Shared.Util

type EditModel = {
    Title: string
    Team: string
    Year: int
    AutoUpdateTable: bool
    SourceUrl: string
    AutoUpdateFixtures: bool
    FixtureSourceUrl: string
}

let editView = "edit-table"
let modelAttribute = "model"

type State =
    { Title : string
      AutoUpdateTable : bool
      TableSourceUrl : string 
      AutoUpdateFixtures : bool
      FixtureSourceUrl : string }

type EditTable(props) =
    inherit Component<EditModel, State>(props)
    do base.setInitState ({ Title = props.Title
                            AutoUpdateTable = props.AutoUpdateTable
                            TableSourceUrl = props.SourceUrl
                            FixtureSourceUrl = props.FixtureSourceUrl
                            AutoUpdateFixtures = props.AutoUpdateFixtures })
    override this.render() =
        let props = this.props
        let state = this.state
        let update fn = this.setState (fun state _ -> fn state)

        let formRow = formRow [Horizontal 4]
      
        EditBlock.render 
          { Render =
            fun isInEditMode -> 
                div [] [
                    isInEditMode &?  
                        fragment [] [
                            Modal.render 
                                { OpenButton = fun handleOpen -> btn [ Danger; OnClick handleOpen; Class "pull-right" ] [ Icons.delete ]
                                  Content = 
                                    fun handleClose ->
                                        div [] [ 
                                          h4 [] [str <| sprintf "Er du sikker på at du vil slette tabellen for %s %i?" props.Team props.Year] 
                                          div [Class "text-center"] [
                                              br []
                                              Send.sendElement 
                                                (fun o -> 
                                                { o with
                                                    SendElement = btn, [Lg;Danger], [str "Slett"]
                                                    SentElement = btn, [Lg], [str "Slettet"]            
                                                    Endpoint = Send.Delete <| sprintf "/api/tables/%s/%i" props.Team props.Year
                                                    OnSubmit = Some !> Browser.Dom.window.location.reload })
                                              btn [Lg; OnClick !> handleClose ] [str "Nei"]
                                          ]
                                      ]
                                }
                            form [ Class "table-edit"; Horizontal 4] [
                                formRow [str "Divisjon"] 
                                        [Textinput.render { Value = state.Title
                                                            Url = sprintf "/api/tables/%s/%i/title" props.Team props.Year
                                                            OnChange = fun value -> update (fun state -> { state with Title = value }) }]                                                              
    
                                formRow [str "Oppdater tabell automatisk"] 
                                        [Checkbox.render { Value = props.AutoUpdateTable
                                                           Url = sprintf "/api/tables/%s/%i/autoupdate" props.Team props.Year
                                                           OnChange = fun value -> update (fun state -> { state with AutoUpdateTable = value }) }]                                                    
                                                                                                                                   
                                state.AutoUpdateTable &?
                                    formRow [str "Url til tabell på fotball.no"] 
                                            [Textinput.render { Value = state.TableSourceUrl
                                                                Url = sprintf "/api/tables/%s/%i/sourceurl" props.Team props.Year
                                                                OnChange = fun value -> update (fun state -> { state with TableSourceUrl = value }) }]
                                                              
                                formRow [str "Oppdater kamper automatisk"]
                                        [Checkbox.render { Value = props.AutoUpdateFixtures
                                                           Url = sprintf "/api/tables/%s/%i/autoupdatefixtures" props.Team props.Year
                                                           OnChange = fun value -> update (fun state -> { state with AutoUpdateFixtures = value }) }]                                                        
                                                                                                                                   
                                state.AutoUpdateFixtures &?
                                    formRow [str "Url til kamper på fotball.no"]
                                            [Textinput.render { Value = state.FixtureSourceUrl
                                                                Url = sprintf "/api/tables/%s/%i/fixturesourceurl" props.Team props.Year
                                                                OnChange = fun value -> update (fun state -> { state with FixtureSourceUrl = value }) }]                                                                                                                
                                br []
                            ]
                        ]
                    h2 [] [Icons.trophy ""
                           whitespace
                           str state.Title] 
                ]
          } 
            
            
let element = ofType<EditTable, _, _>
ReactHelpers.render Decode.Auto.fromString<EditModel> editView element
            