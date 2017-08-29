module Data.Range exposing (Range, Endpoint(..), empty, contains, isEmpty, update, encode, decoder)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline exposing (decode, required)
import Helpers exposing ((=>), maybe)


type alias Range t =
    { upper : Maybe t
    , lower : Maybe t
    }


type Endpoint
    = Upper
    | Lower



-- Helpers ---------------------------------------------------------------------


empty : Range t
empty =
    { lower = Nothing
    , upper = Nothing
    }


contains :
    Range comparable
    -> comparable
    -> Bool
contains range x =
    Maybe.withDefault True <|
        Maybe.map2
            (\l u -> x >= l && x <= u)
            range.lower
            range.upper


isEmpty : Range a -> Bool
isEmpty { upper, lower } =
    case ( upper, lower ) of
        ( Nothing, Nothing ) ->
            True

        _ ->
            False


update :
    Endpoint
    -> a
    -> Range a
    -> Range a
update endpoint value range =
    case endpoint of
        Lower ->
            { range | lower = Just value }

        _ ->
            { range | upper = Just value }



-- Serialization ---------------------------------------------------------------


encode : (t -> Value) -> Range t -> Value
encode encodeT { upper, lower } =
    Encode.object
        [ "upper" => maybe Encode.null encodeT upper
        , "lower" => maybe Encode.null encodeT lower
        ]


decoder : Decoder t -> Decoder (Range t)
decoder decodeT =
    decode Range
        |> required "upper" (Decode.maybe decodeT)
        |> required "lower" (Decode.maybe decodeT)
