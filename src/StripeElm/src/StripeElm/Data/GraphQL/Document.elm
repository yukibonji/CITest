module Data.GraphQL.Document exposing (Document, Definition(..), toString, name)

import Data.GraphQL.Operation as Operation exposing (OperationDefinition, attributes)
import Data.GraphQL.Selection exposing (FragmentDefinition, fragmentDefinitionToString)


{-|
    ## Query Document

    A GraphQL query document describes a complete file or request string
    received by a GraphQL service. A document contains multiple definitions
    of Operations and Fragments. GraphQL query documents are only executable by
    a server if they contain an operation. However documents which do not
    contain operations may still be parsed and validated to allow client to
    represent a single request across many documents.

    If a document contains only one operation, that operation may be unnamed or
    represented in the shorthand form, which omits both the query keyword and
    operation name. Otherwise, if a GraphQL query document contains multiple
    operations, each operation must be named. When submitting a query document
    with multiple operations to a GraphQL service, the name of the desired
    operation to be executed must also be provided.
-}
type alias Document =
    { definitions : List Definition
    }


toString : Document -> String
toString { definitions } =
    String.join "\n" <|
        List.map definitionToString definitions


{-| ## Definition
-}
type Definition
    = Operation OperationDefinition
    | Fragment FragmentDefinition


name : Definition -> Maybe String
name defn =
    case defn of
        Operation op ->
            (.name << attributes) op

        Fragment frag ->
            frag.name


definitionToString : Definition -> String
definitionToString defn =
    case defn of
        Operation op ->
            Operation.toString op

        Fragment frag ->
            fragmentDefinitionToString frag
