module MyTeam.Ajax

open MyTeam
open MyTeam.Views
open Giraffe.GiraffeViewEngine

   
let ajaxSuccessIndicator =
    div [ _class "ajax-success-indicator" ] [
        span [_class "label label-success" ] [icon (fa "check") ""]
        span [ _class "label label-danger" ] [icon (fa "exclamation-triangle") ""]
        icon "loader fa fa-spin fa-spinner" ""
    ]



[<CLIMutable>]
type CheckboxPayload = { value: bool }        

let ajaxCheckbox href isChecked =
    input [ 
        _class "form-control ajax-checkbox"
        attr "data-ajax-href" href
        _type "checkbox" 
        (isChecked =? (_checked, _empty))
    ]    
     


let load url =
      div [ _class "ajax-load" ;_href url ] []