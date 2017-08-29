module Data.Building.DesignCode exposing (DesignCode, encode, decoder)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Helpers exposing (resultToDecoder)


type DesignCode
    = NFPA
    | FM
    | VDS
    | APSAD
    | CEA4001
    | EN12845
    | ASNZ2118
    | LPCorEquivalent
    | OtherLocalStandard
    | NoStandard


toJsonString : DesignCode -> String
toJsonString designCode =
    case designCode of
        NFPA ->
            "NFPA"

        FM ->
            "FM"

        VDS ->
            "VDS"

        APSAD ->
            "APSAD"

        CEA4001 ->
            "CEA4001"

        EN12845 ->
            "EN12845"

        ASNZ2118 ->
            "ASNZ2118"

        LPCorEquivalent ->
            "LPCorEquivalent"

        OtherLocalStandard ->
            "OtherLocalStandard"

        NoStandard ->
            "NoStandard"


toString : DesignCode -> String
toString designCode =
    case designCode of
        NFPA ->
            "NFPA"

        FM ->
            "FM"

        VDS ->
            "VDS"

        APSAD ->
            "APSAD"

        CEA4001 ->
            "CEA4001"

        EN12845 ->
            "EN12845"

        ASNZ2118 ->
            "ASNZ2118"

        LPCorEquivalent ->
            "LPC or equivalent"

        OtherLocalStandard ->
            "Other local standard"

        NoStandard ->
            "No standard"


fromString : String -> Result String DesignCode
fromString designCodeStr =
    case designCodeStr of
        "NFPA" ->
            Ok NFPA

        "FM" ->
            Ok FM

        "VDS" ->
            Ok VDS

        "APSAD" ->
            Ok APSAD

        "CEA4001" ->
            Ok CEA4001

        "EN12845" ->
            Ok EN12845

        "ASNZ2118" ->
            Ok ASNZ2118

        "LPCorEquivalent" ->
            Ok LPCorEquivalent

        "OtherLocalStandard" ->
            Ok OtherLocalStandard

        "NoStandard" ->
            Ok NoStandard

        tag ->
            Err <|
                "Unexpected tag `"
                    ++ tag
                    ++ "` for type `DesignCode`."


encode : DesignCode -> Value
encode =
    toJsonString >> Encode.string


decoder : Decoder DesignCode
decoder =
    Decode.string
        |> Decode.andThen (fromString >> resultToDecoder)
