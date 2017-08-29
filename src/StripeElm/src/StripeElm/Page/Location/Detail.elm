module Page.Location.Detail exposing (view, Model, init, update, Msg, subscriptions)

import Basics exposing ((%))
import Color exposing (Color)
import Date exposing (Date)
import Task exposing (Task)
import Random
import Http
import Html as H exposing (Html, text)
import Html.Attributes as A exposing (class, classList, alt)
import Html.Events as E
import Helpers exposing ((=>), pluralize, maybe)
import Route
import Query.Building
import Query.Location
import View.Frame as Frame
import View.Icon as Icon exposing (Direction(Right))
import View.Button as Button
import View.Input as Input
import View.Select as Select
import Page.Errored as Errored exposing (PageLoadError, pageLoadError)
import Data.AppConfig exposing (AppConfig)
import Data.Address as Address exposing (Address)
import Data.Building exposing (BuildingId(..), BuildingSummary)
import Data.Location exposing (LocationId, Location, locationIdToString)
import Data.Session exposing (Session)
import Data.UUID exposing (UUID, uuid)
import Data.Building.Occupancy as Occupancy exposing (Occupancy(..))
import Command.Warehouse


-- Model -----------------------------------------------------------------------


type alias Model =
    { location : Location
    , buildings : List BuildingSummary
    , tempBuilding : Maybe TemporaryBuilding
    }


type alias TemporaryBuilding =
    { name : String
    , occupancy : Maybe Occupancy
    , status : TemporaryBuildingStatus
    }


type TemporaryBuildingField
    = FieldName
    | FieldOccupancy


type TemporaryBuildingStatus
    = Editing
    | ValidationErrors (List ( TemporaryBuildingField, String ))
    | Saving
    | Failed String
    | Succeded


emptyTempBuilding : { name : String, occupancy : Maybe a, status : TemporaryBuildingStatus }
emptyTempBuilding =
    { name = "", occupancy = Nothing, status = ValidationErrors [] }


init : AppConfig -> Session -> LocationId -> Task PageLoadError Model
init appConfig session locationId =
    let
        maybeAuthToken =
            Maybe.map .token session.user

        loadLocation =
            Http.toTask <|
                Query.Location.byId appConfig maybeAuthToken locationId

        loadBuildings =
            Http.toTask <|
                Query.Building.byLocationId appConfig maybeAuthToken locationId

        handleLoadError err =
            pageLoadError Frame.Other <|
                "This location is currently unavailable "
                    ++ toString err
    in
        Task.mapError handleLoadError <|
            Task.map (\( loc, bldgs ) -> Model loc bldgs Nothing) <|
                Task.andThen
                    (\loc -> Task.map (\bldgs -> ( loc, bldgs )) loadBuildings)
                    loadLocation



-- Update ----------------------------------------------------------------------


type Msg
    = NoOp
    | BeginCreateBuilding
    | DiscardCreateBuilding
    | SaveCreateBuilding
    | CreateBuildingId Occupancy LocationId String
    | CreateBuilding Occupancy LocationId String ( UUID, UUID )
    | CreateBuildingResult (Result Http.Error BuildingSummary)
    | UpdateTempBuildingName String
    | UpdateTempBuildingOccupancy (Maybe Occupancy)
    | DeleteBuildingId BuildingId
    | DeleteBuilding BuildingId UUID
    | DeleteBuildingResult BuildingId (Result Http.Error String)


