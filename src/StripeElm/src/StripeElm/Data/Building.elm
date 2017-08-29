module Data.Building exposing (..)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline as Pipeline exposing (decode, required, optional)
import UrlParser exposing (Parser)
import Data.Location exposing (LocationId(..))
import Data.Building.Occupancy as Occupancy exposing (Occupancy)
import Helpers exposing ((=>))


type BuildingId
    = BuildingId String


buildingIdToString : BuildingId -> String
buildingIdToString (BuildingId id) =
    id


buildingIdParser : Parser (BuildingId -> a) a
buildingIdParser =
    UrlParser.custom "LOCATIONID" (Ok << BuildingId)


encodeBuildingId : BuildingId -> Value
encodeBuildingId (BuildingId id) =
    Encode.object [ "BuildingId" => Encode.string id ]


type alias Building userInput =
    { id : BuildingId
    , locationId : LocationId
    , name : String
    , occupancy : Occupancy
    , userInput : userInput
    }


type alias BuildingSummary =
    Building ()


decoder :
    Decoder userInput
    -> Decoder (Building userInput)
decoder userInputDecoder =
    decode Building
        |> required "id" (decode BuildingId |> required "BuildingId" Decode.string)
        |> required "locationId" (decode LocationId |> required "LocationId" Decode.string)
        |> required "name" Decode.string
        |> required "occupancy" Occupancy.decoder
        |> required "userInput" userInputDecoder


buildingSummaryDecoder : Decoder (Building ())
buildingSummaryDecoder =
    decode Building
        |> required "id" (decode BuildingId |> required "BuildingId" Decode.string)
        |> required "locationId" (decode LocationId |> required "LocationId" Decode.string)
        |> required "name" Decode.string
        |> required "occupancy" Occupancy.decoder
        |> optional "userInput" (Decode.succeed ()) ()
