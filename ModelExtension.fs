namespace OpilioCraft.Lisp

type UnaryFunction = Expression -> Expression
type BinaryFunction = Expression * Expression -> Expression

module FunctionHelper =
    let liftExpression (expr : Expression) = [ expr ]

    let liftUnary (opName : string) (op : UnaryFunction) (args : Expression list) =
        match args with
        | [ arg ] -> op arg
        | _ -> raise <| InvalidLispExpressionException $"Function {opName} expects exactly one argument"

    let liftBinary (opName : string) (op : BinaryFunction) (args : Expression list) =
        match args with
        | [ a; b ] -> op (a, b)
        | _ -> raise <| InvalidLispExpressionException $"Function {opName} expects exactly two arguments"
