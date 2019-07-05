module Client.News.DeleteArticle
open Client.Components
open Fable.React.Props
open Fable.React
open Shared.Components
open Shared.Util
open Thoth.Json

type Model = {
    Name: string option
    Title: string
}

let deleteArticle model children =
    model.Name
    |> Option.map (fun name -> 
        Modal.modal
            { OpenButton = fun handleOpen ->
                btn [Danger; ButtonSize.Normal; Class "pull-right"; OnClick (fun e -> e.preventDefault(); handleOpen(e))]
                    [Icons.delete]
              Content =
                fun handleClose ->            
                    div [] [
                      h4 [] [ str <| sprintf "Er du sikker på at du vil slette '%s'?" model.Title ]
                      div [ Class "text-center" ] [
                          br []
                          buttonLink (sprintf "/nyheter/slett/%O" model.Name) Danger Lg [] [str "Slett"]
                          btn [Lg; OnClick <| fun _ -> handleClose()] [str "Avbryt"]
                      ]
                ]
        })
    |> Option.defaultValue (Fable.React.Helpers.fragment [] [])

ReactHelpers.hydrate "delete-article-modal" Decode.Auto.fromString<Model> deleteArticle

