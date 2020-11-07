module Client.Features.Players.Form


open Client.Components

open Fable.React
open Fable.React.Props
open Shared
open Shared.Components
open Shared.Components.Base
open Shared.Components.Datepicker
open Shared.Components.Forms
open Shared.Components.Links
open Shared.Domain
open Shared.Util
open Shared.Util.ReactHelpers
open Shared.Validation
open System
open Thoth.Json


type Props =
    { Id: Guid
      FirstName: string
      MiddleName: string
      LastName: string
      Positions: string list
      BirthDate: DateTime option
      StartDate: DateTime option
      Phone: string
      UrlName: string
      Image: string }

[<CLIMutable>]
type EditPlayer =
    { FirstName: string
      MiddleName: string
      LastName: string
      Positions: string list
      BirthDate: DateTime
      StartDate: DateTime
      Image: string 
      Phone: string 
      }

type State = Props

let containerId = "player-form"


let element =
    FunctionComponent.Of(fun (props: Props) ->
        let state = Hooks.useState<State> (props)

        let errorState = Hooks.useState<string option> None

        let setFormValue (v: State -> State) = state.update v
        let state = state.current

        let validation =
            Map [ "FirstName", validate "Fornavn" state.FirstName [ isRequired ]
                  "LastName", validate "Etternavn" state.LastName [ isRequired ]                  
                  "BirthDate", validate "Fødselsdato" state.BirthDate [ isSome ]
                  "StartDate", validate "Signert dato" state.StartDate [ isSome ]
                  "Positions", validate "Posisjon" state.Positions [ hasMinimumLength 1 ]
                  "Phone", validate "Telefonnummer" state.Phone [ isRequired ] ]

        let label text = label [] [ str text ]

        let imageInput : IRefHook<Browser.Types.Element option> = createRef None

        fragment [] [
            errorState.current => Alerts.danger
            form
                [ ]
                [ input [ RefValue imageInput
                          Name "ImageFull"
                          HTMLAttr.Custom("type", "hidden")
                          Value props.Image ]

                  formRow
                      [  ]
                      [ label "Fornavn" ]
                      [ textInput [ Validation validation.["FirstName"]
                                    OnChange(fun e ->
                                        let value = e.Value
                                        setFormValue (fun form -> { form with FirstName = value }))
                                    Placeholder "Hallvard"
                                    Value state.FirstName ] ]
                  formRow
                      [  ]
                      [ label "Mellomnavn" ]
                      [ textInput [ 
                                    OnChange(fun e ->
                                        let value = e.Value
                                        setFormValue (fun form -> { form with MiddleName = value }))
                                    Value state.MiddleName ] ]
                  formRow
                      [  ]
                      [ label "Etternavn" ]
                      [ textInput [ Validation validation.["LastName"]
                                    OnChange(fun e ->
                                        let value = e.Value
                                        setFormValue (fun form -> { form with LastName = value }))
                                    Placeholder "Thoresen"
                                    Value state.LastName ] ]

                  formRow
                      [ Class "ep-positions" ]
                      [ label "Posisjon" ]
                      [ multiSelect
                          { OnChange = (fun positions -> setFormValue (fun form -> { form with Positions = positions }))
                            Options =
                                (["Målvakt";"Back";"Midtstopper";"Midtbane";"Ving";"Spiss"]
                                 |> List.map (fun p -> { Name = p; Value = p }))
                            Values = props.Positions
                            Validation = validation.["Positions"] } ]

                  formRow
                      [  ]
                      [ label "Født" ]
                      [ dateInput [ Validation validation.["BirthDate"]
                                    Value state.BirthDate
                                    OnDateChange(fun date -> setFormValue (fun form -> { form with BirthDate = date })) ] ]

                  formRow
                      [  ]
                      [ label "Signerte for klubben" ]
                      [ dateInput [ Validation validation.["StartDate"]
                                    Value state.StartDate
                                    OnDateChange(fun date -> setFormValue (fun form -> { form with StartDate = date })) ] ]
                  formRow
                      [  ]
                      [ label "Telefon" ]
                      [ textInput [ Validation validation.["Phone"]
                                    OnChange(fun e ->
                                        let value = e.Value
                                        setFormValue (fun form -> { form with Phone = value }))
                                    Placeholder "12345678"
                                    Value state.Phone ] ]                                    
                  div
                        [ ]
                        [ Send.sendElement (fun o ->
                            { o with
                                  IsDisabled =
                                      validation
                                      |> Map.toList
                                      |> List.map (fun (_, v) -> v)
                                      |> List.concat
                                      |> List.exists (function
                                          | Error e -> true
                                          | _ -> false)
                                  SendElement = btn, [ Normal; Primary ], [
                                       str "Lagre" ]
                                  SentElement = btn, [ Normal; Success ], [ str "Lagret" ]
                                  Endpoint =                                     
                                          Send.Put
                                              (sprintf "/api/players/%O" props.Id,
                                               Some(fun () ->
                                                   Encode.Auto.toString
                                                       (0,
                                                        { FirstName = state.FirstName
                                                          MiddleName = state.MiddleName
                                                          LastName = state.LastName
                                                          Positions = state.Positions
                                                          BirthDate = state.BirthDate.Value
                                                          StartDate = state.StartDate.Value
                                                          Phone = state.Phone 
                                                          Image = imageInput.current.Value.getAttribute("value")                                                          
                                                        })))
                                    

                                  OnSubmit = Some(fun _ -> Browser.Dom.window.location.replace <| sprintf "/spillere/vis/%s" props.UrlName) }) ]                  
                                     ]
                
        ])

hydrate2 containerId Decode.Auto.fromString<Props> element
