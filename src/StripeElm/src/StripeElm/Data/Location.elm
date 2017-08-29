module Data.Location
    exposing
        ( Location
        , LocationSummary
        , LocationId(..)
        , ApiCommand(..)
        , locationIdParser
        , locationIdToString
        , encodeLocationId
        , encode
        , encodeApiCommand
        , locationSummaryDecoder
        , decoder
        )

import Date exposing (Date)
import Json.Encode as Encode exposing (Value)
import Json.Encode.Extra as Encode
import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline as Pipeline exposing (decode, required, optional)
import UrlParser
import Helpers exposing ((=>), encodeTuple2, decodeTuple2, decodeDate, encodeDate)
import Data.Address as Address exposing (Address)
import Data.Coordinate as Coordinate exposing (Coordinate)
import Data.Building.Ownership as Ownership exposing (Ownership)
import Data.Building.QualitativeRating as Rating exposing (QualitativeRating)


{-|
    # Location

    Locations are groupings buildings that are the insurable asset of a client.

-}
type alias Location =
    { id : LocationId
    , name : String
    , description : Maybe String
    , address : Address
    , geocode : Maybe Coordinate
    , siteCondition : Maybe QualitativeRating
    , plantLayout : Maybe QualitativeRating
    , ownership : Maybe Ownership
    , lastSurveyDate : Maybe Date
    }


{-|
    # Location Summary

    The result of a location search
-}
type alias LocationSummary =
    { id : LocationId
    , name : String
    , description : String
    , address : Address
    , isFavourite : Bool
    }



-- Api Command -----------------------------------------------------------------


type ApiCommand
    = Create LocationId String Address (Maybe Coordinate)
    | UpdateName LocationId String
    | UpdateDescription LocationId String
    | UpdateAddress LocationId Address (Maybe Coordinate)
    | UpdateSiteCondition LocationId QualitativeRating
    | UpdatePlantLayout LocationId QualitativeRating
    | UpdateOwnership LocationId Ownership
    | UpdateLastSurveyDate LocationId Date



-- Identifiers -----------------------------------------------------------------


type LocationId
    = LocationId String


locationIdParser : UrlParser.Parser (LocationId -> a) a
locationIdParser =
    UrlParser.custom "LOCATIONID" (Ok << LocationId)


locationIdToString : LocationId -> String
locationIdToString (LocationId locId) =
    locId


encodeLocationId : LocationId -> Value
encodeLocationId (LocationId id) =
    Encode.object [ "LocationId" => Encode.string id ]



-- Serialization ---------------------------------------------------------------


encode : Location -> Value
encode location =
    Encode.object
        [ "id" => Encode.object [ "LocationId" => Encode.string (locationIdToString location.id) ]
        , "name" => Encode.string location.name
        , "description" => Encode.maybe Encode.string location.description
        , "address" => Address.encode location.address
        , "geocode" => Encode.maybe Coordinate.encode location.geocode
        , "siteCondition" => Encode.maybe Rating.encode location.siteCondition
        , "plantLayout" => Encode.maybe Rating.encode location.plantLayout
        , "ownership" => Encode.maybe Ownership.encode location.ownership
        , "lastSurveyDate" => Encode.maybe encodeDate location.lastSurveyDate
        ]


decoder : Decoder Location
decoder =
    decode Location
        |> required "id" (decode LocationId |> required "LocationId" Decode.string)
        |> required "name" Decode.string
        |> optional "description" (Decode.maybe Decode.string) Nothing
        |> required "address" Address.decoder
        |> optional "geocode" (Decode.maybe Coordinate.decoder) Nothing
        |> optional "siteCondition" (Decode.maybe Rating.decoder) Nothing
        |> optional "plantLayout" (Decode.maybe Rating.decoder) Nothing
        |> optional "ownership" (Decode.maybe Ownership.decoder) Nothing
        |> optional "lastSurveyDate" (Decode.maybe decodeDate) Nothing


locationSummaryDecoder : Decoder LocationSummary
locationSummaryDecoder =
    decode LocationSummary
        |> required "id" (decode LocationId |> required "LocationId" Decode.string)
        |> required "name" Decode.string
        |> required "description" Decode.string
        |> required "address" Address.decoder
        |> required "isFavourite" Decode.bool


encodeApiCommand : ApiCommand -> Value
encodeApiCommand apiCommand =
    (Encode.object << List.singleton) <|
        case apiCommand of
            Create locationId name address maybeCoordinate ->
                "Create"
                    => Encode.list
                        [ Encode.string (locationIdToString locationId)
                        , Encode.string name
                        , Address.encode address
                        , Encode.maybe Coordinate.encode maybeCoordinate
                        ]

            UpdateName locationId name ->
                "UpdateName"
                    => Encode.list
                        [ Encode.string (locationIdToString locationId)
                        , Encode.string name
                        ]

            UpdateDescription locationId description ->
                "UpdateDescription"
                    => Encode.list
                        [ Encode.string (locationIdToString locationId)
                        , Encode.string description
                        ]

            UpdateAddress locationId address maybeCoordinate ->
                "UpdateAddress"
                    => Encode.list
                        [ Encode.string (locationIdToString locationId)
                        , Address.encode address
                        , Encode.maybe Coordinate.encode maybeCoordinate
                        ]

            UpdateSiteCondition locationId rating ->
                "UpdateSiteCondition"
                    => Encode.list
                        [ Encode.string (locationIdToString locationId)
                        , Rating.encode rating
                        ]

            UpdatePlantLayout locationId rating ->
                "UpdatePlantLayout"
                    => Encode.list
                        [ Encode.string (locationIdToString locationId)
                        , Rating.encode rating
                        ]

            UpdateOwnership locationId ownership ->
                "UpdateOwnership"
                    => Encode.list
                        [ Encode.string (locationIdToString locationId)
                        , Ownership.encode ownership
                        ]

            UpdateLastSurveyDate locationId date ->
                "UpdateLastSurveyDate"
                    => Encode.list
                        [ Encode.string (locationIdToString locationId)
                        , encodeDate date
                        ]
