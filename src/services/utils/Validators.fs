namespace MyTeam.Validation

open System

[<AutoOpen>]
module Validators =

    let isValidEmail (__, value) =
                let regex = Text.RegularExpressions.Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
                if String.IsNullOrWhiteSpace(value) || regex.IsMatch(value) then
                    Ok ()
                else
                    Error "E-postadressen er ugyldig"


    let isRequired (__, value: 'a) =
                let value = value.ToString()                              

                if String.IsNullOrWhiteSpace(value) then
                    Error "Feltet er obligatorisk"
                else 
                    Ok ()