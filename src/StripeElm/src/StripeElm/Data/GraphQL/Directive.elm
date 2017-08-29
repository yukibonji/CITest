module Data.GraphQL.Directive exposing (Directive, toString, if_)

import Data.GraphQL.Argument as Argument exposing (Argument)


{-|
    ## Directive

    Directives provide a way to describe alternate runtime execution and type
    validation behavior in a GraphQL document.

    In some cases, you need to provide options to alter GraphQL’s execution
    behavior in ways field arguments will not suffice, such as conditionally
    including or skipping a field. Directives provide this by describing
    additional information to the executor.

    Directives have a name along with a list of arguments which may accept
    values of any input type.

    Directives can be used to describe additional information for types, fields,
    fragments and operations.

    As future versions of GraphQL adopt new configurable execution capabilities,
    they may be exposed via directives.
-}
type alias Directive =
    { name : String
    , arguments : List Argument
    }


toString : Directive -> String
toString { name, arguments } =
    let
        args =
            case arguments of
                [] ->
                    ""

                _ ->
                    "("
                        ++ (String.join "," <| List.map Argument.toString arguments)
                        ++ ")"
    in
        "@" ++ name ++ args


if_ : Directive -> Maybe Argument
if_ { arguments } =
    case List.filter (\arg -> arg.name == "if") arguments of
        x :: _ ->
            Just x

        _ ->
            Nothing
