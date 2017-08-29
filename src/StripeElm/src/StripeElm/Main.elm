module Main exposing (main)

import Task
import Json.Encode as Encode exposing (Value)
import Json.Decode as Decode
import Html as H exposing (Html)
import Navigation
import Helpers exposing (maybe, (=>))
import Ports
import Route exposing (Route)
import Data.AppConfig as AppConfig exposing (AppConfig)
import Data.Session exposing (Session)
import Data.User as User exposing (User)
import Data.Building.Occupancy as Occupancy
import Page.Home as Home
import Page.Errored as Errored exposing (PageLoadError, pageLoadError)
import Page.Location.Search as LocationSearch
import Page.Location.Detail as LocationDetail
import Page.Login as Login
import Page.NotFound as NotFound
import Page.Warehouse.Summary as WarehouseSummary
import Page.Warehouse.Wizard as WarehouseWizard
import View.Frame as Frame exposing (ActivePage, frame)


main : Program Value Model Msg
main =
    Navigation.programWithFlags (Route.fromLocation >> SetRoute)
        { init = init
        , view = view
        , update = update
        , subscriptions = subscriptions
        }



-- Pages -----------------------------------------------------------------------


type Page
    = Blank
    | NotFound
    | Errored PageLoadError
    | Login Login.Model
    | LocationSearch LocationSearch.Model
    | LocationDetail LocationDetail.Model
    | WarehouseSummary WarehouseSummary.Model
    | WarehouseWizard WarehouseWizard.Model
    | Home Home.Model


type PageState
    = Loaded Page
    | TransitioningFrom Page


page : PageState -> Page
page pageState =
    case pageState of
        Loaded page ->
            page

        TransitioningFrom page ->
            page



-- Model -----------------------------------------------------------------------


type alias Model =
    { navOpen : Bool
    , appConfig : AppConfig
    , session : Session
    , pageState : PageState
    }


init : Value -> Navigation.Location -> ( Model, Cmd Msg )
init val location =
    let
        ( appConfig, user ) =
            case Decode.decodeValue decodeFlags val of
                Ok { appConfig, user } ->
                    ( appConfig, user )

                _ ->
                    ( AppConfig.default, Nothing )

        model =
            { navOpen = False
            , pageState = Loaded initialPage
            , appConfig = appConfig
            , session = { user = user }
            }
    in
        setRoute (Route.fromLocation location) model


type alias Flags =
    { appConfig : AppConfig
    , user : Maybe User
    }


decodeFlags : Decode.Decoder Flags
decodeFlags =
    Decode.map2 Flags
        (Decode.field "appConfig" AppConfig.decoder)
        (Decode.field "user"
            (Decode.map
                (maybe Nothing (Decode.decodeString User.decoder >> Result.toMaybe))
             <|
                Decode.nullable Decode.string
            )
        )


initialPage : Page
initialPage =
    Blank



-- Update ----------------------------------------------------------------------


type Msg
    = SetRoute (Maybe Route)
    | SetUser (Maybe User)
    | ToggleNav
    | HomeLoaded (Result PageLoadError Home.Model)
    | HomeMsg Home.Msg
    | LocationSearchLoaded (Result PageLoadError LocationSearch.Model)
    | LocationSearchMsg LocationSearch.Msg
    | LocationDetailLoaded (Result PageLoadError LocationDetail.Model)
    | LocationDetailMsg LocationDetail.Msg
    | WarehouseSummaryLoaded (Result PageLoadError WarehouseSummary.Model)
    | WarehouseSummaryMsg WarehouseSummary.Msg
    | WarehouseWizardLoaded (Result PageLoadError WarehouseWizard.Model)
    | WarehouseWizardMsg WarehouseWizard.Msg
    | LoginMsg Login.Msg


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    updatePage (page model.pageState) msg model


