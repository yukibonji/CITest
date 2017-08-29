module Page.Location.Search
    exposing
        ( view
        , update
        , Model
        , Msg
        , init
        , subscriptions
        )

{-| Location search page.
-}

import Dict
import Http
import Task exposing (Task)
import Color exposing (Color)
import Html as H exposing (Html, text, div, span, strong, a, i, img, p, hr, nav, br, small, figure, article, button, li, ul, header)
import Html.Attributes as A exposing (class, href, src, attribute, alt, title, placeholder)
import Route
import Query.Location
import Data.Address as Address
import Data.LocationFilter as LocationFilter exposing (LocationFilter)
import Data.Location exposing (LocationSummary, LocationId)
import Data.Country exposing (Country)
import Data.Session exposing (Session)
import Data.Building.Ownership as Ownership exposing (Ownership)
import Data.Building.Occupancy as Occupancy exposing (Occupancy)
import Data.Building.QualitativeRating as QualitativeRating exposing (QualitativeRating)
import Data.Range as Range exposing (Range)
import Data.Currency as Currency exposing (Currency)
import Data.AppConfig exposing (AppConfig)
import View.Frame as Frame
import View.Icon as Icon exposing (Direction(Right))
import View.Autocomplete as Autocomplete
import View.HtmlDetails exposing (HtmlDetails)
import Page.Errored as Errored exposing (PageLoadError, pageLoadError)
import Helpers exposing ((=>), pluralize)


-- Model -----------------------------------------------------------------------


type alias Model =
    { errors : List String
    , advancedSearchOpen : Bool
    , countries : List Country
    , countryQuery : String
    , countryAutocompleteState : Autocomplete.State
    , filter : LocationFilter
    , locations : List LocationSummary
    }


init :
    AppConfig
    -> Session
    -> Maybe LocationFilter
    -> Task PageLoadError Model
init appConfig session maybeFilter =
    let
        maybeAuthToken =
            Maybe.map .token session.user

        loadLocations =
            Http.toTask <|
                Query.Location.all appConfig maybeAuthToken filter

        handleLoadError err =
            pageLoadError Frame.Other <| "locations are currently unavailable"

        filter =
            Maybe.withDefault LocationFilter.empty maybeFilter

        initialModel =
            Model
                []
                False
                (List.map Tuple.second <| Dict.toList Data.Country.alpha2)
                ""
                Autocomplete.empty
                filter
    in
        Task.mapError handleLoadError <|
            Task.map initialModel loadLocations



-- Update ----------------------------------------------------------------------


type Msg
    = ToggleAdvancedSearch
    | ToggleLocationFavourite LocationId
    | NextPage
    | PreviousPage
    | GoToPage Int
    | SetLimit Int
    | AutocompleteMsg Autocomplete.Msg
    | SetAutoCompleteQuery String
    | AddCountry Country
    | RemoveCountry Country
    | RemoveOwnership Ownership
    | AddOwnership Ownership
    | RemoveOccupancy Occupancy
    | AddOccupancy Occupancy
    | AddSiteCondition QualitativeRating
    | RemoveSiteCondition QualitativeRating
    | AddPlantLayout QualitativeRating
    | RemovePlantLayout QualitativeRating
    | UpdateTotalAreaRange Range.Endpoint Float
    | UpdateTotalInsuredValueRange Range.Endpoint Float
    | UpdateTotalInsuredValueCurrency Currency
    | UpdateSearchString String
    | UpdatePostcode String
    | UpdateSearchRadius Float
    | ToggleFavouritesOnly


