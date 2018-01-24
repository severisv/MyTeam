namespace MyTeam
open System

module Enums =    
    let getValues<'T>() = 
        Enum.GetValues(typeof<'T>) 
        |> Seq.cast<'T> 
        |> Seq.toList            



    let tryParse<'T> value : Result<Option<'T>, string> = 
        if not (String.IsNullOrEmpty value) then
            try
                Ok (Some (Enum.Parse(typeof<'T>, value, true)  :?> 'T))
            with
            | :? ArgumentException -> Error (sprintf "Ugyldig verdi for Enum %s: '%s'" typeof<'T>.FullName value)                
        else
            Ok None

    let parse<'T> value = 
        try
            Enum.Parse(typeof<'T>, value, true)  :?> 'T
        with
        | :? ArgumentException -> failwithf "Ugyldig verdi for Enum %s: '%s'" typeof<'T>.FullName value             
               

    let toNullableInt (v : Option<'T> when 'T:> Enum) =
        match v with
        | Some x when Enum.IsDefined (typeof<'T>, x) -> 
             Nullable(LanguagePrimitives.EnumToValue(x))
        | _ -> Nullable ()