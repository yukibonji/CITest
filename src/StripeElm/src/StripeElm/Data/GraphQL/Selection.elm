module Data.GraphQL.Selection
    exposing
        ( Selection(..)
        , toString
        , directives
        , Field
        , withSelection
        , aliasOrName
        , FragmentDefinition
        , fragmentDefinitionToString
        , TypeCondition
        , FragmentSpread
        )

import Data.GraphQL.Directive as Directive exposing (Directive)
import Data.GraphQL.Argument as Argument exposing (Argument)


{-|
    ## Selection sets

    An operation selects the set of information it needs, and will receive
    exactly that information and nothing more, avoiding over‐fetching and
    under‐fetching data.
-}
type Selection
    = FieldSelection Field
    | FragmentSpreadSelection FragmentSpread
    | InlineFragment FragmentDefinition


directives : Selection -> List Directive
directives selection =
    case selection of
        FieldSelection field ->
            field.directives

        FragmentSpreadSelection spread ->
            spread.directives

        InlineFragment frag ->
            frag.directives


toString : Selection -> String
toString selection =
    case selection of
        FieldSelection field ->
            fieldToString field

        FragmentSpreadSelection frag ->
            fragmentSpreadToString frag

        InlineFragment frag ->
            fragmentDefinitionToString frag


{-|
    ## Fields

    A selection set is primarily composed of fields. A field describes one
    discrete piece of information available to request within a selection set.

    Some fields describe complex data or relationships to other data. In order
    to further explore this data, a field may itself contain a selection set,
    allowing for deeply nested requests. All GraphQL operations must specify
    their selections down to fields which return scalar values to ensure an
    unambiguously shaped response.

    For example, this operation selects fields of complex data and relationships
    down to scalar values.

    ```
        {
          me {
            id
            firstName
            lastName
            birthday {
              month
              day
            }
            friends {
              name
            }
          }
        }
    ```
    Fields in the top‐level selection set of an operation often represent some
    information that is globally accessible to your application and its
    current viewer. Some typical examples of these top fields include references
    to a current logged‐in viewer, or accessing certain types of data referenced
    by a unique identifier.

    ```
    # `me` could represent the currently logged in viewer.
    {
        me {
            name
        }
    }

    # `user` represents one of many users in a graph of data, referred to by a
    # unique identifier.
    {
        user(id: 4) {
            name
        }
    }
    ```
-}
type alias Field =
    { alias : Maybe String
    , name : String
    , arguments : List Argument
    , directives : List Directive
    , selectionSet : List Selection
    }


withSelection : Selection -> Field -> Field
withSelection selection field =
    { field | selectionSet = selection :: field.selectionSet }


fieldToString : Field -> String
fieldToString { alias, name, arguments, directives, selectionSet } =
    let
        aliasStr =
            case alias of
                Just nm ->
                    Just (nm ++ ":")

                _ ->
                    Nothing

        selectionStr =
            case selectionSet of
                [] ->
                    Nothing

                _ ->
                    Just <|
                        "{"
                            ++ (String.join "," <| List.map toString selectionSet)
                            ++ "}"

        argStr =
            case arguments of
                [] ->
                    Nothing

                _ ->
                    Just <|
                        "("
                            ++ (String.join "," <| List.map Argument.toString arguments)
                            ++ ")"

        dirStr =
            Just <| String.join " " <| List.map Directive.toString directives
    in
        String.join "" <|
            List.filterMap identity
                [ aliasStr
                , Just name
                , argStr
                , dirStr
                , selectionStr
                ]


aliasOrName : Field -> String
aliasOrName { alias, name } =
    case alias of
        Just x ->
            x

        _ ->
            name


{-|
    ## FragmentDefinition

    Fragments are the primary unit of composition in GraphQL.

    Fragments allow for the reuse of common repeated selections of fields,
    reducing duplicated text in the document.

-}
type alias FragmentDefinition =
    { name : Maybe String
    , typeCondition : Maybe TypeCondition
    , directives : List Directive
    , selectionSet : List Selection
    }


fragmentDefinitionToString : FragmentDefinition -> String
fragmentDefinitionToString { name, typeCondition, directives, selectionSet } =
    let
        condStr =
            Maybe.map typeConditionToString typeCondition

        dirStr =
            Just <| String.join " " <| List.map Directive.toString directives

        selectionStr =
            case selectionSet of
                [] ->
                    Nothing

                _ ->
                    Just <|
                        "{"
                            ++ (String.join "," <| List.map toString selectionSet)
                            ++ "}"
    in
        String.join " " <|
            List.filterMap identity
                [ Just "fragment"
                , name
                , condStr
                , dirStr
                , selectionStr
                ]


{-|
    ## Type condition
    Fragments must specify the type they apply to.
-}
type TypeCondition
    = On String


typeConditionToString : TypeCondition -> String
typeConditionToString cond =
    case cond of
        On tyName ->
            "on " ++ tyName


{-|
    ## FragmentSpread

    Fragments can be defined inline within a selection set.
    This is done to conditionally include fields based on their runtime type.
-}
type alias FragmentSpread =
    { name : String
    , directives : List Directive
    }


fragmentSpreadToString : FragmentSpread -> String
fragmentSpreadToString { name, directives } =
    String.join " "
        [ "..." ++ name
        , String.join " " <| List.map Directive.toString directives
        ]
