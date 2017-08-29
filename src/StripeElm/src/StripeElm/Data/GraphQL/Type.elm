module Data.GraphQL.Type exposing (..)

import Data.GraphQL.Operation exposing (OperationDefinition)
import Data.GraphQL.Variable exposing (InputType)
import Data.GraphQL.Value exposing (Value)


type TypeDefinition
    = ScalarTypeDefn String
    | ObjectTypeDefn ObjectTypeDefinition
    | InterfaceTypeDefn InterfaceTypeDefinition
    | UnionTypeDefn UnionTypeDefinition
    | EnumTypeDefn EnumTypeDefinition
    | InputObjectTypeDefn InputObjectTypeDefinition


type alias ObjectTypeDefinition =
    { name : String
    , interfaces : List String
    , fields : List FieldDefinition
    }


type alias FieldDefinition =
    { name : String
    , arguments : List InputValueDefinition
    , type_ : InputType
    }


type alias InputValueDefinition =
    { name : String
    , type_ : InputType
    , defaultValue : Maybe Value
    }


type alias OperationTypeDefinition =
    { type_ : String
    , operation : OperationDefinition
    }


type alias SchemaDefintion =
    { operationTypes : OperationTypeDefinition
    }


type alias InterfaceTypeDefinition =
    { name : String
    , fields : List FieldDefinition
    }


type alias UnionTypeDefinition =
    { name : String
    , types : List String
    }


type alias EnumTypeDefinition =
    { name : String
    , values : List String
    }


type alias InputObjectTypeDefinition =
    { name : String
    , fields : List InputValueDefinition
    }
