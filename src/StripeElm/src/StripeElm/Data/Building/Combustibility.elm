module Data.Building.Combustibility exposing (Combustibility, encode, decoder)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Helpers exposing (resultToDecoder)


type Combustibility
    = FR
    | NC
    | C


toJsonString : Combustibility -> String
toJsonString combustibility =
    case combustibility of
        FR ->
            "FR"

        NC ->
            "NC"

        C ->
            "C"


toString : Combustibility -> String
toString combustibility =
    case combustibility of
        FR ->
            "Fire Resistant"

        NC ->
            "Noncombustible"

        C ->
            "Combustible"


fromString : String -> Result String Combustibility
fromString combustibilityStr =
    case combustibilityStr of
        "FR" ->
            Ok FR

        "NC" ->
            Ok NC

        "C" ->
            Ok C

        tag ->
            Err <|
                "Unexpected tag `"
                    ++ tag
                    ++ "` for type `Combustibility`."


encode : Combustibility -> Value
encode =
    toJsonString >> Encode.string


decoder : Decoder Combustibility
decoder =
    Decode.string
        |> Decode.andThen (fromString >> resultToDecoder)
