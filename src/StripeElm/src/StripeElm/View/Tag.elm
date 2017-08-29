module View.Tag exposing (view, Config)

import Color
import Html exposing (Html, Attribute)
import Html.Attributes exposing (classList)
import Html.Events exposing (onClick)
import View.HtmlDetails exposing (HtmlDetails)
import View.Icon as Icon
import View.Button as Button
import Helpers


type alias Config msg data =
    { onDismiss : String -> Maybe msg
    , attributes : List (Attribute msg)
    , toId : data -> String
    , viewItem : data -> HtmlDetails msg
    }


view : Config msg data -> data -> Html msg
view { onDismiss, attributes, toId, viewItem } item =
    let
        canDismiss =
            Helpers.isJust msg

        htmlDetails =
            viewItem item

        msg =
            onDismiss <| toId item
    in
        Html.div
            (classList [ ( "c-tag", True ), ( "c-tag--dismissable", canDismiss ) ]
                :: attributes
            )
        <|
            List.filterMap identity
                [ Just <| viewContent htmlDetails
                , Maybe.map viewDismissButton msg
                ]


viewContent : HtmlDetails msg -> Html msg
viewContent { attributes, children } =
    Html.div attributes children


viewDismissButton : msg -> Html msg
viewDismissButton msg =
    Button.button
        False
        False
        [ onClick msg ]
        [ Icon.view <|
            Icon.Close
                { size = 14
                , color = Color.rgb 0 0 0
                }
        ]
