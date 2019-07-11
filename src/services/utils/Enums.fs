module MyTeam.Enums

open System

let getValues<'T>() =
    let cases = FSharp.Reflection.FSharpType.GetUnionCases(typeof<'T>)
    [ for c in cases do 
         yield FSharp.Reflection.FSharpValue.MakeUnion(c, [| |]) :?> 'T
    ]

let tryParse<'T> value : Result<'T, string> =
    if not (String.IsNullOrEmpty value) then 
        try 
            Ok(Enum.Parse(typeof<'T>, value, true) :?> 'T)
        with :? ArgumentException -> 
            Error
                (sprintf "Ugyldig verdi for Enum %s: '%s'" typeof<'T>.FullName 
                     value)
    else Error "Input var en null eller en tom streng"

let parse<'T> value =
    try 
        Enum.Parse(typeof<'T>, value, true) :?> 'T
    with :? ArgumentException -> 
        failwithf "Ugyldig verdi for Enum %s: '%s'" typeof<'T>.FullName value

let toNullableInt (v : Option<'T> when 'T :> Enum) =
    match v with
    | Some x when Enum.IsDefined(typeof<'T>, x) -> 
        Nullable(LanguagePrimitives.EnumToValue(x))
    | _ -> Nullable()

open Microsoft.FSharp.Reflection

let fromString<'a> (s:string) =
    match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun case -> case.Name.ToLowerInvariant() = s.ToLowerInvariant()) with
    |[|case|] -> FSharpValue.MakeUnion(case,[||]) :?> 'a
    |_ -> failwithf "Ugyldig verdi for Enum %s: '%s'" typeof<'a>.FullName s


let tryFromString<'a> s =
    try Ok <| fromString<'a> s
    with e -> Error <| e.Message
