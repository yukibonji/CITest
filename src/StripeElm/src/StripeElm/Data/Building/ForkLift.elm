module Data.Building.ForkLift exposing (ForkLift, encode, decoder)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Helpers exposing (resultToDecoder)


type ForkLift
    = Electrical
    | InternalCombustion
    | Manual


toJsonString : ForkLift -> String
toJsonString forkLift =
    case forkLift of
        Electrical ->
            "Electrical"

        InternalCombustion ->
            "InternalCombustion"

        Manual ->
            "Manual"


toString : ForkLift -> String
toString forkLift =
    case forkLift of
        Electrical ->
            "Electrical"

        InternalCombustion ->
            "Internal Combustion"

        Manual ->
            "Manual"


fromString : String -> Result String ForkLift
fromString forkLiftStr =
    case forkLiftStr of
        "Electrical" ->
            Ok Electrical

        "InternalCombustion" ->
            Ok InternalCombustion

        "Manual" ->
            Ok Manual

        tag ->
            Err <|
                "Unexpected tag `"
                    ++ tag
                    ++ "` for type `ForkLift`."


encode : ForkLift -> Value
encode =
    toJsonString >> Encode.string


decoder : Decoder ForkLift
decoder =
    Decode.string
        |> Decode.andThen (fromString >> resultToDecoder)