update : AppConfig -> Session -> Msg -> Model -> ( Model, Cmd Msg )
update appConfig session msg model =
    case msg of
        NoOp ->
            model => Cmd.none

        -- Create building state machine --
        BeginCreateBuilding ->
            case model.tempBuilding of
                Nothing ->
                    { model | tempBuilding = Just emptyTempBuilding } => Cmd.none

                _ ->
                    model => Cmd.none

        UpdateTempBuildingName name ->
            let
                newTempBuilding =
                    Maybe.map (\t -> updateStatus { t | name = name }) model.tempBuilding
            in
                { model | tempBuilding = newTempBuilding } => Cmd.none

        UpdateTempBuildingOccupancy occupancy ->
            let
                newTempBuilding =
                    Maybe.map (\t -> updateStatus { t | occupancy = occupancy }) model.tempBuilding
            in
                { model | tempBuilding = newTempBuilding } => Cmd.none

        DiscardCreateBuilding ->
            { model | tempBuilding = Nothing } => Cmd.none

        SaveCreateBuilding ->
            case model.tempBuilding of
                Just b ->
                    case validateTemporaryBuilding b of
                        [] ->
                            case b.occupancy of
                                Just occupancy ->
                                    let
                                        newTempBuilding =
                                            { b | status = Saving }

                                        newMsg =
                                            CreateBuildingId occupancy model.location.id b.name

                                        newModel =
                                            { model | tempBuilding = Just newTempBuilding }
                                    in
                                        update appConfig session newMsg newModel

                                _ ->
                                    model => Cmd.none

                        errors ->
                            let
                                newTempBuilding =
                                    { b | status = ValidationErrors errors }
                            in
                                { model | tempBuilding = Just newTempBuilding } => Cmd.none

                _ ->
                    model => Cmd.none

        CreateBuildingId occupancy locationId name ->
            let
                cmd =
                    Random.generate
                        (CreateBuilding occupancy locationId name)
                        (Random.pair uuid uuid)
            in
                model => cmd

        CreateBuilding Warehouse locationId name ( buildingId, commandId ) ->
            let
                buildingSummary =
                    { id = BuildingId buildingId
                    , locationId = locationId
                    , name = name
                    , occupancy = Warehouse
                    , userInput = ()
                    }

                cmd =
                    Task.attempt CreateBuildingResult <|
                        Task.map (always buildingSummary) <|
                            Http.toTask <|
                                Command.Warehouse.create
                                    appConfig
                                    Nothing
                                    (BuildingId buildingId)
                                    locationId
                                    name
                                    commandId
            in
                model => cmd

        CreateBuildingResult (Ok buildingSummary) ->
            let
                buildings =
                    model.buildings
            in
                { model
                    | tempBuilding = Nothing
                    , buildings = buildingSummary :: buildings
                }
                    => Cmd.none

        CreateBuildingResult (Err error) ->
            let
                newTempBuilding =
                    Maybe.map
                        (\t -> { t | status = Failed (toString error) })
                        model.tempBuilding
            in
                { model | tempBuilding = newTempBuilding } => Cmd.none

        DeleteBuildingId buildingId ->
            let
                cmd =
                    Random.generate
                        (DeleteBuilding buildingId)
                        uuid
            in
                model => cmd

        DeleteBuilding buildingId commandId ->
            let
                cmd =
                    Task.attempt
                        (DeleteBuildingResult buildingId)
                    <|
                        Http.toTask <|
                            Command.Warehouse.delete
                                appConfig
                                Nothing
                                buildingId
                                commandId
            in
                model => cmd

        DeleteBuildingResult buildingId (Ok _) ->
            let
                buildings =
                    List.filter (\blgs -> blgs.id /= buildingId) model.buildings
            in
                { model | buildings = buildings } => Cmd.none

        DeleteBuildingResult buildingId _ ->
            model => Cmd.none


updateStatus :
    TemporaryBuilding
    -> TemporaryBuilding
updateStatus tempBuilding =
    let
        status =
            case validateTemporaryBuilding tempBuilding of
                [] ->
                    Editing

                errors ->
                    ValidationErrors errors
    in
        { tempBuilding | status = status }


validateTemporaryBuilding :
    TemporaryBuilding
    -> List ( TemporaryBuildingField, String )
validateTemporaryBuilding tempBuilding =
    case ( tempBuilding.name, tempBuilding.occupancy ) of
        ( "", Nothing ) ->
            [ ( FieldName, "The building name cannot be blank" )
            , ( FieldOccupancy, "You must specify the buildings occupancy" )
            ]

        ( "", _ ) ->
            [ ( FieldName, "The building name cannot be blank" )
            ]

        ( _, Nothing ) ->
            [ ( FieldOccupancy, "You must specify the buildings occupancy" )
            ]

        ( name, Just occupancy ) ->
            []



-- Subscriptions ---------------------------------------------------------------


subscriptions : Model -> Sub Msg
subscriptions model =
    Sub.none



-- View ------------------------------------------------------------------------


