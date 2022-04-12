module internal OpilioCraft.Lisp.Runtime.Functions

open System.Text.RegularExpressions

open OpilioCraft.FSharp.Prelude
open OpilioCraft.Lisp

[<LispFunction("contains", LispFunctionType.BinaryOperator)>]
let bopContains _ (exprA, exprB) : Expression =
    match (exprA, exprB) with
    | Atom (FlexibleValue.String value) , Atom (FlexibleValue.String pattern) ->
        value.Contains(pattern) |> LispBoolean
    | _ -> raise <| InvalidArgsException

[<LispFunction("matches", LispFunctionType.BinaryOperator)>]
let bopMatches _ (exprA, exprB) : Expression =
    match (exprA, exprB) with
    | Atom (FlexibleValue.String value) , Atom (FlexibleValue.String pattern) ->
        Regex.IsMatch(value, pattern) |> LispBoolean
    | _ -> raise <| InvalidArgsException
