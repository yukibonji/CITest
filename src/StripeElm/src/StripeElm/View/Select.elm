module View.Select exposing (view, Config)

import Json.Decode as Decode
import Html exposing (div, select, option, text, Html, Attribute)
import Html.Attributes exposing (class, selected, value)
import Html.Events exposing (on, targetValue)


type alias Config data error msg =
    { toString : data -> String
    , toValue : data -> String
    , fromValue : String -> Result error data
    , emptyElement : Maybe String
    , onChange : Maybe data -> msg
    , attributes : List (Attribute msg)
    }


view : Config data error msg -> List data -> Maybe data -> Html msg
view config items maybeSelectedItem =
    select
        (class "c-field"
            :: onChange config.fromValue config.onChange
            :: config.attributes
        )
        (case config.emptyElement of
            Just emptyStr ->
                viewEmptyElement emptyStr maybeSelectedItem
                    :: viewItems config items maybeSelectedItem

            _ ->
                viewItems config items maybeSelectedItem
        )


viewEmptyElement : String -> Maybe a -> Html msg
viewEmptyElement emptyStr maybeSelectedItem =
    let
        isSelected =
            case maybeSelectedItem of
                Nothing ->
                    True

                _ ->
                    False
    in
        option
            [ value emptyStr
            , selected isSelected
            ]
            [ text emptyStr ]


viewItems : Config data error msg -> List data -> Maybe data -> List (Html msg)
viewItems config items maybeSelectedItem =
    case maybeSelectedItem of
        Just selectedItem ->
            List.map
                (viewItem config ((==) selectedItem))
                items

        _ ->
            List.map
                (viewItem config (always False))
                items


viewItem : Config data error msg -> (data -> Bool) -> data -> Html msg
viewItem config isSelected item =
    option
        [ value <| config.toValue item
        , selected <| isSelected item
        ]
        [ text <| config.toString item ]


onChange : (String -> Result error a) -> (Maybe a -> b) -> Attribute b
onChange fromValue toMsg =
    on "change" <|
        Decode.andThen
            (\str ->
                case fromValue str of
                    Ok b ->
                        Decode.succeed <| toMsg (Just b)

                    Err err ->
                        Decode.succeed <| toMsg Nothing
            )
            targetValue