view : AppConfig -> Session -> Model -> Html Msg
view appConfig session model =
    H.div
        []
        --class "l-container l-container--vertical" ]
        [ H.div
            [ class "o-grid o-grid--small-full" ]
            [ H.div [ class "o-grid__cell o-grid__cell--width-60@medium o-grid__cell--width-65@large" ]
                [ viewLocationDetail model
                , viewBuildings model.location.id model.tempBuilding model.buildings
                ]
            , H.div
                [ class "o-grid__cell" ]
                [ viewLocationMap model.location.geocode ]
            ]
        ]


viewLocationDetail : Model -> Html msg
viewLocationDetail { location } =
    H.div
        [ class "c-simple-panel" ]
        [ H.div
            [ class "c-card__body" ]
            [ H.h2 [ class "c-card__title" ] [ H.text location.name ]
            , H.p [ class "c-card__subtitle" ]
                [ H.text <| String.join " " location.address.addressLines
                , H.br [] []
                , H.text <| Address.summary location.address
                ]
            , H.p [ class "c-card__intro" ]
                [ H.text <|
                    Maybe.withDefault
                        "no description available"
                        location.description
                ]
            , H.hr [] []
            , H.div
                [ class "o-grid" ]
                [ H.div
                    [ class "o-grid__cell" ]
                    [ H.h5
                        [ class "c-card__section-title" ]
                        [ H.text "Last Survey Date" ]
                    , H.p
                        [ class "c-card__subtitle" ]
                        [ H.text <|
                            Maybe.withDefault "no survey" <|
                                Maybe.map
                                    formatSurveyDate
                                    location.lastSurveyDate
                        ]
                    ]
                , H.div
                    [ class "o-grid__cell" ]
                    [ H.h5
                        [ class "c-card__section-title" ]
                        [ H.text "Ownership" ]
                    , H.p
                        [ class "c-card__subtitle" ]
                        [ H.text <|
                            Maybe.withDefault "no ownership information" <|
                                Maybe.map toString location.ownership
                        ]
                    ]
                ]
            , H.hr [] []
            , H.div
                [ class "o-grid" ]
                [ H.div
                    [ class "o-grid__cell" ]
                    [ H.h5
                        [ class "c-card__section-title" ]
                        [ H.text "Site Condition" ]
                    , H.p
                        [ class "c-card__subtitle" ]
                        [ H.text <|
                            Maybe.withDefault "no site condition information" <|
                                Maybe.map toString location.siteCondition
                        ]
                    ]
                , H.div
                    [ class "o-grid__cell" ]
                    [ H.h5
                        [ class "c-card__section-title" ]
                        [ H.text "Plant Layout" ]
                    , H.p
                        [ class "c-card__subtitle" ]
                        [ H.text <|
                            Maybe.withDefault "no plant layout information" <|
                                Maybe.map toString location.plantLayout
                        ]
                    ]
                ]
            ]
        ]


formatSurveyDate : Date -> String
formatSurveyDate date =
    toString (Date.dayOfWeek date)
        ++ ", "
        ++ toString (Date.day date)
        ++ " "
        ++ toString (Date.month date)
        ++ ", "
        ++ toString (Date.year date)


viewLocationMap : Maybe { c | latitude : a, longitude : b } -> Html msg
viewLocationMap geocode =
    H.div
        [ class "c-card c-simple-panel"
        ]
        [ H.div
            [ A.style
                [ ( "background-color", "#e7e7e7" )
                , ( "height", "100%" )
                , ( "width", "100%" )
                , ( "background-size", "contain" )
                , ( "background-repeat", "no-repeat" )
                , case geocode of
                    Just coord ->
                        ( "background-image", "url('" ++ mapboxUrl coord ++ "')" )

                    _ ->
                        ( "background-image", "url('./static/img/450x450.png')" )
                ]
            ]
            [ H.img
                [ A.src "./static/img/450x450.png"
                , A.style
                    [ ( "background-color", "#e7e7e7" )
                    , ( "height", "100%" )
                    , ( "width", "100%" )
                    , ( "visibility", "hidden" )
                    ]
                ]
                []
            ]
        ]


viewBuildings :
    LocationId
    -> Maybe TemporaryBuilding
    -> List BuildingSummary
    -> Html Msg
