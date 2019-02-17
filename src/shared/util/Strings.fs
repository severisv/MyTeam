module Shared.Strings

open System
open System.Text.RegularExpressions

let hasValue s = not <| String.IsNullOrWhiteSpace(s)
let toLower (a: obj) = (string a).ToLower()
let split separator (str: string) = str.Split(separator) |> Array.toList
let trim (str: string) = str.Trim()
let removeAllWhitespaces (s: string) = Regex.Replace(s, @"\s+", "")
let removeDoubleWhitespaces (s: string) = Regex.Replace(s, "[ ]{2,}", " ")
let contains otherString (str: string) = str.Contains(otherString)
let defaultValue a = if hasValue a then a else ""
let asOption a = if hasValue a then Some a else None
let (!!) = defaultValue
let replace (a: string) (b: string) (str: string) = str.Replace(a,b)