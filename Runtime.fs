namespace OpilioCraft.Lisp.Runtime

open System.IO

open OpilioCraft.Lisp
open OpilioCraft.Lisp.StandardLib

type LispRuntime (postProcess : obj -> obj) =
    // initialize runtimes
    let objPathRuntime = new OpilioCraft.ObjectPath.DefaultRuntime () :> OpilioCraft.ObjectPath.IRuntime
    let runtime = new OpilioCraft.Lisp.DefaultRuntime (OpilioCraft.Lisp.StandardLib.init) :> OpilioCraft.Lisp.IRuntime

    // populate function table
    let registerUnaryOperator name (unaryOp : UnaryOperator) = runtime.RegisterFunction name (liftUnaryOp name unaryOp)
    let registerBinaryOperator name (binaryOp : BinaryOperator) = runtime.RegisterFunction name (liftBinaryOp name binaryOp)
    let registerFunction name (lispFunction : Function) = runtime.RegisterFunction name lispFunction

    do
        registerFunction "property" (ObjectPath.applyObjectPath objPathRuntime postProcess)
        registerBinaryOperator "contains" Functions.bopContains
        registerBinaryOperator "matches" Functions.bopMatches

    // populate macro table
    let registerMacro name (lispMacro : Macro) = runtime.RegisterMacro name lispMacro

    do
        registerMacro "property-is" Macros.propertyIs
        registerMacro "property-is-not" Macros.propertyIsNot
        registerMacro "property-contains" Macros.propertyContains
        registerMacro "property-matches" Macros.propertyMatches

    // simplify registration
    member _.Register(name, body : Function) = registerFunction name body
    member _.Register(name, body : UnaryOperator) = registerUnaryOperator name body
    member _.Register(name, body : BinaryOperator) = registerBinaryOperator name body
    member _.Register(name, body : Macro) = runtime.RegisterMacro name body

    // IRuntime interface
    interface OpilioCraft.Lisp.IRuntime with
        member _.Eval expr = runtime.Eval expr
        member _.Parse expr = runtime.Parse expr
        member _.RegisterFunction name funcBody = runtime.RegisterFunction name funcBody
        member _.RegisterMacro name macroBody = runtime.RegisterMacro name macroBody
        member _.TryParse expr = runtime.TryParse expr

    // High-level API
    member x.ParseFile path =
        if File.Exists path
        then
            path
            |> File.ReadAllText
            |> runtime.Parse
        else
            invalidArg "path" "no model for given path found"

    member x.ParseString source =
        source |> runtime.Parse

    member x.Eval lispExpr =
        lispExpr |> runtime.Eval

    member x.EvalWithContext context lispExpr =
        objPathRuntime.ObjectData <- context
        lispExpr |> runtime.Eval

    member x.Run = x.ParseString >> x.Eval
    member x.RunWithContext context = x.ParseString >> (x.EvalWithContext context)