updatePage : Page -> Msg -> Model -> ( Model, Cmd Msg )
updatePage page msg model =
    let
        session =
            model.session

        appConfig =
            model.appConfig

        toPage toModel toMsg subUpdate subMsg subModel =
            let
                ( newModel, newCmd ) =
                    subUpdate subMsg subModel
            in
                ( { model | pageState = Loaded (toModel newModel) }, Cmd.map toMsg newCmd )

        errored =
            pageErrored model
    in
        case ( msg, page ) of
            -- Home page
            ( HomeLoaded (Ok subModel), _ ) ->
                { model | pageState = Loaded (Home subModel) } => Cmd.none

            ( HomeLoaded (Err error), _ ) ->
                { model | pageState = Loaded (Errored error) } => Cmd.none

            ( HomeMsg subMsg, Home subModel ) ->
                toPage Home
                    HomeMsg
                    (Home.update appConfig session)
                    subMsg
                    subModel

            -- Location search
            ( LocationSearchLoaded (Ok subModel), _ ) ->
                { model | pageState = Loaded (LocationSearch subModel) } => Cmd.none

            ( LocationSearchLoaded (Err error), _ ) ->
                { model | pageState = Loaded (Errored error) } => Cmd.none

            ( LocationSearchMsg subMsg, LocationSearch subModel ) ->
                toPage LocationSearch
                    LocationSearchMsg
                    (LocationSearch.update appConfig session)
                    subMsg
                    subModel

            ( LocationDetailLoaded (Ok subModel), _ ) ->
                { model | pageState = Loaded (LocationDetail subModel) } => Cmd.none

            ( LocationDetailLoaded (Err error), _ ) ->
                { model | pageState = Loaded (Errored error) } => Cmd.none

            ( LocationDetailMsg subMsg, LocationDetail subModel ) ->
                toPage LocationDetail
                    LocationDetailMsg
                    (LocationDetail.update appConfig session)
                    subMsg
                    subModel

            -- Warehouse
            ( WarehouseSummaryLoaded (Ok subModel), _ ) ->
                { model | pageState = Loaded (WarehouseSummary subModel) } => Cmd.none

            ( WarehouseSummaryLoaded (Err error), _ ) ->
                { model | pageState = Loaded (Errored error) } => Cmd.none

            ( WarehouseSummaryMsg subMsg, WarehouseSummary subModel ) ->
                toPage WarehouseSummary
                    WarehouseSummaryMsg
                    (WarehouseSummary.update appConfig session)
                    subMsg
                    subModel

            ( WarehouseWizardLoaded (Ok subModel), _ ) ->
                { model | pageState = Loaded (WarehouseWizard subModel) } => Cmd.none

            ( WarehouseWizardLoaded (Err error), _ ) ->
                { model | pageState = Loaded (Errored error) } => Cmd.none

            ( WarehouseWizardMsg subMsg, WarehouseWizard subModel ) ->
                toPage WarehouseWizard
                    WarehouseWizardMsg
                    (WarehouseWizard.update appConfig session)
                    subMsg
                    subModel

            ( ToggleNav, _ ) ->
                { model | navOpen = not model.navOpen } => Cmd.none

            ( SetRoute route, _ ) ->
                setRoute route model

            ( SetUser user, _ ) ->
                let
                    session =
                        model.session

                    cmd =
                        -- If we just signed out, then redirect to Home.
                        if session.user /= Nothing && user == Nothing then
                            Route.modifyUrl Route.Login
                        else
                            Cmd.none
                in
                    { model | session = { session | user = user } }
                        => cmd

            ( _, NotFound ) ->
                -- Disregard incoming messages when we're on the
                -- NotFound page.
                model => Cmd.none

            ( _, _ ) ->
                -- Disregard incoming messages that arrived for the wrong page
                model => Cmd.none


