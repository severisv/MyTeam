module MyTeam.Strings

open System

let hasValue s = not <| String.IsNullOrWhiteSpace(s)

let toLower (a: string) = a.ToLower()