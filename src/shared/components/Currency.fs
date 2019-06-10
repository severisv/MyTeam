module Shared.Components.Currency

open Shared
open Fable.React
open Fable.React.Props

type CurrencyColor =
    | Positive
    | Neutral
    | Negative

type CurrencyStyle =
    | Normal
    | Colored of (int -> CurrencyColor)

let currency attr amount =
        let color = attr
                    |> List.tryHead
                    |> Option.map (fun style ->
                        style
                        |> function
                        | Colored getColor ->
                            match getColor amount with
                            | Positive -> "u-currency--plus"
                            | Negative -> "u-currency--minus"
                            | Neutral -> ""                       
                        | Normal -> "" )
                    |> Option.defaultValue ""
        
        span [Class color] [  (string >> Strings.replace "-" "- " >> str) amount
                              span [Class "u-currency-postfix"]
                            [ str ",-" ] ]
        