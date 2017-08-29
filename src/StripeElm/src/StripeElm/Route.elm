module Route exposing (Route(..), href, modifyUrl, fromLocation)

import UrlParser as Url exposing (parseHash, s, (</>), (<?>), stringParam, intParam, string, oneOf, Parser)
import Navigation exposing (Location)
import Html exposing (Attribute)
import Html.Attributes as Attr
import Helpers exposing (jsonParam)
import Json.Encode exposing (encode)
import Data.Building exposing (BuildingId, buildingIdParser, buildingIdToString)
import Data.Location exposing (LocationId, locationIdParser, locationIdToString)
import Data.LocationFilter as LocationFilter exposing (LocationFilter)
import Data.Building.Occupancy as Occupancy exposing (Occupancy, occupancyParser)


-- Routing ---------------------------------------------------------------------


type Route
    = Login
    | Logout
    | Home
    | LocationSearch (Maybe LocationFilter)
    | LocationDetail LocationId
    | BuildingSummary Occupancy BuildingId
    | EvidenceBuilder Occupancy BuildingId


route : Parser (Route -> a) a
route =
    oneOf
        [ Url.map Home (s "")
        , Url.map Login (s "login")
        , Url.map Login (s "logout")
        , Url.map LocationDetail
            (s "location" </> locationIdParser)
        , Url.map LocationSearch
            (s "location" <?> jsonParam "filter" LocationFilter.decoder)
        , Url.map EvidenceBuilder
            (occupancyParser </> buildingIdParser </> s "evidence")
        , Url.map BuildingSummary
            (occupancyParser </> buildingIdParser)
        ]



-- Helpers ---------------------------------------------------------------------


href : Route -> Attribute msg
href route =
    Attr.href (routeToString route)


modifyUrl : Route -> Cmd msg
modifyUrl =
    routeToString >> Navigation.modifyUrl


fromLocation : Location -> Maybe Route
fromLocation location =
    if String.isEmpty location.hash then
        Just Home
    else
        parseHash route <|
            fixLocationQuery location



-- Internal --------------------------------------------------------------------


routeToString : Route -> String
routeToString page =
    let
        pieces =
            case page of
                Home ->
                    []

                Login ->
                    List.singleton "login"

                Logout ->
                    List.singleton "logout"

                LocationSearch Nothing ->
                    [ "location" ]

                LocationSearch (Just filter) ->
                    [ "location?filter=" ++ (encode 0 <| LocationFilter.encodeCompact filter) ]

                LocationDetail locationId ->
                    [ "location", locationIdToString locationId ]

                BuildingSummary occupancy buildingId ->
                    [ (String.toLower <| Occupancy.toString occupancy)
                    , buildingIdToString buildingId
                    ]

                EvidenceBuilder occupancy buildingId ->
                    [ (String.toLower <| Occupancy.toString occupancy)
                    , buildingIdToString buildingId
                    , "evidence"
                    ]
    in
        "#/" ++ (String.join "/" pieces)


fixLocationQuery : Location -> Location
fixLocationQuery location =
    let
        hash =
            String.split "?" location.hash
                |> List.head
                |> Maybe.withDefault ""

        search =
            String.split "?" location.hash
                |> List.drop 1
                |> String.join "?"
                |> String.append "?"
    in
        { location
            | hash = hash
            , search = search
        }
