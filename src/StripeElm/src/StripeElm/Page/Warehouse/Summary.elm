module Page.Warehouse.Summary
    exposing
        ( view
        , Model
        , init
        , update
        , Msg
        , subscriptions
        )

import Http
import Task exposing (Task)
import Html exposing (Html, text)
import Helpers exposing ((=>))
import Data.AppConfig exposing (AppConfig)
import Data.Session exposing (Session)
import Data.Building exposing (BuildingId)
import Data.Warehouse exposing (Warehouse)
import Page.Errored as Errored exposing (PageLoadError, pageLoadError)
import Query.Warehouse
import View.Frame as Frame


type alias Model =
    { warehouse : Warehouse }


init : AppConfig -> Session -> BuildingId -> Task PageLoadError Model
init appConfig session buildingId =
    let
        maybeAuthToken =
            Maybe.map .token session.user

        load =
            Http.toTask <|
                Query.Warehouse.byId appConfig maybeAuthToken buildingId

        handleLoadError err =
            pageLoadError Frame.Other <|
                "warehouse details are currently unavailable "
                    ++ toString err
    in
        Task.mapError handleLoadError <|
            Task.map Model load



-- Update ----------------------------------------------------------------------


type Msg
    = NoOp


update : AppConfig -> Session -> Msg -> Model -> ( Model, Cmd Msg )
update appConfig session msg model =
    case msg of
        NoOp ->
            model => Cmd.none



-- Subscriptions ---------------------------------------------------------------


subscriptions : Model -> Sub Msg
subscriptions model =
    Sub.none



-- View ------------------------------------------------------------------------


view : AppConfig -> Session -> Model -> Html Msg
view appConfig session { warehouse } =
    text <| warehouse.name
