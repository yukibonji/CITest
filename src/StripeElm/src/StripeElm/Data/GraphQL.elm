module Data.GraphQL exposing (simpleQuery, query, subscription, scalarField, simpleField, field, aliasedField)

import Data.GraphQL.Document exposing (Document, Definition(Operation, Fragment))
import Data.GraphQL.Operation exposing (OperationDefinition(..), OperationAttr)
import Data.GraphQL.Selection exposing (Selection(..), Field)
import Data.GraphQL.Variable exposing (VariableDefinition)
import Data.GraphQL.Directive exposing (Directive)
import Data.GraphQL.Argument exposing (Argument)


-- Top-level document helpers


{-| simple ananymous query
-}
simpleQuery : List Selection -> Definition
simpleQuery =
    Operation
        << Query
        << OperationAttr Nothing [] []


query :
    Maybe String
    -> List VariableDefinition
    -> List Directive
    -> List Selection
    -> Definition
query name vbls dirs selections =
    Operation <|
        Query <|
            OperationAttr name vbls dirs selections


subscription :
    Maybe String
    -> List VariableDefinition
    -> List Directive
    -> List Selection
    -> Definition
subscription name vbls dirs selections =
    Operation <|
        Subscription <|
            OperationAttr name vbls dirs selections



-- Field helpers ---------------------------------------------------------------


{-| Leaf level selection returning a scalar value
-}
scalarField : String -> Selection
scalarField name =
    FieldSelection <|
        Field
            Nothing
            name
            []
            []
            []


{-| Selection with no arguments
-}
simpleField : String -> List Selection -> Selection
simpleField name selections =
    FieldSelection <|
        Field
            Nothing
            name
            []
            []
            selections


{-| Commonly used un-aliased field
-}
field :
    String
    -> List Argument
    -> List Selection
    -> Selection
field name args selections =
    FieldSelection <|
        Field
            Nothing
            name
            args
            []
            selections


{-| Aliased field
-}
aliasedField :
    String
    -> String
    -> List Argument
    -> List Selection
    -> Selection
aliasedField alias name args selections =
    FieldSelection <|
        Field
            (Just alias)
            name
            args
            []
            selections
