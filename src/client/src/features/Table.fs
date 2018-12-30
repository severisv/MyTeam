module MyTeam.Client.Table

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.Browser
open Fable.Import.React
open MyTeam
open MyTeam.Client.Components
open MyTeam.Components
open MyTeam.Shared.Components
open Shared.Features.Table.Table
open Thoth.Json

let formRow lbl inpt right =
    div [ Class "row" ] [ label [ Class "col-xs-4" ] [ lbl ]
                          div [ Class "col-xs-6" ] [ inpt ]
                          div [ Class "col-xs-1" ] [ right ] ]

type State =
    { Title : string
      AutoUpdate : bool
      SourceUrl : string }

type EditTable(props) =
    inherit Component<Model, State>(props)
    do base.setInitState ({ Title = props.Title
                            AutoUpdate = props.AutoUpdateTable
                            SourceUrl = props.SourceUrl })
    override this.render() =
        let props = this.props
        let state = this.state
        let handleTitleChange value = this.setState (fun state props -> { state with Title = value })
        let handleSourceUrlChange value = this.setState (fun state props -> { state with SourceUrl = value })
        let handleAutoUpdateChange value = this.setState (fun state props -> { state with AutoUpdate = value })

        fragment [] 
            [ EditBlock.render 
                  { Render =
                        fun isInEditMode -> 
                            fragment [] [
                                (if isInEditMode then    
                                    div [ Class "table-edit" ] [
                                        formRow (str "Divisjon") 
                                                (Textinput.render { Value = state.Title
                                                                    Url = sprintf "/api/tables/%s/%i/title" props.Team props.Year
                                                                    OnChange = handleTitleChange })
                                                (Modal.render 
                                                    { OpenButton = fun handleOpen -> btn Danger Normal [ OnClick handleOpen ] [ Icons.delete ]
                                                      Content = 
                                                        fun handleClose ->
                                                            div [] [ 
                                                              h4 [] [str <| sprintf "Er du sikker på at du vil slette tabellen for %s %i?" props.Team props.Year] 
                                                              div [ Class "text-center"] [
                                                                  br []
                                                                  SubmitButton.render { IsSubmitted = false
                                                                                        Text = "Ja" 
                                                                                        SubmittedText = "Slettet"
                                                                                        Endpoint = SubmitButton.Delete <| sprintf "/api/tables/%s/%i" props.Team props.Year
                                                                                        OnSubmit = Browser.location.reload }
                                                                  btn Default Lg [ OnClick handleClose ] [ str "Nei" ]
                                                              ]
                                                          ]
                                                    })                                                                

                                        formRow (str "Oppdater automatisk") 
                                                (Checkbox.render { Value = props.AutoUpdateTable
                                                                   Url = sprintf "/api/tables/%s/%i/autoupdate" props.Team props.Year
                                                                   OnChange = handleAutoUpdateChange })                                                        
                                                empty
                                                                                                   
                                        (if state.AutoUpdate then
                                            formRow (str "Url til tabell på fotball.no") 
                                                    (Textinput.render { Value = state.SourceUrl
                                                                        Url = sprintf "/api/tables/%s/%i/sourceurl" props.Team props.Year
                                                                        OnChange = handleSourceUrlChange })
                                                    empty                                                            
                                         else empty)                                                                
                                        br []
                                    ]
                                 else empty)
                                h2 [] [ Icons.trophy ""
                                        whitespace
                                        str state.Title ] 
                            ]
                  } 
            ]

let element model = ofType<EditTable, _, _> model []
let node = document.getElementById (clientView)

if not <| isNull node then 
    node.getAttribute (Interop.modelAttributeName)
    |> Decode.Auto.fromString<Model>
    |> function 
    | Ok model -> ReactDom.render (element model, node)
    | Error e -> failwithf "Json deserialization failed: %O" e
