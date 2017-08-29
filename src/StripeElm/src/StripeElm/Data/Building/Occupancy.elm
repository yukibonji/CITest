module Data.Building.Occupancy exposing (Occupancy(..), occupancyParser, toString, fromString, encode, decoder)

import String
import Json.Encode as Encode exposing (Value, null)
import Json.Decode as Decode exposing (Decoder, succeed)
import UrlParser exposing (Parser)
import Helpers exposing (resultToDecoder)


type Occupancy
    = Warehouse


occupancyParser : Parser (Occupancy -> b) b
occupancyParser =
    UrlParser.custom "OCCUPANCY" <|
        \occupanccyStr ->
            case String.toLower occupanccyStr of
                "warehouse" ->
                    Ok Warehouse

                _ ->
                    Err <|
                        "Unexpected tag `"
                            ++ occupanccyStr
                            ++ "` for type `Occupancy`."


toString : Occupancy -> String
toString occupancy =
    case occupancy of
        Warehouse ->
            "Warehouse"


fromString : String -> Result String Occupancy
fromString occupancy =
    case occupancy of
        "Warehouse" ->
            Ok Warehouse

        tag ->
            Err <|
                "Unexpected tag `"
                    ++ tag
                    ++ "` for type `Occupancy`."



-- Serialization ---------------------------------------------------------------


encode : Occupancy -> Value
encode =
    toString >> Encode.string


decoder : Decoder Occupancy
decoder =
    Decode.string
        |> Decode.andThen (fromString >> resultToDecoder)
