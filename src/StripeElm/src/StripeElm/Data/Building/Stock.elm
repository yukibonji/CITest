module Data.Building.Stock exposing (Stock(..), decoder)

import Json.Encode as Encode exposing (Value, null)
import Json.Decode as Decode exposing (Decoder, succeed)
import Helpers exposing (camelCase, encodeDU, decodeDU)


type Stock
    = Stock


stockToString : Stock -> String
stockToString stock =
    case stock of
        Stock ->
            "stock"


stockFromString : String -> Result String Stock
stockFromString stock =
    case camelCase stock of
        "stock" ->
            Ok Stock

        tag ->
            Err <|
                "Unexpected tag `"
                    ++ tag
                    ++ "` for type `Stock`."



-- Serialization ---------------------------------------------------------------


encode : Stock -> Value
encode =
    encodeDU
        (stockToString >> (\tag -> ( tag, null )))
        tyName


decoder : Decoder Stock
decoder =
    decodeDU
        tyName
        (\tag ->
            case stockFromString tag of
                Ok tag ->
                    Just ( succeed (), \_ -> tag )

                _ ->
                    Nothing
        )


tyName : String
tyName =
    camelCase "Stock"
