module Shared.Util.Enums

open System
open Microsoft.FSharp.Reflection


let fromString<'a> typedef (s:string)  =
    match FSharpType.GetUnionCases typedef |> Array.filter (fun case -> case.Name.ToLowerInvariant() = s.ToLowerInvariant()) with
    |[|case|] -> FSharpValue.MakeUnion(case,[||]) :?> 'a
    |_ -> failwithf "Ugyldig verdi for Enum %s: '%s'" typedef.FullName s