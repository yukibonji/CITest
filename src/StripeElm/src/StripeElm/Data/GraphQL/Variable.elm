module Data.GraphQL.Variable exposing (VariableDefinition, toString, InputType)

import Data.GraphQL.Value as Value exposing (Value)


{-|
    ## VariableDefinition

    A GraphQL query can be parameterized with variables, maximizing query reuse,
    and avoiding costly string building in clients at runtime.

    If not defined as constant (for example, in DefaultValue), a Variable can be
    supplied for an input value.

    Variables must be defined at the top of an operation and are in scope
    throughout the execution of that operation.
-}
type alias VariableDefinition =
    { variableName : String
    , variableType : InputType
    , defaultValue : Maybe Value
    }


toString :
    VariableDefinition
    -> String
toString { variableName, variableType, defaultValue } =
    String.join " " <|
        List.filterMap identity
            [ Just <| "$" ++ variableName ++ ":"
            , Just <| inputTypeToString identity variableType
            , Maybe.map Value.toString defaultValue
            ]


{-|
    ##InputType

    GraphQL describes the types of data expected by query variables. Input types
    may be lists of another input type, or a non‐null variant of any other input
    type.

-}
type InputType
    = NamedType String
    | ListType InputType
    | NonNullType InputType


inputTypeToString : (String -> String) -> InputType -> String
inputTypeToString k ty =
    case ty of
        NamedType nm ->
            k nm

        ListType ts ->
            inputTypeToString (k << \ty -> "[" ++ ty ++ "]") ts

        NonNullType ty ->
            inputTypeToString (k << \ty -> ty ++ "!") ty
