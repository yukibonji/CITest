module Data.Currency exposing (Currency, gbp, usd, chf, jpy, aud, cad, cny, encode, decoder)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Json.Encode.Extra as Encode
import Json.Decode.Pipeline as Pipeline exposing (decode, required, optional)
import Helpers exposing ((=>))


type alias Currency =
    { entity : String
    , currency : String
    , alphabeticCode : String
    , numericCode : String
    , minorUnit : Maybe Int
    }



-- Hard coded currencies -------------------------------------------------------


gbp =
    { entity = "UNITED KINGDOM OF GREAT BRITAIN AND NORTHERN IRELAND (THE)"
    , currency = "Pound Sterling"
    , alphabeticCode = "GBP"
    , numericCode = "826"
    , minorUnit = Just 2
    }


usd =
    { entity = "UNITED STATES OF AMERICA (THE)"
    , currency = "US Dollar"
    , alphabeticCode = "USD"
    , numericCode = "840"
    , minorUnit = Just 2
    }


aud =
    { entity = "AUSTRALIA"
    , currency = "Australian Dollar"
    , alphabeticCode = "AUD"
    , numericCode = "036"
    , minorUnit = Just 2
    }


cad =
    { entity = "CANADA"
    , currency = "Canadian Dollar"
    , alphabeticCode = "CAD"
    , numericCode = "124"
    , minorUnit = Just 2
    }


jpy =
    { entity = "JAPAN"
    , currency = "Yen"
    , alphabeticCode = "JPY"
    , numericCode = "392"
    , minorUnit = Just 0
    }


cny =
    { entity = "CHINA"
    , currency = "Yuan Renminbi"
    , alphabeticCode = "CNY"
    , numericCode = "156"
    , minorUnit = Just 2
    }


chf =
    { entity = "SWITZERLAND"
    , currency = "Swiss Franc"
    , alphabeticCode = "CHF"
    , numericCode = "756"
    , minorUnit = Just 2
    }



-- Serialization ---------------------------------------------------------------


encode : Currency -> Value
encode currency =
    Encode.object
        [ "entity" => Encode.string currency.entity
        , "currency" => Encode.string currency.currency
        , "alphabeticCode" => Encode.string currency.alphabeticCode
        , "numericCode" => Encode.string currency.numericCode
        , "minorUnit" => Encode.maybe Encode.int currency.minorUnit
        ]


decoder : Decoder Currency
decoder =
    decode Currency
        |> required "entity" Decode.string
        |> required "currency" Decode.string
        |> required "alphabeticCode" Decode.string
        |> required "numericCode" Decode.string
        |> optional "dependentLocality" (Decode.maybe Decode.int) Nothing
