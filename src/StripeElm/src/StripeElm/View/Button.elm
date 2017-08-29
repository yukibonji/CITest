module View.Button exposing (button, anchorButton, intentButton, anchorIntentButton)

import Html as H exposing (Html, Attribute)
import Html.Attributes as Attr
import View.Intent as Intent exposing (Intent(..))


button : Bool -> Bool -> List (Attribute msg) -> List (Html msg) -> Html msg
button =
    buttonHelper Nothing


anchorButton : Bool -> Bool -> List (Attribute msg) -> List (Html msg) -> Html msg
anchorButton =
    anchorButtonHelper Nothing


intentButton :
    Intent
    -> Bool
    -> Bool
    -> List (Attribute msg)
    -> List (Html msg)
    -> Html msg
intentButton intent =
    buttonHelper (Just intent)


anchorIntentButton :
    Intent
    -> Bool
    -> Bool
    -> List (Attribute msg)
    -> List (Html msg)
    -> Html msg
anchorIntentButton intent =
    anchorButtonHelper (Just intent)


buttonHelper : Maybe Intent -> Bool -> Bool -> List (Attribute msg) -> List (Html msg) -> Html msg
buttonHelper intent isActive isDisabled attrs content =
    let
        intentClass =
            case intent of
                Nothing ->
                    ( "", False )

                Just intent ->
                    ( "c-button" ++ Intent.toString intent, True )
    in
        H.button
            (Attr.classList
                [ ( "c-button", True )
                , ( "c-button--active", isActive )
                , intentClass
                ]
                :: Attr.disabled isDisabled
                :: attrs
            )
            content


anchorButtonHelper : Maybe Intent -> Bool -> Bool -> List (Attribute msg) -> List (Html msg) -> Html msg
anchorButtonHelper intent isActive isDisabled attrs content =
    let
        intentClass =
            case intent of
                Nothing ->
                    ( "", False )

                Just intent ->
                    ( "c-button" ++ Intent.toString intent, True )
    in
        H.a
            (Attr.classList
                [ ( "c-button", True )
                , ( "c-button--active", isActive )
                , ( "c-button-disabled", isDisabled )
                , intentClass
                ]
                :: attrs
            )
            content
