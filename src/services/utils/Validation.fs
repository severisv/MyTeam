module MyTeam.Validation

open FSharp.Quotations.Evaluator
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open MyTeam
open System

type ValidationFn<'T> = string * 'T -> Result<unit, string>

let validateField (expr : Expr<'T>) (validationFns : list<ValidationFn<'T>>) =
    let (name, value) =
        let getName expr =
            match expr with
            | PropertyGet(a, pi, list) -> pi.Name
            | _ -> ""
        (getName expr, (QuotationEvaluator.Evaluate expr))
    { Name = name
      Errors =
          validationFns
          |> List.map (fun fn -> fn (name, value))
          |> List.choose (function 
                 | Ok _ -> None
                 | Error e -> Some e) }

let (>-) = validateField

let validate errors =
    let errors = errors |> List.filter (fun e -> e.Errors |> (List.isEmpty >> not))
    if errors |> Seq.isEmpty then Ok()
    else Error errors

let isValidEmail (__, value) =
    let regex = Text.RegularExpressions.Regex @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
    if String.IsNullOrWhiteSpace value || regex.IsMatch value then Ok()
    else Error "E-postadressen er ugyldig"

let isRequired (__, value) =
    let value = string value
    if String.IsNullOrWhiteSpace value then Error "Feltet er obligatorisk"
    else Ok()
