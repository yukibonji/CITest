module Data.LocationFilter exposing (LocationFilter, empty, encode, encodeCompact, decoder)

import Date exposing (Date)
import Json.Encode as Encode exposing (Value)
import Json.Encode.Extra as Encode
import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline as Pipeline exposing (required, optional, decode)
import Helpers exposing ((=>), encodeDate, decodeDate, encodeTuple2, decodeTuple2)
import Data.Coordinate as Coordinate exposing (Coordinate)
import Data.Country as Country exposing (Country)
import Data.Currency as Currency exposing (Currency)
import Data.Building.Occupancy as Occupancy exposing (Occupancy)
import Data.Building.Ownership as Ownership exposing (Ownership)
import Data.Building.QualitativeRating as QualitativeRating exposing (QualitativeRating)
import Data.Range as Range exposing (Range)


type alias LocationFilter =
    { country : List Country
    , searchString : Maybe String
    , searchArea : Maybe ( Coordinate, Float )
    , ownership : List Ownership
    , occupancy : List Occupancy
    , totalArea : Range Float
    , totalInsuredValue : ( Currency, Range Float )
    , siteCondition : List QualitativeRating
    , plantLayout : List QualitativeRating
    , lastSurveyDate : Maybe Date
    , favouritesOnly : Bool
    }


empty : LocationFilter
empty =
    { country = []
    , searchString = Nothing
    , searchArea = Nothing
    , ownership = []
    , occupancy = []
    , totalArea = Range.empty
    , totalInsuredValue = ( Currency.usd, Range.empty )
    , siteCondition = []
    , plantLayout = []
    , lastSurveyDate = Nothing
    , favouritesOnly = False
    }



-- Serialization ---------------------------------------------------------------


encodeHelper : a -> (b -> Value) -> List b -> Maybe ( a, Value )
encodeHelper label encoder list =
    case list of
        [] ->
            Nothing

        _ ->
            List.map encoder list
                |> \v ->
                    label
                        => Encode.list v
                        |> Just



-- Compact encoder for client side URL


encodeCompact : LocationFilter -> Value
encodeCompact locationSearch =
    Encode.object <|
        List.filterMap identity
            [ encodeHelper "country" Country.encode locationSearch.country
            , Maybe.map
                (\v -> "searchString" => Encode.string v)
                locationSearch.searchString
            , Maybe.map
                (\v -> "searchArea" => encodeTuple2 Coordinate.encode Encode.float v)
                locationSearch.searchArea
            , encodeHelper "ownership" Ownership.encode locationSearch.ownership
            , encodeHelper "occupancy" Occupancy.encode locationSearch.occupancy
            , if Range.isEmpty locationSearch.totalArea then
                Nothing
              else
                Just <|
                    ( "totalArea"
                    , Range.encode Encode.float locationSearch.totalArea
                    )
            , if Range.isEmpty <| Tuple.second locationSearch.totalInsuredValue then
                Nothing
              else
                Just <|
                    ( "totalInsuredValue"
                    , encodeTuple2
                        Currency.encode
                        (Range.encode Encode.float)
                        locationSearch.totalInsuredValue
                    )
            , encodeHelper "siteCondition" QualitativeRating.encode locationSearch.siteCondition
            , encodeHelper "plantLayout" QualitativeRating.encode locationSearch.plantLayout
            , Maybe.map (\d -> "lastSurveyDate" => encodeDate d) locationSearch.lastSurveyDate
            , Just ("favouritesOnly" => Encode.bool locationSearch.favouritesOnly)
            ]


encode : LocationFilter -> Value
encode filter =
    Encode.object <|
        [ "country" => (Encode.list <| List.map Country.encode filter.country)
        , "searchString" => (Encode.maybe Encode.string filter.searchString)
        , "searchArea" => (Encode.maybe (encodeTuple2 Coordinate.encode Encode.float) filter.searchArea)
        , "ownership" => (Encode.list <| List.map Ownership.encode filter.ownership)
        , "occupancy" => (Encode.list <| List.map Occupancy.encode filter.occupancy)
        , "totalArea" => Range.encode Encode.float filter.totalArea
        , "totalInsuredValue" => encodeTuple2 Currency.encode (Range.encode Encode.float) filter.totalInsuredValue
        , "siteCondition" => (Encode.list <| List.map QualitativeRating.encode filter.siteCondition)
        , "plantLayout" => (Encode.list <| List.map QualitativeRating.encode filter.plantLayout)
        , "lastSurveyDate" => Encode.maybe encodeDate filter.lastSurveyDate
        , "favouritesOnly" => Encode.bool filter.favouritesOnly
        ]


decoder : Decoder LocationFilter
decoder =
    decode LocationFilter
        |> optional "country" (Decode.list Country.decoder) []
        |> optional "searchString" (Decode.map Just Decode.string) Nothing
        |> optional "searchArea" (Decode.map Just <| decodeTuple2 Coordinate.decoder Decode.float) Nothing
        |> optional "ownership" (Decode.list Ownership.decoder) []
        |> optional "occupancy" (Decode.list Occupancy.decoder) []
        |> optional "totalArea" (Range.decoder Decode.float) Range.empty
        |> optional "totalInsuredValue"
            (decodeTuple2 Currency.decoder (Range.decoder Decode.float))
            ( Currency.usd, Range.empty )
        |> optional "siteCondition" (Decode.list QualitativeRating.decoder) []
        |> optional "plantLayout" (Decode.list QualitativeRating.decoder) []
        |> optional "lastSurveyDate" (Decode.map Just decodeDate) Nothing
        |> required "favouritesOnly" Decode.bool
