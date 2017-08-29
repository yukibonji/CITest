module Query.Warehouse exposing (byId)

import Http
import Array
import Json.Decode as Decode
import HttpBuilder exposing (withExpect, withQueryParams, RequestBuilder, withBody)
import Query.Helpers exposing (apiUrl)
import Data.AppConfig exposing (AppConfig)
import Data.AuthToken as AuthToken exposing (AuthToken)
import Data.Building as Building exposing (BuildingId, buildingIdToString)
import Data.Warehouse as Warehouse exposing (Warehouse)


byId :
    AppConfig
    -> Maybe AuthToken
    -> BuildingId
    -> Http.Request Warehouse
byId appConfig maybeToken buildingId =
    let
        expecting =
            Http.expectJson Warehouse.decoder

        url =
            "/warehouse/"
                ++ buildingIdToString buildingId
    in
        HttpBuilder.toRequest
            << AuthToken.withAuthorization maybeToken
            << HttpBuilder.withExpect expecting
            << HttpBuilder.get
            << apiUrl appConfig
        <|
            url
