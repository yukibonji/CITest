module Query.Location exposing (byId, all)

import Http
import Array
import Json.Decode as Decode
import HttpBuilder exposing (withExpect, withQueryParams, RequestBuilder, withBody)
import Query.Helpers exposing (apiUrl)
import Data.AuthToken as AuthToken exposing (AuthToken)
import Data.AppConfig exposing (AppConfig)
import Data.Location as Location exposing (Location, LocationSummary, LocationId, locationIdToString)
import Data.LocationFilter as LocationFilter exposing (LocationFilter)


byId : AppConfig -> Maybe AuthToken -> LocationId -> Http.Request Location
byId appConfig maybeToken locationId =
    let
        expecting =
            Http.expectJson <|
                Location.decoder

        url =
            "/location/"
                ++ locationIdToString locationId
    in
        HttpBuilder.toRequest
            << AuthToken.withAuthorization maybeToken
            << HttpBuilder.withExpect expecting
            << HttpBuilder.get
            << apiUrl appConfig
        <|
            url


all :
    AppConfig
    -> Maybe AuthToken
    -> LocationFilter
    -> Http.Request (List LocationSummary)
all appConfig maybeToken filter =
    let
        expecting =
            Http.expectJson <|
                Decode.map Array.toList <|
                    Decode.array Location.locationSummaryDecoder

        -- queryParams =
        --     maybe
        --         []
        --         (\search ->
        --             [ "search" => (encode 0 <| LocationSearch.encode search) ]
        --         )
        --         maybeSearch
        url =
            "/location"
    in
        HttpBuilder.toRequest
            << AuthToken.withAuthorization maybeToken
            -- << HttpBuilder.withQueryParams queryParams
            <<
                HttpBuilder.withExpect expecting
            << HttpBuilder.get
            << apiUrl appConfig
        <|
            url



-- module Request.Location
--     exposing
--         ( get
--         , list
--         )
--
-- import Http
-- import Array
-- import Json.Decode as Decode
-- import Json.Encode exposing (encode)
-- import HttpBuilder exposing (withExpect, withQueryParams, RequestBuilder, withBody)
-- import Data.AuthToken as AuthToken exposing (AuthToken)
-- import Data.AppConfig exposing (AppConfig)
-- import Data.Location as Location exposing (Location, LocationId, locationIdToString)
-- import Data.LocationSearch as LocationSearch exposing (LocationSearch)
-- import Request.Helpers exposing (apiUrl)
-- import Helpers exposing (maybe, (=>))
--
--
-- get : AppConfig -> Maybe AuthToken -> LocationId -> Http.Request Location
-- get appConfig maybeToken locId =
--     let
--         expecting =
--             Http.expectJson <|
--                 Decode.field "location" Location.decoder
--
--         url =
--             "/location/"
--                 ++ locationIdToString locId
--     in
--         HttpBuilder.toRequest
--             << AuthToken.withAuthorization maybeToken
--             << HttpBuilder.withExpect expecting
--             << HttpBuilder.get
--             << apiUrl appConfig
--         <|
--             url
--
--
-- list : AppConfig -> Maybe AuthToken -> Maybe LocationSearch -> Http.Request (List Location)
-- list appConfig maybeToken maybeSearch =
--     let
--         expecting =
--             Http.expectJson <|
--                 Decode.map Array.toList <|
--                     Decode.array Location.decoder
--
--         -- TODO : implement search
--         queryParams =
--             maybe
--                 []
--                 (\search ->
--                     [ "search" => (encode 0 <| LocationSearch.encode search) ]
--                 )
--                 maybeSearch
--
--         url =
--             "/location"
--     in
--         HttpBuilder.toRequest
--             << AuthToken.withAuthorization maybeToken
--             << HttpBuilder.withQueryParams queryParams
--             << HttpBuilder.withExpect expecting
--             << HttpBuilder.get
--             << apiUrl appConfig
--         <|
--             url
