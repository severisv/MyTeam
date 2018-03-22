namespace MyTeam

open System

[<AutoOpen>]
module StringExtensions =

        type System.String with
            member s1.EqualsIc(s2: string) = System.String.Equals(s1, s2, System.StringComparison.CurrentCultureIgnoreCase)

        let hasValue s = not <| String.IsNullOrWhiteSpace(s)

        type System.String with
            member s.HasValue = hasValue s

        let toLower (a: string) = a.ToLower()

        let isNumber (s: string) = s |> Seq.forall Char.IsDigit