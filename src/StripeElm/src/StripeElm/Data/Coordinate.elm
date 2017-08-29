module Data.Coordinate exposing (Coordinate, encode, decoder)

import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode exposing (Decoder)
import Json.Decode.Pipeline as Pipeline exposing (decode, required, optional)
import Helpers exposing ((=>))


-- Types -----------------------------------------------------------------------


type alias Coordinate =
    { latitude : Float
    , longitude : Float
    }



-- type Latitude
--     = North Angle
--     | South Angle
--
--
-- type Longitude
--     = East Angle
--     | West Angle
--
--
-- type alias Angle =
--     { degrees : Float
--     , minutes : Float
--     , seconds : Float
--     }
--
--
--
-- -- Helpers ---------------------------------------------------------------------
--
--
-- latitudeToFloat : Latitude -> Float
-- latitudeToFloat latitude =
--     case latitude of
--         North angle ->
--             angleToFloat angle
--
--         South angle ->
--             -1 * angleToFloat angle
--
--
-- latitudeFromFloat : Float -> Maybe Latitude
-- latitudeFromFloat latitude =
--     let
--         latabs =
--             latitude * 1000000 |> floor |> abs
--     in
--         if latabs > 90000000 then
--             Nothing
--         else
--             let
--                 x =
--                     (toFloat latabs) / 1000000
--
--                 degs =
--                     toFloat <| floor x
--
--                 y =
--                     (x - degs) * 60
--
--                 mins =
--                     toFloat <| floor y
--
--                 secs =
--                     60 * y - 60 * mins
--
--                 angle =
--                     Angle degs mins secs
--             in
--                 if latitude >= 0 then
--                     Just <| North angle
--                 else
--                     Just <| South angle
--
--
-- longitudeToFloat : Longitude -> Float
-- longitudeToFloat longitude =
--     case longitude of
--         East angle ->
--             angleToFloat angle
--
--         West angle ->
--             -1 * angleToFloat angle
--
--
-- longitudeFromFloat : Float -> Maybe Longitude
-- longitudeFromFloat latitude =
--     let
--         latabs =
--             latitude * 1000000 |> floor |> abs
--     in
--         if latabs > 180000000 then
--             Nothing
--         else
--             let
--                 x =
--                     (toFloat latabs) / 1000000
--
--                 degs =
--                     toFloat <| floor x
--
--                 y =
--                     (x - degs) * 60
--
--                 mins =
--                     toFloat <| floor y
--
--                 secs =
--                     60 * y - 60 * mins
--
--                 angle =
--                     Angle degs mins secs
--             in
--                 if latitude >= 0 then
--                     Just <| East angle
--                 else
--                     Just <| West angle
--
--
-- angleToFloat :
--     Angle
--     -> Float
-- angleToFloat { degrees, minutes, seconds } =
--     degrees
--         + (minutes / 60)
--         + (seconds / 360)
-- Serialization ---------------------------------------------------------------


encode : Coordinate -> Value
encode geocode =
    Encode.object
        [ "latitude" => Encode.float geocode.latitude
        , "longitude" => Encode.float geocode.longitude
        ]


decoder : Decoder Coordinate
decoder =
    decode Coordinate
        |> required "latitude" Decode.float
        |> required "longitude" Decode.float



--
-- encodeLatitude : Latitude -> Value
-- encodeLatitude latitude =
--     Encode.object <|
--         case latitude of
--             North angle ->
--                 [ "North" => encodeAngle angle ]
--
--             South angle ->
--                 [ "South" => encodeAngle angle ]
--
--
-- decodeLatitude : Decoder Latitude
-- decodeLatitude =
--     Decode.oneOf
--         [ Decode.map North <| Decode.field "North" decodeAngle
--         , Decode.map South <| Decode.field "South" decodeAngle
--         ]
--
--
-- encodeLongitude : Longitude -> Value
-- encodeLongitude longitude =
--     Encode.object <|
--         case longitude of
--             East angle ->
--                 [ "East" => encodeAngle angle ]
--
--             West angle ->
--                 [ "West" => encodeAngle angle ]
--
--
-- decodeLongitude : Decoder Longitude
-- decodeLongitude =
--     Decode.oneOf
--         [ Decode.map East <| Decode.field "East" decodeAngle
--         , Decode.map West <| Decode.field "West" decodeAngle
--         ]
--
--
-- encodeAngle : Angle -> Value
-- encodeAngle { degrees, minutes, seconds } =
--     Encode.object
--         [ "degrees" => Encode.float degrees
--         , "minutes" => Encode.float minutes
--         , "seconds" => Encode.float seconds
--         ]
--
--
-- decodeAngle : Decoder Angle
-- decodeAngle =
--     decode Angle
--         |> required "degrees" Decode.float
--         |> required "minutes" Decode.float
--         |> required "seconds" Decode.float