viewBuildings locationId maybeTempBuilding buildings =
    H.div
        [ class "c-simple-panel"
        ]
        [ H.nav
            [ class "c-navbar" ]
            [ H.div
                [ class "c-navbar__navgroup " ]
                [ H.text <| pluralize "building" "buildings" (List.length buildings)
                ]
            , H.div
                [ class "c-navbar__navgroup " ]
                [ H.button
                    [ class "c-navbar__navicon c-tooltip c-tooltip--top"
                    , A.attribute "aria-label" "New Building"
                    , E.onClick BeginCreateBuilding
                    ]
                    [ Icon.view <|
                        Icon.AddLocation
                            { size = 32, color = Color.rgb 29 188 156 }
                    ]
                ]
            ]
        , H.div [ class ".card-container" ]
            (case maybeTempBuilding of
                Just tempBuilding ->
                    viewTempBuilding tempBuilding
                        :: List.indexedMap (viewBuilding True) buildings

                _ ->
                    List.indexedMap (viewBuilding False) buildings
            )
        ]


isJust : Maybe a -> Bool
isJust x =
    case x of
        Just _ ->
            True

        _ ->
            False


isDisabledStatus : TemporaryBuildingStatus -> Bool
isDisabledStatus status =
    case status of
        Saving ->
            True

        ValidationErrors _ ->
            True

        _ ->
            False


viewTempBuilding : TemporaryBuilding -> Html Msg
viewTempBuilding tempBuilding =
    let
        errors =
            case tempBuilding.status of
                ValidationErrors errors ->
                    errors

                _ ->
                    []

        nameError =
            List.filter (\( fld, _ ) -> fld == FieldName) errors
                |> List.head

        occupancyError =
            List.filter (\( fld, _ ) -> fld == FieldOccupancy) errors
                |> List.head
    in
        H.div
            [ class "o-grid"
            , A.style
                [ ( "border-bottom", "1px solid #e7e7e7" )
                , ( "border-radius", "0" )
                , ( "background-color", "#f7f7f7" )
                ]
            ]
            [ H.div
                [ class "o-grid__cell o-grid__cell--width-35"
                ]
                [ Input.view
                    { format = identity
                    , placeholder = "building name"
                    , attributes =
                        [ classList [ ( "c-field--error", isJust nameError ) ] ]
                    , onInput = UpdateTempBuildingName
                    }
                    tempBuilding.name
                ]
            , H.div
                [ class "o-grid__cell o-grid__cell--width-30" ]
                [ Select.view
                    { toString = Occupancy.toString
                    , toValue = Occupancy.toString
                    , fromValue = Occupancy.fromString
                    , emptyElement = Just "select occupancy"
                    , onChange = UpdateTempBuildingOccupancy
                    , attributes =
                        [ classList [ ( "c-field--error", isJust occupancyError ) ] ]
                    }
                    [ Occupancy.Warehouse ]
                    tempBuilding.occupancy
                ]
            , H.div
                [ class "o-grid__cell o-grid__cell--width-30" ]
                [ H.span
                    [ A.class "c-input-group" ]
                    [ Button.button
                        False
                        (isDisabledStatus tempBuilding.status)
                        [ A.style
                            [ ( "height", "50%" )
                            , ( "width", "100%" )
                            ]
                        , E.onClick SaveCreateBuilding
                        ]
                        [ H.text "save" ]
                    , Button.button
                        False
                        (isDisabledStatus tempBuilding.status)
                        [ A.style
                            [ ( "height", "50%" ), ( "width", "100%" ) ]
                        , E.onClick DiscardCreateBuilding
                        ]
                        [ H.text "discard" ]
                    ]
                ]
            ]


viewBuilding :
    Bool
    -> Int
    -> BuildingSummary
    -> Html Msg
