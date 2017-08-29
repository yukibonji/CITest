module Data.Response exposing (Response(..), encode, decoder)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Helpers exposing ((=>))


type Response a
    = Answered a
    | Unknown
    | Skip
    | NotAsked


toMaybe : Response a -> Maybe a
toMaybe response =
    case response of
        Answered a ->
            Just a

        _ ->
            Nothing


encode : (a -> Value) -> Response a -> Value
encode encodeAnswer response =
    case response of
        Answered a ->
            Encode.object
                [ "Answered" => encodeAnswer a ]

        Unknown ->
            Encode.string "Unknown"

        Skip ->
            Encode.string "Skip"

        NotAsked ->
            Encode.string "NotAsked"


decoder : Decoder a -> Decoder (Response a)
decoder decodeAnswer =
    Decode.oneOf
        [ Decode.map Answered <| Decode.field "Answered" decodeAnswer
        , Decode.andThen
            (\str ->
                case str of
                    "Unknown" ->
                        Decode.succeed Unknown

                    "Skip" ->
                        Decode.succeed Skip

                    "NotAsked" ->
                        Decode.succeed NotAsked

                    _ ->
                        Decode.fail "str"
            )
          <|
            Decode.string
        ]
