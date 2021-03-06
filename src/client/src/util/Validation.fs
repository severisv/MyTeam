namespace Shared

open System

type ValidationError = { Name: string; Errors: string list }

module Validation =
    type ValidationFn<'a> = string -> 'a -> Result<unit, string>

    let validate name value validationFns =
        validationFns
        |> List.map (fun fn -> fn name value)

    let private validateString fn value =
        let str = string value
        if not <| Strings.hasValue str then Ok() else fn str

    let combine errors =
        let errors =
            errors
            |> List.filter (fun e -> e.Errors |> (List.isEmpty >> not))

        if errors |> Seq.isEmpty then Ok() else Error errors

    let isValidEmail name =
        validateString
        <| fun str ->
            let regex =
                Text.RegularExpressions.Regex @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"

            if regex.IsMatch str then
                Ok()
            else
                Error
                <| sprintf "%s er ikke en gyldig e-postadresse" name

    let isRequired name value =
        let value = string value
        if not <| Strings.hasValue value then Error <| sprintf "%s må fylles ut" name else Ok()

    let minLength length name =
        validateString
        <| fun str ->
            if str.Length < length then
                Error
                <| sprintf "%s må være minst %i tegn" name length
            else
                Ok()

    let maxLength length name =
        validateString
        <| fun str ->
            if str.Length > length then
                Error
                <| sprintf "%s kan være maks %i tegn" name length
            else
                Ok()

    let isNumber name =
        validateString
        <| fun str ->
            if str |> Number.isNumber |> not
            then Error <| sprintf "%s må være et heltall" name
            else Ok()

    let isTimeString name =
        validateString
        <| fun value ->
            Date.tryParseTime value
            |> Option.map (fun _ -> Ok())
            |> Option.defaultValue
                (Error
                 <| sprintf "%s må være et gyldig tidspunkt (f.eks. 14:30)" name)

    let isSome name (value: 'a option) =
        if value.IsSome
        then Ok()
        else Error <| sprintf "%s må være et valgt" name


    let dateIsAfter ((startName, start): string * DateTime option) name (value: DateTime option) =
        match (start, value) with
        | (Some start, Some value) ->
            if start < value
            then Ok()
            else Error <| sprintf "%s må etter %s" name startName
        | _ -> Ok()


    let hasMinimumLength minLength =
        fun name (list: 'a list) ->
            if list.Length >= minLength then
                Ok()
            else
                Error
                <| sprintf "Velg minst én %s" (Strings.toLower name)
