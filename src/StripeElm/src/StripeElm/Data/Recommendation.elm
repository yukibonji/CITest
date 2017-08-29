module Data.Recommendation
    exposing
        ( Recommendation
        , encode
        , decoder
        , Resolution(..)
        , resolutionToString
        , resolutionFromString
        , encodeResolution
        , resolutionDecoder
        )

import Json.Encode as Encode exposing (Value, null)
import Json.Decode as Decode exposing (Decoder, succeed)
import Json.Decode.Pipeline exposing (decode, required)
import Helpers exposing (encodeDU, decodeDU, (=>), camelCase)
import Data.Issue as Issue exposing (Issue)


type alias Recommendation =
    { issue : Issue
    , resolution : Resolution
    }


type Resolution
    = FollowUp
    | Closed



-- Serialization ---------------------------------------------------------------


encode : Recommendation -> Value
encode { issue, resolution } =
    Encode.object
        [ "issue" => Issue.encode issue
        , "resolution" => encodeResolution resolution
        ]


decoder : Decoder Recommendation
decoder =
    decode Recommendation
        |> required "issue" Issue.decoder
        |> required "resolution" resolutionDecoder


encodeResolution : Resolution -> Value
encodeResolution =
    encodeDU
        (resolutionToString >> (\tag -> ( tag, null )))
        "resolution"


resolutionDecoder : Decoder Resolution
resolutionDecoder =
    decodeDU
        "resolution"
        (\tag ->
            case resolutionFromString tag of
                Ok tag ->
                    Just ( succeed (), \_ -> tag )

                _ ->
                    Nothing
        )



-- Helpers ---------------------------------------------------------------------


resolutionToString : Resolution -> String
resolutionToString issueType =
    case issueType of
        FollowUp ->
            "followUp"

        Closed ->
            "closed"


resolutionFromString : String -> Result String Resolution
resolutionFromString resolutionStr =
    case camelCase resolutionStr of
        "followUp" ->
            Ok FollowUp

        "closed" ->
            Ok Closed

        tag ->
            Err <|
                "Unexpected tag `"
                    ++ tag
                    ++ "` for type `Resolution`."
