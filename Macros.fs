module internal OpilioCraft.Lisp.Runtime.Macros

open OpilioCraft.FSharp.Prelude
open OpilioCraft.Lisp

// LISP macros
let propertyIs (exprList : Expression list) : Expression =
    match exprList with
    | [ Atom (FlexibleValue.String _); Atom _ ] as [ objectPath ; pattern ]->
        List [ Symbol "eq" ; List [ Symbol "property" ; objectPath ] ; pattern ]
    | _ -> raise <| InvalidLispExpressionException "property-is expects an object path string and an atom as arguments"

let propertyIsNot (exprList : Expression list) : Expression =
    match exprList with
    | [ Atom (FlexibleValue.String _); Atom _ ] as [ objectPath ; pattern ]->
        List [ Symbol "not" ; List [ Symbol "property-is" ; objectPath ; pattern ] ]
    | _ -> raise <| InvalidLispExpressionException "property-is expects an object path string and an atom as arguments"

let propertyContains (exprList : Expression list) : Expression =
    match exprList with
    | [ Atom (FlexibleValue.String _); Atom (FlexibleValue.String _) ] as [ objectPath ; pattern ]->
        List [ Symbol "contains" ; List [ Symbol "property" ; objectPath ] ; pattern ]
    | _ -> raise <| InvalidLispExpressionException "property-contains expects an object path string and a string as arguments"

let propertyMatches (exprList : Expression list) : Expression =
    match exprList with
    | [ Atom (FlexibleValue.String _); Atom (FlexibleValue.String _) ] as [ objectPath ; pattern ]->
        List [ Symbol "matches" ; List [ Symbol "property" ; objectPath ] ; pattern ]
    | _ -> raise <| InvalidLispExpressionException "property-matches expects an object path string and a string as arguments"
