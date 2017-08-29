module Data.Building.Density exposing (Density, encode, decoder)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Helpers exposing (resultToDecoder)


type Density
    = Empty
    | Low
    | Medium
    | High


toString : Density -> String
toString density =
    case density of
        Empty ->
            "Empty"

        Low ->
            "Low"

        Medium ->
            "Medium"

        High ->
            "High"


fromString : String -> Result String Density
fromString densityStr =
    case densityStr of
        "Empty" ->
            Ok Empty

        "Low" ->
            Ok Low

        "Medium" ->
            Ok Medium

        tag ->
            Err <|
                "Unexpected tag `"
                    ++ tag
                    ++ "` for type `Density`."


encode : Density -> Value
encode =
    toString >> Encode.string


decoder : Decoder Density
decoder =
    Decode.string
        |> Decode.andThen (fromString >> resultToDecoder)
