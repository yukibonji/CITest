module Data.AuthToken
    exposing
        ( AuthToken
        , encode
        , decoder
        , withAuthorization
        )

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import HttpBuilder exposing (withHeader, RequestBuilder)
import Helpers exposing (maybe)


type AuthToken
    = AuthToken String


encode : AuthToken -> Value
encode (AuthToken token) =
    Encode.string token


decoder : Decoder AuthToken
decoder =
    Decode.map AuthToken <|
        Decode.string


withAuthorization :
    Maybe AuthToken
    -> RequestBuilder a
    -> RequestBuilder a
withAuthorization maybeToken builder =
    maybe
        builder
        (\(AuthToken token) ->
            withHeader "authorization" ("Token " ++ token) builder
        )
        maybeToken
