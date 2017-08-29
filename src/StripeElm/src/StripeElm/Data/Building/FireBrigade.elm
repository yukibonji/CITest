module Data.Building.FireBrigade exposing (FireBrigade, encode, decoder)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Helpers exposing (resultToDecoder)


type FireBrigade
    = Professional
    | OnSite
    | Volunteer


toJsonString : FireBrigade -> String
toJsonString fireBrigade =
    case fireBrigade of
        Professional ->
            "Professional"

        OnSite ->
            "OnSite"

        Volunteer ->
            "Volunteer"


toString : FireBrigade -> String
toString fireBrigade =
    case fireBrigade of
        Professional ->
            "Professional"

        OnSite ->
            "On-site"

        Volunteer ->
            "Volunteer"


fromString : String -> Result String FireBrigade
fromString fireBrigadeStr =
    case fireBrigadeStr of
        "Professional" ->
            Ok Professional

        "OnSite" ->
            Ok OnSite

        "Volunteer" ->
            Ok Volunteer

        tag ->
            Err <|
                "Unexpected tag `"
                    ++ tag
                    ++ "` for type `FireBrigade`."


encode : FireBrigade -> Value
encode =
    toJsonString >> Encode.string


decoder : Decoder FireBrigade
decoder =
    Decode.string
        |> Decode.andThen (fromString >> resultToDecoder)
