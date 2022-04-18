module internal OpilioCraft.Lisp.Runtime.Functions

open System.Text.RegularExpressions

open OpilioCraft.FSharp.Prelude
open OpilioCraft.Lisp

let private applyFunction funcName funcImpl exprA exprB : Expression =
    match (exprA, exprB) with
    | Atom (FlexibleValue.String value) , Atom (FlexibleValue.String pattern) -> funcImpl value pattern |> LispBoolean
    | _ , Atom (FlexibleValue.String pattern) -> LispFalse
    | _ -> raise <| InvalidLispExpressionException $"{funcName} expects a string pattern as second argument"

let bfuncContains (exprA, exprB) : Expression =
    applyFunction "contains" (fun value pattern -> value.Contains(pattern)) exprA exprB

let bfuncMatches (exprA, exprB) : Expression =
    applyFunction "matches" (fun value pattern -> Regex.IsMatch(value, pattern)) exprA exprB
