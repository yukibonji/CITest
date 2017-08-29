module Data.Building.QualitativeRating exposing (QualitativeRating, encode, decoder)

import Json.Encode as Encode exposing (Value, null)
import Json.Decode as Decode exposing (Decoder, succeed)
import Helpers exposing (resultToDecoder)


type QualitativeRating
    = Poor
    | Fair
    | Good
    | Excellent


toString : QualitativeRating -> String
toString rating =
    case rating of
        Poor ->
            "Poor"

        Fair ->
            "Fair"

        Good ->
            "Good"

        Excellent ->
            "Excellent"


fromString : String -> Result String QualitativeRating
fromString rating =
    case rating of
        "Poor" ->
            Ok Poor

        "Fair" ->
            Ok Fair

        "Good" ->
            Ok Good

        "Excellent" ->
            Ok Excellent

        tag ->
            Err <|
                "Unexpected tag `"
                    ++ tag
                    ++ "` for type `QualitativeRating`."



-- Serialization ---------------------------------------------------------------


encode : QualitativeRating -> Value
encode =
    toString >> Encode.string


decoder : Decoder QualitativeRating
decoder =
    Decode.string
        |> Decode.andThen
            (fromString >> resultToDecoder)
