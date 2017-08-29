module View.HtmlDetails exposing (HtmlDetails)

import Html exposing (Html, Attribute)


type alias HtmlDetails msg =
    { attributes : List (Attribute msg)
    , children : List (Html msg)
    }
