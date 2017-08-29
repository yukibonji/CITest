module Data.GraphQL.Value exposing (toString, Value(..))

import Dict exposing (Dict)
import String


{-|
    ## Values

    Field and directive arguments accept input values of various literal
    primitives; input values can be scalars, enumeration values, lists,
    or input objects.
-}
type Value
    = IntValue Int
    | FloatValue Float
    | BooleanValue Bool
    | StringValue String
    | EnumValue String
    | ListValue (List Value)
    | ObjectValue (Dict String Value)
    | Variable String
    | NullValue


toString : Value -> String
toString value =
    case value of
        NullValue ->
            "null"

        Variable v ->
            "$" ++ v

        IntValue i ->
            Basics.toString i

        FloatValue f ->
            Basics.toString f

        BooleanValue b ->
            String.toLower <|
                Basics.toString b

        StringValue s ->
            Basics.toString s

        EnumValue e ->
            e

        ListValue xs ->
            "["
                ++ (List.map toString xs
                        |> String.join ","
                   )
                ++ "]"

        ObjectValue xs ->
            "{"
                ++ (String.join "," <|
                        List.map (\( lbl, val ) -> lbl ++ ":" ++ toString val) <|
                            Dict.toList xs
                   )
                ++ "}"
