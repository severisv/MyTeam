module MyTeam.Client.Table.Edit

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.Browser
open Fable.Import.React
open MyTeam
open MyTeam.Client.Components
open Shared.Components
open Shared.Components.Base
open Shared.Features.Table.Table
open Shared.Components.Forms
open Thoth.Json


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

        fragment [] 
            [ EditBlock.render 
                  { Render =
                        fun isInEditMode -> 
                            div [] [
                                (if isInEditMode then    
                                    fragment [] [
                                        (Modal.render 
                                                    { OpenButton = fun handleOpen -> btn [ Danger; OnClick handleOpen; Class "pull-right" ] [ Icons.delete ]
                                                      Content = 
                                                        fun handleClose ->
                                                            div [] [ 
                                                              h4 [] [str <| sprintf "Er du sikker på at du vil slette tabellen for %s %i?" props.Team props.Year] 
                                                              div [ Class "text-center"] [
                                                                  br []
                                                                  SubmitButton.render 
                                                                    (fun o -> 
                                                                    { o with
                                                                        IsSubmitted = false
                                                                        IsDisabled = false
                                                                        Size = Lg
                                                                        Text = "Ja" 
                                                                        SubmittedText = "Slettet"
                                                                        Endpoint = SubmitButton.Delete <| sprintf "/api/tables/%s/%i" props.Team props.Year
                                                                        OnSubmit = Some Browser.location.reload })
                                                                  btn [ Lg; OnClick handleClose ] [ str "Nei" ]
                                                              ]
                                                          ]
                                                    })
                                        form [ Class "table-edit"; Horizontal 4] [
                                            formRow [str "Divisjon"] 
                                                    [Textinput.render { Value = state.Title
                                                                        Url = sprintf "/api/tables/%s/%i/title" props.Team props.Year
                                                                        OnChange = fun value -> update (fun state -> { state with Title = value }) }                                                 
                                                    ]                                                              

                                            formRow [str "Oppdater tabell automatisk"] 
                                                    [Checkbox.render { Value = props.AutoUpdateTable
                                                                       Url = sprintf "/api/tables/%s/%i/autoupdate" props.Team props.Year
                                                                       OnChange = fun value -> update (fun state -> { state with AutoUpdateTable = value }) }]                                                    
                                                    
                                                                                                       
                                            (if state.AutoUpdateTable then
                                                formRow [str "Url til tabell på fotball.no"] 
                                                        [Textinput.render { Value = state.TableSourceUrl
                                                                            Url = sprintf "/api/tables/%s/%i/sourceurl" props.Team props.Year
                                                                            OnChange = fun value -> update (fun state -> { state with TableSourceUrl = value }) }]
                                                                                                                    
                                             else empty)

                                            formRow [str "Oppdater kamper automatisk"]
                                                    [Checkbox.render { Value = props.AutoUpdateFixtures
                                                                       Url = sprintf "/api/tables/%s/%i/autoupdatefixtures" props.Team props.Year
                                                                       OnChange = fun value -> update (fun state -> { state with AutoUpdateFixtures = value }) }]                                                        
                                                    
                                                                                                       
                                            (if state.AutoUpdateFixtures then
                                                formRow [str "Url til kamper på fotball.no"]
                                                        [Textinput.render { Value = state.FixtureSourceUrl
                                                                            Url = sprintf "/api/tables/%s/%i/fixturesourceurl" props.Team props.Year
                                                                            OnChange = fun value -> update (fun state -> { state with FixtureSourceUrl = value }) }]                                                                                                                
                                             else empty)                            

                                            br []
                                        ]
                                    ]
                                     else empty)
                                h2 [] [ Icons.trophy ""
                                        whitespace
                                        str state.Title ] 
                            ]
                  } 
            ]

let element model = ofType<EditTable, _, _> model []
let node = document.getElementById editView

if not <| isNull node then 
    node.getAttribute (Interop.modelAttributeName)
    |> Decode.Auto.fromString<EditModel>
    |> function 
    | Ok model -> ReactDom.render (element model, node)
    | Error e -> failwithf "Json deserialization failed: %O" e