update : AppConfig -> Session -> Msg -> Model -> ( Model, Cmd Msg )
update appConfig session msg model =
    let
        cmd filter =
            Route.modifyUrl
                << Route.LocationSearch
            <|
                Just filter

        remove x xs =
            List.filter ((/=) x) xs

        add x xs =
            if List.member x xs then
                xs
            else
                x :: xs
    in
        case msg of
            AutocompleteMsg subMsg ->
                let
                    ( state, maybeMsg ) =
                        Autocomplete.update
                            autoCompleteUpdateConfig
                            subMsg
                            model.countryAutocompleteState
                            model.countries

                    newModel =
                        { model | countryAutocompleteState = state }
                in
                    case maybeMsg of
                        Just msg ->
                            update appConfig session msg newModel

                        _ ->
                            newModel => Cmd.none

            SetAutoCompleteQuery query ->
                { model | countryQuery = query } => Cmd.none

            ToggleAdvancedSearch ->
                { model
                    | advancedSearchOpen = not model.advancedSearchOpen
                }
                    => Cmd.none

            ToggleLocationFavourite locationId ->
                let
                    locations =
                        model.locations
                            |> List.map
                                (\loc ->
                                    if loc.id == locationId then
                                        { loc | isFavourite = not loc.isFavourite }
                                    else
                                        loc
                                )

                    -- TODO - integrate with write model
                in
                    { model | locations = locations } => Cmd.none

            -- Search update
            AddCountry country ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | country =
                                add country filter.country
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            RemoveCountry country ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | country =
                                remove country filter.country
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            AddOwnership ownership ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | ownership =
                                add ownership filter.ownership
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            RemoveOwnership ownership ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | ownership =
                                remove ownership filter.ownership
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            AddOccupancy occupancy ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | occupancy =
                                add occupancy filter.occupancy
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            RemoveOccupancy occupancy ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | occupancy =
                                remove occupancy filter.occupancy
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            AddSiteCondition rating ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | siteCondition =
                                add rating filter.siteCondition
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            RemoveSiteCondition rating ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | siteCondition =
                                remove rating filter.siteCondition
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            AddPlantLayout rating ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | plantLayout =
                                add rating filter.plantLayout
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            RemovePlantLayout rating ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | plantLayout =
                                remove rating filter.plantLayout
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            UpdateTotalAreaRange endpoint value ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | totalArea =
                                Range.update
                                    endpoint
                                    value
                                    filter.totalArea
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            UpdateTotalInsuredValueRange endpoint value ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | totalInsuredValue =
                                ( Tuple.first filter.totalInsuredValue
                                , Range.update
                                    endpoint
                                    value
                                    (Tuple.second filter.totalInsuredValue)
                                )
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            UpdateTotalInsuredValueCurrency ccy ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter
                            | totalInsuredValue =
                                ( ccy
                                , Tuple.second filter.totalInsuredValue
                                )
                        }
                in
                    { model | filter = newFilter } => cmd newFilter

            UpdateSearchString str ->
                -- TODO - what is this actually for??
                model => Cmd.none

            UpdatePostcode postdode ->
                -- TODO - integrate with Google Maps API
                model => Cmd.none

            UpdateSearchRadius radius ->
                -- TODO - integrate with Google Maps API
                model => Cmd.none

            ToggleFavouritesOnly ->
                let
                    filter =
                        model.filter

                    newFilter =
                        { filter | favouritesOnly = not filter.favouritesOnly }
                in
                    { model | filter = newFilter } => cmd newFilter

            --  TODO : Pagination
            NextPage ->
                model => Cmd.none

            PreviousPage ->
                model => Cmd.none

            GoToPage n ->
                model => Cmd.none

            SetLimit limit ->
                model => Cmd.none



-- Autocomplete ----------------------------------------------------------------


autoCompleteUpdateConfig : Autocomplete.UpdateConfig Msg Country
autoCompleteUpdateConfig =
    { toId = (\country -> country.alpha2)
    , onChangeQuery = SetAutoCompleteQuery >> Just
    , onAddItem = Maybe.map AddCountry << countryFromAlpha2
    , onRemoveItem = Maybe.map RemoveCountry << countryFromAlpha2
    }


countryFromAlpha2 : String -> Maybe Country
countryFromAlpha2 alpha2 =
    Dict.get alpha2 Data.Country.alpha2


autocompleteViewConfig : Autocomplete.ViewConfig Country
autocompleteViewConfig =
    { placeholder = "Geography"
    , inputAttributes = []
    , tagAttributes = []
    , viewTagItem = viewTagItem
    , menuAttributes = []
    , viewMenuItem = viewCountryAutocompleteMenuItem
    , menuItemLimit = Nothing
    , toId = countryId
    }


countryId : Country -> String
countryId { alpha2 } =
    alpha2


viewTagItem : Country -> HtmlDetails Never
viewTagItem { name } =
    { attributes = []
    , children = [ H.text name ]
    }


viewCountryAutocompleteMenuItem : Bool -> Bool -> Country -> HtmlDetails Never
viewCountryAutocompleteMenuItem keySelected mouseSelected country =
    { attributes = []
    , children = [ H.text country.name ]
    }



-- Subscriptions ---------------------------------------------------------------


