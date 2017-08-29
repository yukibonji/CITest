module Data.Warehouse exposing (Warehouse, ApiCommand(..), encodeApiCommand, decoder)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline as Pipeline exposing (decode, required, optional)
import Data.Building as Building exposing (Building, BuildingId)
import Data.Location as Location exposing (LocationId)
import Data.Response as Response exposing (Response)
import Helpers exposing ((=>))


type alias Warehouse =
    Building UserInput


type alias UserInput =
    { footprint : Response Float
    , yearBuilt : Response Int
    }



-- Api Commands ----------------------------------------------------------------


type ApiCommand
    = Create BuildingId LocationId String
    | Delete BuildingId
    | UpdateName BuildingId String
    | AnswerFootprint BuildingId (Response Float)
    | AnswerYearBuilt BuildingId (Response Int)


encodeApiCommand : ApiCommand -> Value
encodeApiCommand apiCommand =
    (Encode.object << List.singleton) <|
        case apiCommand of
            Create buildingId locationId name ->
                "Create"
                    => Encode.list
                        [ Building.encodeBuildingId buildingId
                        , Location.encodeLocationId locationId
                        , Encode.string name
                        ]

            Delete buildingId ->
                "Delete"
                    => Building.encodeBuildingId buildingId

            UpdateName buildingId name ->
                "UpdateName"
                    => Encode.list
                        [ Building.encodeBuildingId buildingId
                        , Encode.string name
                        ]

            AnswerFootprint buildingId response ->
                "AnswerFootprint"
                    => Encode.list
                        [ Building.encodeBuildingId buildingId
                        , Response.encode Encode.float response
                        ]

            AnswerYearBuilt buildingId response ->
                "AnswerYearBuilt"
                    => Encode.list
                        [ Building.encodeBuildingId buildingId
                        , Response.encode Encode.int response
                        ]



-- Serialization ---------------------------------------------------------------


decoder : Decoder Warehouse
decoder =
    Building.decoder userInputDecoder


userInputDecoder : Decoder UserInput
userInputDecoder =
    decode UserInput
        |> required "footprint" (Response.decoder Decode.float)
        |> required "yearBuilt" (Response.decoder Decode.int)
