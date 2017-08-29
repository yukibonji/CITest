module Page.Warehouse.Wizard
    exposing
        ( view
        , Model
        , init
        , update
        , Msg
        , subscriptions
        )

import Task exposing (Task)
import Html exposing (Html, text)
import Helpers exposing ((=>))
import Data.AppConfig exposing (AppConfig)
import Data.Building exposing (BuildingId)
import Data.Session exposing (Session)
import Page.Errored as Errored exposing (PageLoadError, pageLoadError)


type alias Model =
    {}


init : AppConfig -> Session -> BuildingId -> Task PageLoadError Model
init appConfig session buildingId =
    Task.succeed {}



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
view appConfig session model =
    text "Warehouse Evidence"
