module View.Input exposing (view, Config)

import Html exposing (div, input, Html, Attribute)
import Html.Attributes exposing (class, placeholder, value)
import Html.Events exposing (onInput)


type alias Config data msg =
    { format : data -> String
    , placeholder : String
    , attributes : List (Attribute msg)
    , onInput : String -> msg
    }


view : Config data msg -> data -> Html msg
view config data =
    div
        [ class "o-field" ]
        [ Html.input
            (class "c-field"
                :: placeholder config.placeholder
                :: (value <| config.format data)
                :: onInput config.onInput
                :: config.attributes
            )
            []
        ]
