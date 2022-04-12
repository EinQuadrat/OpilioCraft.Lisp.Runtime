module internal OpilioCraft.Lisp.Runtime.ObjectPath

open System

open OpilioCraft.FSharp.Prelude
open OpilioCraft.Lisp

type IObjectPathRuntime = OpilioCraft.ObjectPath.IRuntime

// lisp function to integrate OQL
let private evalObjectPath (runtime : IObjectPathRuntime) (postProcessing : obj -> obj) objectPath defaultValue : Expression =
    runtime.TryRun objectPath
    |> Option.map postProcessing
    |> Option.map (FlexibleValue.Wrap >> Atom)
    |> Option.defaultValue defaultValue

let applyObjectPath (runtime : IObjectPathRuntime) (postProcessing : obj -> obj) _ (exprList : Expression list) : Expression =
    match exprList with
    | [ Atom (FlexibleValue.String objectPath) ] ->
        evalObjectPath runtime postProcessing objectPath (Symbol "#UNKNOWN-PROPERTY")

    | [ Atom (FlexibleValue.String objectPath) ; Atom _ ] as [ _ ; defaultValue ]->
        evalObjectPath runtime postProcessing objectPath defaultValue

    | _ -> raise <| new InvalidOperationException()
