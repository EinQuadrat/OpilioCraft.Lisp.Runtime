namespace OpilioCraft.Lisp

type LispFunctionType =
    | Ordinary = 1
    | UnaryOperator = 2
    | BinaryOperator = 3

type LispFunctionAttribute (name : string, funcType : LispFunctionType) =
    inherit System.Attribute ()

    member val Name = name with get
    member val FunctionType = funcType with get
