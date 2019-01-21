module MyTeam.Validation

open FSharp.Quotations.Evaluator
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open MyTeam
open System

type ValidationFn<'T> = string * 'T -> Result<unit, string>



let internal getNameAndValue (expr : Expr<'T>) =
    let getName (expr : Expr) =
        match expr with
        | PropertyGet(a, pi, list) -> pi.Name
        | _ -> ""
    (getName expr, (QuotationEvaluator.Evaluate expr))

let validationError name message =
    { Name = name
      Errors = [ message ] }

let validate (expr : Expr<'T>) (validationFns : list<ValidationFn<'T>>) =
    let combine (validationFns : list<ValidationFn<'T>>) (name, value) =
        { Name = name
          Errors =
              validationFns
              |> List.map (fun fn -> fn (name, value))
              |> List.choose (fun result -> 
                     match result with
                     | Ok _ -> None
                     | Error e -> Some e) }
    getNameAndValue expr |> combine validationFns

let (>-) = validate


let map form errors =
    let errors = errors |> List.filter (fun e -> e.Errors |> (List.isEmpty >> not))
    if errors |> Seq.isEmpty then Ok form
    else Error errors


let (>>=) result (fn : 'a -> HttpResult<'b>) =
    match result with
    | Ok a -> fn a
    | Error ve -> ValidationErrors ve

[<AutoOpen>]
module Validators =
    let isValidEmail (__, value) =
        let regex = Text.RegularExpressions.Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
        if String.IsNullOrWhiteSpace(value) || regex.IsMatch(value) then Ok()
        else Error "E-postadressen er ugyldig"
    
    let isRequired (__, value) =
        let value = string value
        if String.IsNullOrWhiteSpace(value) then Error "Feltet er obligatorisk"
        else Ok()
