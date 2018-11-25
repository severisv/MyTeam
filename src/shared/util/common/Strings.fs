module MyTeam.Strings

open System

let hasValue s = not <| String.IsNullOrWhiteSpace(s)

let toLower (a: obj) = (string a).ToLower()

let defaultValue a = if hasValue a then a else ""

let asOption a = if hasValue a then Some a else None

let (!!) = defaultValue
