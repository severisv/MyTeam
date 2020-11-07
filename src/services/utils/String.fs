namespace MyTeam

open Shared
open System
open System.Text.RegularExpressions

[<AutoOpen>]
module StringExtensions =

        type System.String with
            member s1.EqualsIc(s2: string) = System.String.Equals(s1, s2, System.StringComparison.CurrentCultureIgnoreCase)


        type System.String with
            member s.HasValue = Strings.hasValue s

        let toLower = Strings.toLower
        let contains (a: string) (b: string) = a.Contains(b)

        let isNumber (s: string) = s |> Seq.forall Char.IsDigit

        let toLowerString a = (string >> Strings.toLower) a

        let isNullOrEmpty a = 
            String.IsNullOrEmpty(a) 

        let replace (a: string) (b: string) (str: string) = 
            str.Replace(a,b)

        let regexReplace (regex: string) (replaceWith: string) (str: string) =
            let rgx = Regex(regex)
            rgx.Replace(str, replaceWith)        


        let nullCheck s = 
            if Strings.hasValue s then s else ""
                        
        let truncate length (str: string) =
            if str.Length > length then str.Substring(0, length)
            else str


     