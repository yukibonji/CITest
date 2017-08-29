module Page.NotFound exposing (view)

import Html as H exposing (Html, Attribute)
import Html.Attributes as A
import Data.Session as Session exposing (Session)


-- import View.Assets as Assets
-- View ------------------------------------------------------------------------


view : Session -> Html msg
view session =
    H.main_ [ A.id "content", A.class "container", A.tabindex -1 ]
        [ H.h1 [] [ H.text "Not Found" ]
        , H.div [ A.class "row" ] [ H.text "Page not found" ]
          -- [ H.img [ Assets.src Assets.error, A.alt "Page not found." ] [] ]
        ]
