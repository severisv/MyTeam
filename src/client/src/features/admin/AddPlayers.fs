module Client.Admin.AddPlayers

open Fable.React
open Fable.React.Props
open Client.Components
open Shared.Util.ReactHelpers
open Thoth.Json
open Shared
open Shared.Image
open Shared.Components
open Base
open Layout
open Forms
open Send

[<CLIMutable>]
type AddMemberForm = {
    FacebookId: string 
    ``E-postadresse``: string
    Fornavn: string
    Mellomnavn: string
    Etternavn: string
}

type Model = {
    ImageOptions: CloudinaryOptions
    MemberRequests: AddMemberForm list
}

let clientView = "addplayers"
let modelAttribute = "model"

let defaultForm = {
    ``E-postadresse`` = ""
    Fornavn = ""
    Etternavn = ""
    Mellomnavn = ""
    FacebookId = ""
 }

type State = {
    Player : AddMemberForm
    Message : string option
    Errors : string list
    SuccessMessage : string option
 }

let onFormError setState =
        function
        | Send.ValidationError e ->
              let errors = e |> List.collect (fun e -> e.Errors)
              setState (fun state props -> { state with Errors = errors; SuccessMessage = None })

        | Send.Exception e ->
            let errors = [e]
            setState (fun state props -> { state with Errors = errors; SuccessMessage = None })


let onSubmitForm setState () =
    setState (fun state props ->
                 { state with Errors = []
                              SuccessMessage = Some(sprintf "%s %s ble lagt til" state.Player.Fornavn state.Player.Etternavn)
                              Player = defaultForm })

let handleFormChange setState update value =
    setState (fun state props -> { state with Player = update state.Player value })
    
    
let element =
        FunctionComponent.Of(fun (props: Model) ->
                 
                 let state = Hooks.useState<State> ( { Player = defaultForm
                                                       Message = None
                                                       Errors = []
                                                       SuccessMessage = None })
                 
                 let setState fn = state.update (fun prevState -> fn prevState props)
                 let state = state.current
                 
                 let handleFormChange = handleFormChange setState

                 block [] [
                     h3 [] [str "Medlemsforespørsler";
                            whitespace;
                            tooltip "Nye spillere kan registrere seg på wamkam.no/blimed, så vil de dukke opp her. Når de er godkjent får til tilgang til internsidene."
                                [] [Icons.info ""] ]
                     ul [ Class "list-users" ] 
                        (props.MemberRequests
                         |> List.map (fun request ->
                            li [] [ 
                                span[] [
                                    img [Src <| Image.getMember props.ImageOptions
                                                     (fun opts -> { opts with Height = Some 50; Width = Some 50 })
                                                     "" request.FacebookId ]
                                    str (sprintf "%s %s %s" request.Fornavn request.Mellomnavn request.Etternavn) 
                                    ]
                                sendElement
                                     (fun o ->
                                     { o with
                                           SendElement = btn, [Normal;Primary], [str "Legg til"]
                                           SentElement = btn, [Normal;Success], [str "Lagt til"]                         
                                           Endpoint = Send.Post ("/api/members", Some (fun () -> Encode.Auto.toString(0, request)))
                                     })
                            ]
                        ))  
                                    
                     br []
                     form [ Horizontal 3 ] [
                         h3 [] [ str "Legg til manuelt" ]
                         state.SuccessMessage => Alerts.success
                         formRow [ Horizontal 2; Class "text-danger" ]
                                 []
                                 [ ul [] (state.Errors |> List.map (fun e -> li [] [ str e ])) ]

                         formRow [ Horizontal 2 ]
                                 [ str "E-post" ]
                                 [ textInput [ Placeholder "hallvardthoresen@gmail.com"
                                               OnChange (fun e -> e.Value |> (handleFormChange <|
                                                                                fun form value -> { form with ``E-postadresse`` = value }))
                                               Value state.Player.``E-postadresse`` ] ]
                         formRow [ Horizontal 2 ]
                                 [ str "Fornavn" ]
                                 [ textInput [ Placeholder "Hallvard"
                                               OnChange (fun e -> e.Value |> (handleFormChange <|
                                                                              fun form value -> { form with Fornavn = value }))
                                               Value state.Player.Fornavn ] ]

                         formRow [ Horizontal 2 ]
                                 [ str "Mellomnavn" ]
                                 [ textInput [ Placeholder "Jensen"
                                               OnChange (fun e -> e.Value |> (handleFormChange <|
                                                                              fun form value -> { form with Mellomnavn = value }))
                                               Value state.Player.Mellomnavn ] ]

                         formRow [ Horizontal 2 ]
                                 [ str "Etternavn" ]
                                 [ textInput [ Placeholder "Thoresen"
                                               OnChange (fun e -> e.Value |> (handleFormChange <|
                                                                              fun form value -> { form with Etternavn = value }))
                                               Value state.Player.Etternavn ] ]

                         formRow [ Horizontal 2 ]
                                 []
                                 [ Send.sendElement
                                         (fun o ->
                                         { o with
                                               SendElement = btn, [Normal;Primary], [str "Legg til"]
                                               SentElement = btn, [Normal;Success], [str "Lagt til"]                         
                                               Endpoint = Send.Post ("/api/members", Some (fun () -> Encode.Auto.toString(0, state.Player)))
                                               OnSubmit = Some <| !> (onSubmitForm setState)
                                               OnError = Some <| onFormError setState
                                         })]
                     ]
                 ]
                 
        )

hydrate2 clientView Decode.Auto.fromString<Model> element
