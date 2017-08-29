module Data.GraphQL.Argument exposing (Argument, toString)

import Data.GraphQL.Value as Value exposing (Value)


{-|
    ## Arguments

    Fields are conceptually functions which return values, and occasionally
    accept arguments which alter their behavior. These arguments often map
    directly to function arguments within a GraphQL server’s implementation.

-}
type alias Argument =
    { name : String
    , value : Value
    }


toString : Argument -> String
toString { name, value } =
    name ++ ": " ++ Value.toString value
