module MyTeam.Validation

open FSharp.Quotations.Evaluator
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Shared
open Shared.Validation

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
          |> List.map (fun fn -> fn name value)
          |> List.choose (function
                 | Ok _ -> None
                 | Error e -> Some e) }

let (>-) = validateField
