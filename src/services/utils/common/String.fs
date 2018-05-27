namespace MyTeam

open System
open System.Text.RegularExpressions

[<AutoOpen>]
module StringExtensions =

        type System.String with
            member s1.EqualsIc(s2: string) = System.String.Equals(s1, s2, System.StringComparison.CurrentCultureIgnoreCase)

        let hasValue s = not <| String.IsNullOrWhiteSpace(s)

        type System.String with
            member s.HasValue = hasValue s

        let toLower (a: string) = a.ToLower()

        let isNumber (s: string) = s |> Seq.forall Char.IsDigit

        let toLowerString = string >> toLower

        let isNullOrEmpty a = 
            String.IsNullOrEmpty(a) 

        let replace (a: string) (b: string) (str: string) = 
            str.Replace(a,b)

        let regexReplace (regex: string) (replaceWith: string) (str: string) =
            let rgx = Regex(regex)
            rgx.Replace(str, replaceWith)        


        let nullCheck s = 
            if hasValue s then s else ""
                        