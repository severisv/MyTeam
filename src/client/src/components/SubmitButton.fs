module MyTeam.Client.Components.SubmitButton

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.React
open Fable.PowerPack
open Fable.PowerPack.Fetch
open MyTeam.Components
open MyTeam.Shared.Components

type Endpoint<'a> =
    | Post of string * 'a
    | Delete of string

type SubmitButtonState =
    | Default
    | Submitted
    | Posting
    | Error

type SubmitButtonProps<'a> =
    { IsSubmitted : bool
      Text: string
      SubmittedText: string
      Endpoint : Endpoint<'a>
      OnSubmit: unit -> unit }


let defaultButton attr content = 
            btn Primary Lg attr  content      

type SubmitButton<'a>(props) =
    inherit Component<SubmitButtonProps<'a>, SubmitButtonState>(props)
    
    do 
        base.setInitState(if props.IsSubmitted then Submitted else Default) 
        
    member this.handleClick _ =
            let props = this.props
            this.setState(fun _ _ -> Posting)     
            promise { 
                let! res = 
                    match props.Endpoint with
                        | Post (url, payload) -> postRecord url payload []
                        | Delete url -> fetch url [Method HttpMethod.DELETE]
                
                if not res.Ok then 
                    failwithf "Received %O from server: %O" res.Status res.StatusText
                this.setState(fun state props -> Submitted )
                props.OnSubmit()
            }
            |> Promise.catch(fun e -> 
                   Browser.console.error <| sprintf "%O" e
                   this.setState(fun _ _ -> Error))
            |> Promise.start                              
    
    override this.render() =

        let props = this.props
        let state = this.state
        let handleClick =  this.handleClick
                                    
        match state with
          | Submitted ->
              btn 
                Success 
                Lg 
                [ Class "disabled" ] 
                [ Icons.checkCircle
                  whitespace
                  str props.SubmittedText ]
          
          | Posting -> defaultButton [ Class "disabled" ] [ Icons.spinner ]
          | Error -> fragment [] 
                              [ 
                                defaultButton [ OnClick handleClick ] [ str props.Text ]
                                Labels.error
                              ]
          | Default -> 
                defaultButton [ OnClick handleClick ] [ str props.Text ]

let render model = ofType<SubmitButton<'a>, _, _> model []
