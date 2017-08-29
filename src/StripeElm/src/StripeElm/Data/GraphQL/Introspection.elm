module Data.GraphQL.Introspection
    exposing
        ( IntrospectionSchema
        , IntrospectionType
        , IntrospectionField
        , IntrospectionInputVal
        , IntrospectionTypeRef
        , IntrospectionEnumVal
        , IntrospectionDirective
        , DirectiveLocation
        , Kind
        )


type alias IntrospectionSchema =
    { --/// Introspection reference to schema's query root.
      queryType :
        IntrospectionTypeRef
        --/// Introspection reference to schema's mutation root.
    , mutationType :
        Maybe IntrospectionTypeRef
        --/// Introspection reference to schema's subscription root.
    , subscriptionType :
        Maybe IntrospectionTypeRef
        -- /// Array of all introspection types defined within current schema.
        -- /// Includes types for queries, mutations and subscriptions.
    , types :
        List IntrospectionType
        -- /// Array of all directives supported by current schema.
    , directives : List IntrospectionDirective
    }


type alias IntrospectionType =
    { -- Which kind category current type belongs to.
      kind :
        Kind
        -- Type name. Must be unique in scope of the defined schema.
    , name :
        String
        -- Optional type description.
    , description :
        Maybe String
        -- /// Array of field descriptors defined within current type.
        -- /// Only present for Object and Interface types.
    , fields :
        Maybe (List IntrospectionField)
        -- /// Array of interfaces implemented by output object type defintion.
    , interfaces :
        Maybe (List IntrospectionTypeRef)
        -- /// Array of type references being possible implementation of current type.
        -- /// Only present for Union types (list of union cases) and Interface types
        --/// (list of all objects implementing interface in scope of the schema).
    , possibleTypes :
        Maybe (List IntrospectionTypeRef)
        --/// Array of enum values defined by current Enum type.
    , enumValues :
        Maybe (List IntrospectionEnumVal)
        --/// Array of input fields defined by current InputObject type.
    , inputFields :
        Maybe (List IntrospectionInputVal)
        --/// Type param reference - used only by List and NonNull types.
    , ofType : Maybe IntrospectionTypeRef
    }


type alias IntrospectionField =
    { --/// Field name. Must be unique in scope of the definin object/interface.
      name :
        String
        --/// Optional field description.
    , description :
        Maybe String
        --/// Array of field arguments. In GraphQL fields can be parametrized,
        --/// working effectively like methods.
    , args :
        List IntrospectionInputVal
        --/// Introspection reference to field's type.
    , type_ :
        IntrospectionTypeRef
        --/// If true, marks current field as deprecated, but still
        --/// available for compatibility reasons.
    , isDeprecated :
        Bool
        --/// If field is deprecated here a deprecation reason may be set.
    , deprecationReason : Maybe String
    }


type alias IntrospectionInputVal =
    { --/// Input argument name.
      name :
        String
        -- /// Optional input argument description.
    , description :
        Maybe String
        --/// Introspection reference to argument's type.
    , type_ :
        IntrospectionTypeRef
        -- /// Default arguments value, if provided.
    , defaultValue : Maybe String
    }


{-| Introspection type reference. Used to navigate between type dependencies inside introspected schema.
-}
type IntrospectionTypeRef
    = IntrospectionTypeRef
        { --/// Referenced type kind.
          kind :
            Kind
            --/// Type name. None if referenced type is List or NonNull.
        , name :
            Maybe String
            --/// Optional type description.
        , description :
            Maybe String
            --/// Type param reference. Used only by List and NonNull types.
        , ofType : Maybe IntrospectionTypeRef
        }


type alias IntrospectionEnumVal =
    { -- Enum value name - must be unique in scope of defining enum.
      name :
        String
        --/// Optional enum value description.
    , description :
        Maybe String
        --/// If true, marks current value as deprecated, but still
        -- /// available for compatibility reasons.
    , isDeprecated :
        Bool
        -- /// If value is deprecated this field may describe a deprecation reason.
    , deprecationReason : Maybe String
    }


type alias IntrospectionDirective =
    { --/// Directive name.
      name :
        String
        -- /// Description of a target directive.
    , description :
        Maybe String
        --/// Array of AST locations, where it's valid to place target directive.
    , locations :
        List DirectiveLocation
        --/// Array of arguments, current directive can be parametrized with.
    , args : List IntrospectionInputVal
    }


type DirectiveLocation
    = Query
    | Mutation
    | Subscription
    | Field
    | FragmentDefinition
    | FragmentSpread
    | InlineFragment


type Kind
    = Scalar
    | Object
    | Interface
    | Union
    | Enum
    | InputObject
    | List
    | NonNull
