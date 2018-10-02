module MyTeam.Shared.Components.Labels

open Fable.Helpers.React
open Fable.Helpers.React.Props


let error =
    span [ Class "label-feedback label label-danger" ]
                                           [ i [ Class "fa fa-exclamation-triangle" ]
                                               [ ] ]                                              


let success = 
    span [ Class "label-feedback label label-success" ]
                                           [ i [ Class "fa fa-check" ]
                                               [ ] ]
                                               
                                                                                     