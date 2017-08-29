module View.Frame exposing (frame, ActivePage(..))

{-| The frame around a page
-}

import Color exposing (Color)
import Html exposing (..)
import Html.Attributes exposing (..)
import Data.User exposing (User)
import View.Icon as Icon exposing (Icon(Logo, Settings, User))
import Route


{-| Determines which navbar link (if any) will be rendered as active.

Note that we don't enumerate every page here, because the navbar doesn't
have links for every page. Anything that's not part of the navbar falls
under Other.

-}
type ActivePage
    = Other
    | Home
    | LocationSearch
    | Scenarios
    | Settings
    | Profile


{-| Take a page's Html and frame it with a header and footer.

The caller provides the current user, so we can display in either
"signed in" (rendering username) or "signed out" mode.

isLoading is for determining whether we should show a loading spinner
in the header. (This comes up during slow page transitions.)

-}
frame : Bool -> Maybe User -> ActivePage -> Html msg -> Html msg
frame isLoading user activePage content =
    div []
        [ viewHeader isLoading activePage
        , viewContent isLoading user content
        ]


viewHeader : Bool -> ActivePage -> Html msg
viewHeader isLoading activePage =
    header
        []
        [ nav
            [ class "c-navbar" ]
            [ div
                []
                [ a
                    [ class "c-navbar--logo"
                    , href "#"
                    ]
                    [ Icon.view (Logo { size = 36, color = Color.rgb 57 75 89 })
                    ]
                ]
            , div
                [ class "c-navbar__navgroup " ]
                [ a
                    [ classList
                        [ ( "c-navbar__navitem", True )
                        , ( "c-navbar__navitem--state-active", activePage == Home )
                        ]
                    , Route.href Route.Home
                    ]
                    [ text "home" ]
                , a
                    [ classList
                        [ ( "c-navbar__navitem", True )
                        , ( "c-navbar__navitem--state-active", activePage == LocationSearch )
                        ]
                    , Route.href <|
                        Route.LocationSearch Nothing
                    ]
                    [ text "locations" ]
                , a
                    [ classList
                        [ ( "c-navbar__navitem", True )
                        , ( "c-navbar__navitem--state-active", activePage == Scenarios )
                        ]
                    ]
                    [ text "scenarios" ]
                ]
            , div
                [ class "c-navbar__navgroup" ]
                [ a
                    [ classList
                        [ ( "c-navbar__navicon", True )
                        , ( "c-navbar__navitem--state-active", activePage == Settings )
                        , ( "c-tooltip", True )
                        , ( "c-tooltip--bottom", True )
                        ]
                    , attribute "aria-label" "settings"
                    ]
                    [ Icon.view (Icon.Settings { size = 26, color = Color.rgb 8 65 80 }) ]
                , a
                    [ classList
                        [ ( "c-navbar__navicon", True )
                        , ( "c-navbar__navitem--state-active", activePage == Profile )
                        , ( "c-tooltip", True )
                        , ( "c-tooltip--bottom", True )
                        ]
                    , attribute "aria-label" "profile"
                    ]
                    [ Icon.view (Icon.User { size = 26, color = Color.rgb 8 65 80 })
                    ]
                ]
            ]
        , div [ classList [ ( "c-loading", True ), ( "c-loading--active", isLoading ) ] ]
            []
        ]


viewContent : a -> b -> Html msg -> Html msg
viewContent isLoading user content =
    div [ class "c-app" ]
        [ div
            [ class "l-container l-container--vertical" ]
            [ content ]
        ]
