module Shared.Strings

open System
open System.Text.RegularExpressions

let hasValue s = not <| String.IsNullOrWhiteSpace s
let toLower (a: obj) = (string a).ToLower()
let equalsIgnoreCase (a: string) b = a.Equals(b, StringComparison.CurrentCultureIgnoreCase)
let split separator (str: string) = str.Split([|separator|]) |> Array.toList
let trim (str: string) = str.Trim()
let trimChars chars (str: string)  = str.Trim(chars |> List.toArray)
let removeAllWhitespaces (s: string) = Regex.Replace(s, @"\s+", "")
let removeDoubleWhitespaces (s: string) = Regex.Replace(s, "[ ]{2,}", " ")
let contains otherString (str: string) = str.Contains(otherString)
let defaultValue a = if hasValue a then a else ""
let asOption a = if hasValue a then Some a else None
let (!!) = defaultValue
let replace (a: string) (b: string) (str: string) = str.Replace(a,b)
let truncate maxLength (str: string)  =
    if str.Length > maxLength then
        sprintf "%s..." (str.Substring(0, maxLength))
    else str