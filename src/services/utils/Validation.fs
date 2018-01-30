namespace MyTeam.Validation

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open FSharp.Quotations.Evaluator
open MyTeam

type ValidationFn<'T> = (string * 'T) -> Result<unit, string>


module ValidationHelpers =
    let combine (validationFns : list<ValidationFn<'T>>) (name, value) =
        {
            Name = name
            Errors = validationFns 
                    |> List.map(fun fn -> fn(name, value)) 
                    |> List.choose (fun result -> 
                        match result with 
                            | Ok _ -> None
                            | Error e -> Some e
                        )
        }  

    let createResult form errors =
                        let errors = errors 
                                    |> List.filter (fun e -> not (e.Errors |> List.isEmpty))                               
                        
                        if errors |> Seq.isEmpty then
                            Ok (form)
                        else
                            Error (ValidationErrors errors)   

    let getNameAndValue (expr : Expr<'T>) =
        let getName(expr : Expr) = 
            match expr with
            | PropertyGet(a, pi, list) -> pi.Name
            | _ -> ""                          

        (getName expr, (QuotationEvaluator.Evaluate expr))


open ValidationHelpers

[<AutoOpen>]
module Framework =
    let validationError name message = 
        { Name = name; Errors=[message] }

    let (>-) (expr : Expr<'T>) (validationFns: list<ValidationFn<'T>>) =
        getNameAndValue expr |> combine validationFns

    let (==>) validationResult form =
       form |> createResult validationResult    


