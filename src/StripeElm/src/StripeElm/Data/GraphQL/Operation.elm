module Data.GraphQL.Operation exposing (OperationDefinition(..), toString, OperationAttr, attributes)

import Data.GraphQL.Variable as Variable exposing (VariableDefinition)
import Data.GraphQL.Directive as Directive exposing (Directive)
import Data.GraphQL.Selection as Selection exposing (Selection)


{-| ## Operations

    There are three types of operations that GraphQL models:
    - query &mdash a read‐only fetch.
    - mutation &mdash a write followed by a fetch.
    - subscription &mdash a long‐lived request that fetches data in response to source events.
-}
type OperationDefinition
    = Query OperationAttr
    | Mutation OperationAttr
    | Subscription OperationAttr


toString : OperationDefinition -> String
toString op =
    case op of
        Query attr ->
            operationAttrToString "query" attr

        Mutation attr ->
            operationAttrToString "mutation" attr

        Subscription attr ->
            operationAttrToString "subscription" attr


type alias OperationAttr =
    { name : Maybe String
    , variableDefinitions : List VariableDefinition
    , directives : List Directive
    , selectionSet : List Selection
    }


operationAttrToString :
    String
    -> OperationAttr
    -> String
operationAttrToString opName { name, variableDefinitions, directives, selectionSet } =
    let
        vblStr =
            case variableDefinitions of
                [] ->
                    Nothing

                _ ->
                    Just <|
                        String.join "," <|
                            List.map Variable.toString variableDefinitions

        dirStr =
            case directives of
                [] ->
                    Nothing

                _ ->
                    Just <| String.join " " <| List.map Directive.toString directives

        selectionStr =
            (\str ->
                "{"
                    ++ str
                    ++ "}"
            )
            <|
                String.join "," <|
                    List.map Selection.toString selectionSet
    in
        String.join " " <|
            List.filterMap identity
                [ Just opName
                , name
                , dirStr
                , Just selectionStr
                ]


attributes : OperationDefinition -> OperationAttr
attributes op =
    case op of
        Query attr ->
            attr

        Mutation attr ->
            attr

        Subscription attr ->
            attr
