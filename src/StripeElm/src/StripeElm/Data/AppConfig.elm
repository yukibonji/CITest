module Data.AppConfig exposing (AppConfig, default, decoder)

import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline as Pipeline exposing (decode, required)


{-|
    Top-level configuration
-}
type alias AppConfig =
    { apiProtocol : String
    , apiHost : String
    , apiPort : Int
    , apiVersion : Int
    }


default : AppConfig
default =
    AppConfig "http" "127.0.0.1" 8081 1


decoder : Decoder AppConfig
decoder =
    decode AppConfig
        |> required "apiProtocol" Decode.string
        |> required "apiHost" Decode.string
        |> required "apiPort" Decode.int
        |> required "apiVersion" Decode.int
