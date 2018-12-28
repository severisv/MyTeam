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

type State =
    { Title : string }

type EditTable(props) =
    inherit Component<Model, State>(props)
    do base.setInitState ({ Title = props.Title })
    override this.render() =
        let props = this.props
        let state = this.state
        let handleChange value = this.setState (fun state props -> { state with Title = value })
        fragment [] 
            [ EditBlock.render 
                  { Render =
                        fun isInEditMode -> 
                            if not isInEditMode then 
                                fragment [] [ h2 [] [ Icons.trophy ""
                                                      whitespace
                                                      str state.Title ] ]
                            else 
                                div [ Class "table-edit" ] 
                                    [ Icons.trophy ""
                                      whitespace
                                      Textinput.render { Value = state.Title
                                                         Url =
                                                             sprintf "/api/tables/%s/%i/title" 
                                                                 props.Team props.Year
                                                         OnChange = handleChange } ] } ]

let element model = ofType<EditTable, _, _> model []
let node = document.getElementById (clientView)

if not <| isNull node then 
    node.getAttribute (Interop.modelAttributeName)
    |> Decode.Auto.fromString<Model>
    |> function 
    | Ok model -> ReactDom.render (element model, node)
    | Error e -> failwithf "Json deserialization failed: %O" e
