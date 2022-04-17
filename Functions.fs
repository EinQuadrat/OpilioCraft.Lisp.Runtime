module internal OpilioCraft.Lisp.Runtime.Functions

open System.Text.RegularExpressions

open OpilioCraft.FSharp.Prelude
open OpilioCraft.Lisp

let bfuncContains (exprA, exprB) : Expression =
    match (exprA, exprB) with
    | Atom (FlexibleValue.String value) , Atom (FlexibleValue.String pattern) ->
        value.Contains(pattern) |> LispBoolean
    | _ -> raise <| InvalidLispExpressionException $"contains expects two strings arguments"

let bfuncMatches (exprA, exprB) : Expression =
    match (exprA, exprB) with
    | Atom (FlexibleValue.String value) , Atom (FlexibleValue.String pattern) ->
        Regex.IsMatch(value, pattern) |> LispBoolean
    | _ -> raise <| InvalidLispExpressionException $"matches expects two string arguments"