subscriptions : Model -> Sub Msg
subscriptions model =
    Sub.batch
        [ Sub.map AutocompleteMsg Autocomplete.subscription ]



-- View ------------------------------------------------------------------------


view : AppConfig -> Session -> Model -> Html Msg
view appConfig session model =
    H.div
        [ class "l-container l-container--vertical" ]
        [ viewFilterPanel model
        , viewLocationPanel model.locations
        ]



-- Location Search View --------------------------------------------------------


viewFilterPanel : Model -> Html Msg
viewFilterPanel { filter, countryAutocompleteState, countries } =
    H.div
        [ class "c-simple-panel" ]
        [ H.div
            [ class "o-grid o-grid--small-full" ]
            [ H.div
                [ class "o-grid__cell" ]
                [ viewInput ]
            , H.div
                [ class "o-grid__cell" ]
                [ H.map AutocompleteMsg <|
                    Autocomplete.view
                        autocompleteViewConfig
                        countryAutocompleteState
                        countries
                        filter.country
                ]
            , H.div
                [ class "o-grid__cell" ]
                [ H.select
                    [ class "c-field" ]
                    [ H.option [] [ H.text "test" ]
                    , H.option [] [ H.text "test" ]
                    , H.option [] [ H.text "test" ]
                    ]
                ]
            ]
        ]


viewInput : Html msg
viewInput =
    H.div
        [ class "c-input-group" ]
        [ H.div
            [ class "o-field o-field--icon-left" ]
            [ H.div [ class "c-icon" ]
                [ Icon.view <|
                    Icon.MyLocation { size = 22, color = Color.rgb 157 175 189 }
                ]
            , H.input
                [ class "c-field"
                , placeholder "postal code"
                ]
                []
            ]
        ]



-- Location Results View -------------------------------------------------------


viewLocationPanel :
    List LocationSummary
    -> Html msg
viewLocationPanel locations =
    H.div
        [ class "c-simple-panel"
        ]
        [ H.nav
            [ class "c-navbar" ]
            [ H.div
                [ class "c-navbar__navgroup " ]
                [ H.text <| pluralize "location" "locations" (List.length locations)
                ]
            , H.div
                [ class "c-navbar__navgroup " ]
                [ H.a
                    [ A.classList
                        [ ( "c-navbar__navicon", True )
                        , ( "c-tooltip", True )
                        , ( "c-tooltip--top", True )
                        ]
                    , attribute "aria-label" ("New Location")
                    ]
                    [ Icon.view <|
                        Icon.AddLocation
                            { size = 40, color = Color.rgb 29 188 156 }
                    ]
                ]
            ]
        , H.div [ class ".card-container" ]
            (List.map viewLocation locations)
        ]


viewLocation :
    LocationSummary
    -> Html msg
viewLocation locationSummary =
    H.div
        [ class "c-card c-card--location-detail " ]
        [ H.div
            [ class "c-card__body" ]
            [ H.h2 [ class "c-card__title" ] [ H.text locationSummary.name ]
            , H.p [ class "c-card__subtitle" ] [ H.text <| Address.summary locationSummary.address ]
            ]
        , H.div
            [ class "c-card__description" ]
            [ H.p [ class "c-card__intro" ] [ H.text locationSummary.description ]
            ]
        , H.footer
            [ class "c-card__footer c-card--location-footer c-navbar" ]
            [ H.div
                [ class "c-navbar__navgroup" ]
                [ H.div
                    [ A.classList
                        [ ( "c-navbar__navicon", True )
                        , ( "c-tooltip", True )
                        , ( "c-tooltip--bottom", True )
                        ]
                    , attribute "aria-label" ("Bookmark Location")
                    ]
                    [ Icon.view <|
                        Icon.Bookmark
                            { size = 26
                            , color =
                                Color.rgb 255 255 255
                                --29 188 156
                            , filled = locationSummary.isFavourite
                            }
                    ]
                ]
            , H.div
                [ class "c-navbar__navgroup " ]
                [ H.a
                    [ A.classList
                        [ ( "c-navbar__navicon", True )
                        , ( "c-tooltip", True )
                        , ( "c-tooltip--bottom", True )
                        ]
                    , attribute "aria-label" ("View Location")
                    , Route.href <| Route.LocationDetail locationSummary.id
                    ]
                    [ Icon.view <|
                        Icon.Chevron
                            { size = 38
                            , color = Color.rgb 8 65 80
                            , direction = Right
                            }
                    ]
                ]
            ]
        ]
