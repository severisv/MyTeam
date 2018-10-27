module MyTeam.Ajax

open MyTeam
open MyTeam.Views
open Giraffe.GiraffeViewEngine
open MyTeam.Shared.Components
   
let ajaxSuccessIndicator =
    div [ _class "ajax-success-indicator" ] [
        !!Labels.success
        !!Labels.error
        icon "loader fa fa-spin fa-spinner" ""
    ]




let ajaxCheckbox href isChecked =
    input [ 
        _class "form-control ajax-checkbox"
        attr "data-ajax-href" href
        _type "checkbox" 
        (isChecked =? (_checked, _empty))
    ]    
     


let load url =
      div [ _class "ajax-load" ;_href url ] []