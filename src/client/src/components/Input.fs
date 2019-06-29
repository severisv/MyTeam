module Shared.Components.Input

open Fable.React
open Fable.React.Props
open Shared.Util.Html

[<CLIMutable>]
type CheckboxPayload =
    { value : bool }

[<CLIMutable>]
type StringPayload =
    { Value : string }



type RadioOption<'a> = {
    Label: string
    Value: 'a
}


let radio onClick options selectedValue =
    div [Class "radios"] 
      (options
      |> List.map (fun opt ->
                  label [Class "radio-inline"] [
                    input [Type "radio"
                           Value opt.Value
                           OnChange (fun _ -> onClick opt.Value)
                           Checked (Some opt.Value = selectedValue)
                           ]
                    str opt.Label
                  ]
          )
)
    
    
    
