module Page.Errored exposing (view, pageLoadError, PageLoadError)

{-| The page that renders when there was an error trying to load another page,
for example a Page Not Found error.
-}

import Color exposing (Color)
import Html exposing (Html, main_, h4, div, img, text, p, br)
import Html.Attributes exposing (class, style, tabindex, id, alt)
import Data.Session as Session exposing (Session)
import View.Frame exposing (ActivePage)
import View.Icon as Icon


-- MODEL --


type PageLoadError
    = PageLoadError Model


type alias Model =
    { activePage : ActivePage
    , errorMessage : String
    }


pageLoadError : ActivePage -> String -> PageLoadError
pageLoadError activePage errorMessage =
    PageLoadError { activePage = activePage, errorMessage = errorMessage }



-- VIEW --


view : Session -> PageLoadError -> Html msg
view session (PageLoadError model) =
    div
        [ style
            [ ( "display", "flex" )
            , ( "align-items", "center" )
            , ( "justify-content", "center" )
            ]
        ]
        [ div
            [ style
                [ ( "max-width", "50%" )
                , ( "margin-top", "60px" )
                , ( "display", "flex" )
                , ( "flex-direction", "column" )
                , ( "align-items", "center" )
                , ( "justify-content", "center" )
                ]
            ]
            [ Icon.view <| Icon.Warning { size = 60, color = Color.rgb 125 125 125 }
            , br [] []
            , h4 [] [ text "Error Loading Page" ]
            , div [] [ p [] [ text model.errorMessage ] ]
            ]
        ]
