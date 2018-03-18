namespace MyTeam.Ajax

open MyTeam

module Api = 
    [<CLIMutable>]
    type CheckboxPayload = {
        value: bool
    }    
   
    let checkbox fn next (ctx: HttpContext) =           

        let payload = ctx.BindJson<CheckboxPayload>()

        fn payload.value ctx.Database
        |> fromResult next ctx


namespace MyTeam.Views
open Giraffe.GiraffeViewEngine
open MyTeam

[<AutoOpen>]
module Components =  
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
     