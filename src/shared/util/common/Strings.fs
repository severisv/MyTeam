module MyTeam.Strings

open System

let hasValue s = not <| String.IsNullOrWhiteSpace(s)

let toLower (a: obj) = (string a).ToLower()