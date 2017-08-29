module Data.SpatialQuery exposing (SpatialQuery, encode, decoder)

import Helpers exposing (encodeDU, decodeDU)
import Json.Encode exposing (Value, null)
import Json.Decode
    exposing
        ( Decoder
        , succeed
        )


{-|
    # SpatialQuery

    TODO: implement Dimensionally extended nine-intersection model for use
    with PostGIS
-}
type SpatialQuery
    = SpatialQuery


encode : SpatialQuery -> Value
encode =
    encodeDU
        (\_ -> ( "spatialQuery", null ))
        "spatialQuery"


decoder : Decoder SpatialQuery
decoder =
    decodeDU
        "spatialQuery"
        (\tag ->
            if tag == "spatialQuery" then
                Just ( succeed (), \_ -> SpatialQuery )
            else
                Nothing
        )
