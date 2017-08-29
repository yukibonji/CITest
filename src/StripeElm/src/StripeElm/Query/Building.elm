module Query.Building exposing (byLocationId)

import Http
import Array
import Json.Decode as Decode
import HttpBuilder exposing (withExpect, withQueryParams, RequestBuilder, withBody)
import Query.Helpers exposing (apiUrl)
import Data.AuthToken as AuthToken exposing (AuthToken)
import Data.Building as Building exposing (Building, BuildingSummary)
import Data.Location as Location exposing (LocationId, locationIdToString)
import Data.AppConfig exposing (AppConfig)


byLocationId :
    AppConfig
    -> Maybe AuthToken
    -> LocationId
    -> Http.Request (List BuildingSummary)
byLocationId appConfig maybeToken locationId =
    let
        expecting =
            Http.expectJson <|
                Decode.map Array.toList <|
                    Decode.array Building.buildingSummaryDecoder

        url =
            "/location/"
                ++ locationIdToString locationId
                ++ "/building"
    in
        HttpBuilder.toRequest
            << AuthToken.withAuthorization maybeToken
            << HttpBuilder.withExpect expecting
            << HttpBuilder.get
            << apiUrl appConfig
        <|
            url
