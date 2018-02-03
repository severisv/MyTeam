namespace MyTeam

open System

[<AutoOpen>]
module StringExtensions =

        type System.String with
            member s1.EqualsIc(s2: string) = System.String.Equals(s1, s2, System.StringComparison.CurrentCultureIgnoreCase)

        type System.String with
            member s.HasValue = not <| String.IsNullOrWhiteSpace(s)


        let str a = a.ToString()