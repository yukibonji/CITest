module Data.Building.Ownership
    exposing
        ( Ownership
        , ownershipToString
        , ownershipFromString
        , encode
        , decoder
        )

import Json.Encode as Encode exposing (Value, null)
import Json.Decode as Decode exposing (Decoder, succeed)
import Helpers exposing (resultToDecoder, camelCase)


type Ownership
    = Owned
    | OwnedOccupant
    | Leased
    | Tennant


ownershipToString : Ownership -> String
ownershipToString ownership =
    case ownership of
        Owned ->
            "Owned"

        OwnedOccupant ->
            "OwnedOccupant"

        Leased ->
            "Leased"

        Tennant ->
            "Tennant"


ownershipFromString : String -> Result String Ownership
ownershipFromString ownershipStr =
    case ownershipStr of
        "Owned" ->
            Ok Owned

        "OwnedOccupant" ->
            Ok OwnedOccupant

        "Leased" ->
            Ok Leased

        "Tennant" ->
            Ok Tennant

        tag ->
            Err <|
                "Unexpected tag `"
                    ++ tag
                    ++ "` for type `Ownership`."



-- Serialization ---------------------------------------------------------------


encode : Ownership -> Value
encode =
    ownershipToString >> Encode.string


decoder : Decoder Ownership
decoder =
    Decode.string
        |> Decode.andThen
            (ownershipFromString >> resultToDecoder)