viewBuilding isEditing i buildingSummary =
    let
        bg =
            if
                (i
                    % 2
                    == (if isEditing then
                            1
                        else
                            0
                       )
                )
            then
                "#f7f7f7"
            else
                "#fff"
    in
        H.div
            [ class "o-grid"
            , A.style
                [ ( "border-bottom", "1px solid #e7e7e7" )
                , ( "border-radius", "0" )
                , ( "background-color", bg )
                ]
            ]
            [ H.div
                [ class "o-grid__cell o-grid__cell--width-35"
                ]
                [ H.div
                    [ A.style
                        [ ( "user-select", "none" )
                        , ( "cursor", "default" )
                        ]
                    ]
                    [ H.div
                        [ A.style [ ( "transform", "translateY(50%)" ) ] ]
                        [ H.text buildingSummary.name ]
                    ]
                ]
            , H.div
                [ class "o-grid__cell o-grid__cell--width-30" ]
                [ H.span
                    [ A.style
                        [ ( "user-select", "none" )
                        , ( "cursor", "default" )
                        ]
                    ]
                    [ H.div
                        [ A.style
                            [ ( "transform", "translateY(50%)" )
                            ]
                        ]
                        [ H.text <| toString buildingSummary.occupancy ]
                    ]
                ]
            , H.div
                [ class "o-grid__cell o-grid__cell--width-10" ]
                [ H.button
                    [ class "c-navbar__navicon c-tooltip c-tooltip--top"
                    , A.attribute "aria-label" "Delete Building"
                    , E.onClick <| DeleteBuildingId buildingSummary.id
                    ]
                    [ Icon.view <|
                        Icon.Delete
                            { size = 20
                            , color = Color.rgb 255 120 100
                            }
                    ]
                ]
            , H.div
                [ class "o-grid__cell o-grid__cell--width-10" ]
                [ H.a
                    [ class "c-navbar__navicon c-tooltip c-tooltip--top"
                    , A.attribute "aria-label" "Review Building"
                    , Route.href <|
                        Route.BuildingSummary buildingSummary.occupancy buildingSummary.id
                    ]
                    [ Icon.view <|
                        Icon.PageView
                            { size = 20
                            , color = Color.rgb 182 202 43
                            }
                    ]
                ]
            , H.div
                [ class "o-grid__cell o-grid__cell--width-10" ]
                [ H.a
                    [ class "c-navbar__navicon c-tooltip c-tooltip--top"
                    , A.attribute "aria-label" "Edit Building"
                      -- , A.style [ ( "transform", "translateY(28%)" ) ]
                    , Route.href <|
                        Route.EvidenceBuilder buildingSummary.occupancy buildingSummary.id
                    ]
                    [ Icon.view <|
                        Icon.Edit
                            { size = 20
                            , color = Color.rgb 0 99 126
                            }
                    ]
                ]
            ]



-- H.div
--     [ class "c-card" ]
--     [ H.div
--         [ class "c-card__body" ]
--         [ H.h2 [ class "c-card__title" ] [ H.text buildingSummary.name ]
--         , H.p [ class "c-card__subtitle" ] [ H.text <| toString buildingSummary.occupancy ]
--         ]
--     , H.footer
--         [ class "c-card__footer c-navbar" ]
--         [ H.div
--             [ class "c-navbar__navgroup" ]
--             [ H.div
--                 [ class "c-navbar__navicon"
--                 ]
--                 [ Icon.view <|
--                     Icon.Delete
--                         { size = 28
--                         , color = Color.rgb 190 59 49
--                         }
--                 ]
--             ]
--         , H.div
--             [ class "c-navbar__navgroup" ]
--             [ H.div
--                 [ class "c-navbar__navicon"
--                 ]
--                 [ Icon.view <|
--                     Icon.PageView
--                         { size = 28
--                         , color = Color.rgb 57 75 89
--                         }
--                 ]
--             ]
--         , H.div
--             [ class "c-navbar__navgroup " ]
--             [ H.a
--                 [ class "c-navbar__navitem"
--                 , Route.href <|
--                     Route.EvidenceBuilder buildingSummary.occupancy buildingSummary.id
--                 ]
--                 [ Icon.view <|
--                     Icon.Edit
--                         { size = 28
--                         , color = Color.rgb 8 65 80
--                         }
--                 ]
--             ]
--         ]
--     ]
-- Helpers ---------------------------------------------------------------------


mapboxUrl : { c | latitude : a, longitude : b } -> String
mapboxUrl coordinate =
    let
        lat =
            coordinate.latitude

        long =
            coordinate.longitude

        baseUrl =
            "https://api.mapbox.com/styles/v1/mapbox/satellite-streets-v9/static"

        --streets-v10
        pin =
            "/pin-s+1DBC9C(" ++ toString long ++ "," ++ toString lat ++ ")"

        position =
            "/" ++ toString long ++ "," ++ toString lat ++ ",15"

        format =
            "/400x400@2x"

        token =
            "?access_token=pk.eyJ1IjoiZW5ldHNlZSIsImEiOiJjajRxcnFoaGMyZ3FoMzNvN3RvNjhpdGxrIn0.QXBVnSjvPHxZTlpg0p8Nvw"
    in
        baseUrl ++ pin ++ position ++ format ++ token
