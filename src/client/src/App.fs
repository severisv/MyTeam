namespace Client

module EntryPoint =
    
    [ Games.SelectSquad.element |> ignore
      Features.Games.Form.element    |> ignore
      Features.Trainings.Form.element    |> ignore
      Features.Players.Stats.element    |> ignore
      GamePlan.View.element |> ignore
      Table.Edit.element |> ignore
      Table.Create.element |> ignore
      Table.Create.element |> ignore
      Admin.AddPlayers.element |> ignore
      Fines.List.element |> ignore
      Fines.RemedyRates.element |> ignore
      Fines.Payments.element |> ignore
      Events.List.element |> ignore
      News.DeleteArticle.deleteArticle |> ignore ] 
    |> ignore
    