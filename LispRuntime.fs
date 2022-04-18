﻿namespace OpilioCraft.Lisp

open System.IO

open OpilioCraft.Lisp.Runtime
open OpilioCraft.Lisp.Runtime.ObjectPathExtension

type LispRuntime private () =
    inherit MinimalRuntime ()

    // initialize ObjectPath context
    let mutable objectPathContext : ObjectPathContext =
        {
            Runtime = new OpilioCraft.ObjectPath.DefaultRuntime () :> OpilioCraft.ObjectPath.IRuntime
            ObjectData = "#UNDEFINED" :> obj
            ResultHook = id
        }

    // static initialization
    static member Initialize () =
        let instance = new LispRuntime ()

        // add standard library
        OpilioCraft.Lisp.StandardLib.unaryFunctions |> Map.iter (fun name body -> instance.Register(name, body))
        OpilioCraft.Lisp.StandardLib.binaryFunctions |> Map.iter (fun name body -> instance.Register(name, body))
        OpilioCraft.Lisp.StandardLib.ordinaryFunctions |> Map.iter (fun name body -> instance.Register(name, body))

        // additional functions
        instance.Register("contains", Functions.bfuncContains)
        instance.Register("matches", Functions.bfuncMatches)

        // ObjectPath extension
        instance.Register("has-property",       (funcIsValidObjectPath instance.ObjectPathContextProvider))
        instance.Register("property",           (funcLookupObjectPath instance.ObjectPathContextProvider))
        instance.Register("property-is",        macroPropertyIs)
        instance.Register("property-is-not",    macroPropertyIsNot)
        instance.Register("property-contains",  macroPropertyContains)
        instance.Register("property-matches",   macroPropertyMatches)

        // return it
        instance

    // simplify function registration
    member x.Register(name, body : Function) = x.RegisterFunction name body
    member x.Register(name, body : UnaryFunction) = x.RegisterFunction name (FunctionHelper.liftUnary name body)
    member x.Register(name, body : BinaryFunction) = x.RegisterFunction name (FunctionHelper.liftBinary name body)

    // ObjectPath context
    member private _.ObjectPathContextProvider () = objectPathContext
    member x.InjectResultHook hook = objectPathContext <- { objectPathContext with ResultHook = hook }; x
    member x.InjectObjectData objData = objectPathContext <- { objectPathContext with ObjectData = objData }; x

    // High-level API
    member x.LoadFile path =
        if File.Exists path
        then
            path |> File.ReadAllText
        else
            raise <| new System.IO.FileNotFoundException(null, path)

    member x.TryParse lispString =
        try
            x.Parse lispString |> Some
        with
        | _ -> None

    member x.EvalWithContext contextData lispExpr =
        x.InjectObjectData(contextData).Eval lispExpr

    member x.EvalWithContextAndResult contextData lispExpr =
        x.InjectObjectData(contextData).EvalWithResult lispExpr

    member x.RunWithContext context =
        x.Parse >> (x.EvalWithContext context)

    member x.RunWithContextAndResult context =
        x.Parse >> (x.EvalWithContextAndResult context)

    member _.ResultToString (result : Result<Expression, string>) =
        match result with
        | Ok result -> result.ToString()
        | Error errorMsg -> $"Error occurred: {errorMsg}"

    member x.PrintResult (result : Result<Expression, string>) =
        x.ResultToString result
        |> System.Console.WriteLine