setRoute : Maybe Route -> Model -> ( Model, Cmd Msg )
setRoute maybeRoute model =
    let
        transition toMsg task =
            { model | pageState = TransitioningFrom (page model.pageState) }
                => Task.attempt toMsg task

        errored =
            pageErrored model
    in
        case maybeRoute of
            Nothing ->
                { model | pageState = Loaded NotFound } => Cmd.none

            Just (Route.EvidenceBuilder occupancy buildingId) ->
                case occupancy of
                    Occupancy.Warehouse ->
                        transition WarehouseWizardLoaded <|
                            WarehouseWizard.init model.appConfig model.session buildingId

            Just (Route.BuildingSummary occupancy buildingId) ->
                case occupancy of
                    Occupancy.Warehouse ->
                        transition WarehouseSummaryLoaded <|
                            WarehouseSummary.init model.appConfig model.session buildingId

            Just (Route.LocationSearch search) ->
                transition LocationSearchLoaded <|
                    LocationSearch.init model.appConfig model.session search

            Just (Route.LocationDetail locationId) ->
                transition LocationDetailLoaded <|
                    LocationDetail.init model.appConfig model.session locationId

            Just (Route.Home) ->
                transition HomeLoaded <|
                    Home.init model.appConfig model.session

            Just (Route.Login) ->
                { model | pageState = Loaded (Login Login.initialModel) } => Cmd.none

            Just (Route.Logout) ->
                let
                    session =
                        model.session
                in
                    { model | session = { session | user = Nothing } }
                        => Cmd.batch
                            [ Ports.storeSession Nothing
                            , Route.modifyUrl Route.Login
                            ]


pageErrored : Model -> ActivePage -> String -> ( Model, Cmd msg )
pageErrored model activePage errorMessage =
    let
        error =
            pageLoadError activePage errorMessage
    in
        { model | pageState = Loaded (Errored error) } => Cmd.none



-- Subscription ----------------------------------------------------------------


subscriptions : Model -> Sub Msg
subscriptions model =
    Sub.batch
        [ pageSubscriptions (page model.pageState)
        , Sub.map SetUser sessionChange
        ]


sessionChange : Sub (Maybe User)
sessionChange =
    Ports.onSessionChange (Decode.decodeValue User.decoder >> Result.toMaybe)


pageSubscriptions : Page -> Sub Msg
pageSubscriptions page =
    case page of
        Blank ->
            Sub.none

        Errored _ ->
            Sub.none

        NotFound ->
            Sub.none

        Login _ ->
            Sub.none

        Home model ->
            Sub.map HomeMsg <|
                Home.subscriptions model

        LocationSearch model ->
            Sub.map LocationSearchMsg <|
                LocationSearch.subscriptions model

        LocationDetail model ->
            Sub.map LocationDetailMsg <|
                LocationDetail.subscriptions model

        WarehouseSummary model ->
            Sub.map WarehouseSummaryMsg <|
                WarehouseSummary.subscriptions model

        WarehouseWizard model ->
            Sub.map WarehouseWizardMsg <|
                WarehouseWizard.subscriptions model



-- View ------------------------------------------------------------------------


view : Model -> Html Msg
view model =
    case model.pageState of
        Loaded page ->
            viewPage model.appConfig model.session model.navOpen False page

        TransitioningFrom page ->
            viewPage model.appConfig model.session model.navOpen True page


viewPage : AppConfig -> { user : Maybe User } -> a -> Bool -> Page -> Html Msg
viewPage appConfig session navOpen isLoading page =
    let
        pageFrame =
            frame isLoading session.user
    in
        case page of
            NotFound ->
                pageFrame Frame.Other <|
                    NotFound.view session

            Blank ->
                pageFrame Frame.Other <| H.text "loading"

            --spinner
            Errored model ->
                pageFrame Frame.Other <|
                    Errored.view session model

            LocationSearch model ->
                pageFrame Frame.LocationSearch <|
                    H.map LocationSearchMsg <|
                        LocationSearch.view appConfig session model

            LocationDetail model ->
                pageFrame Frame.LocationSearch <|
                    H.map LocationDetailMsg <|
                        LocationDetail.view appConfig session model

            WarehouseSummary model ->
                pageFrame Frame.LocationSearch <|
                    H.map WarehouseSummaryMsg <|
                        WarehouseSummary.view appConfig session model

            WarehouseWizard model ->
                pageFrame Frame.LocationSearch <|
                    H.map WarehouseWizardMsg <|
                        WarehouseWizard.view appConfig session model

            Home subModel ->
                pageFrame Frame.Home <|
                    H.map HomeMsg <|
                        Home.view appConfig session subModel

            Login subModel ->
                pageFrame Frame.Other <|
                    H.map LoginMsg <|
                        Login.view appConfig session subModel
