namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open MyTeam

[<AutoOpen>]
module AjaxComponents =  
    let ajaxSuccessIndicator =
        div [ _class "ajax-success-indicator" ] [
            span [_class "label label-success" ] [icon (fa "check") ""]
            span [ _class "label label-danger" ] [icon (fa "exclamation-triangle") ""]
            icon "loader fa fa-spin fa-spinner" ""
        ]

    let ajaxCheckbox href isChecked =
        input [ 
            _class "form-control ajax-checkbox"
            attr "data-ajax-href" href
            _type "checkbox" 
            (isChecked =? (_checked, _empty))
        ]    
     