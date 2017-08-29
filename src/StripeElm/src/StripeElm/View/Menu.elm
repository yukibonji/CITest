module View.Menu exposing (view, update, Msg, State, empty, subscription, ViewConfig, UpdateConfig)

import Html exposing (Html, Attribute)
import Html.Attributes exposing (class)
import Html.Keyed
import Html.Events exposing (onMouseEnter, onMouseLeave, onClick)
import Char exposing (KeyCode)
import Keyboard
import View.HtmlDetails exposing (HtmlDetails)
import Helpers exposing (maybe)


type State
    = State { keySelected : Maybe String, mouseSelected : Maybe String }


empty : State
empty =
    State { keySelected = Nothing, mouseSelected = Nothing }



-- Update  ---------------------------------------------------------------------


type alias UpdateConfig msg data =
    { onKeyDown : KeyCode -> Maybe String -> Maybe msg
    , onTooLow : Maybe msg
    , onTooHigh : Maybe msg
    , onMouseEnter : String -> Maybe msg
    , onMouseLeave : String -> Maybe msg
    , onMouseClick : String -> Maybe msg
    , toId : data -> String
    , separateSelections : Bool
    }


type Msg
    = KeyDown KeyCode
    | WentTooLow
    | WentTooHigh
    | MouseEnter String
    | MouseLeave String
    | MouseClick String
    | NoOp


update : UpdateConfig msg data -> Msg -> State -> List data -> ( State, Maybe msg )
update config msg (State state) data =
    case msg of
        KeyDown keyCode ->
            let
                boundedList =
                    List.map config.toId data

                newKey =
                    navigateWithKey keyCode boundedList state.keySelected
            in
                if newKey == state.keySelected && keyCode == 38 then
                    update config WentTooHigh (State state) data
                else if newKey == state.keySelected && keyCode == 40 then
                    update config WentTooLow (State state) data
                else if config.separateSelections then
                    ( State { state | keySelected = newKey }
                    , config.onKeyDown keyCode newKey
                    )
                else
                    ( State { keySelected = newKey, mouseSelected = newKey }
                    , config.onKeyDown keyCode newKey
                    )

        WentTooLow ->
            ( State state
            , config.onTooLow
            )

        WentTooHigh ->
            ( State state
            , config.onTooHigh
            )

        MouseEnter id ->
            ( State <| resetMouseStateWithId config.separateSelections id state
            , config.onMouseEnter id
            )

        MouseLeave id ->
            ( State <| resetMouseStateWithId config.separateSelections id state
            , config.onMouseLeave id
            )

        MouseClick id ->
            ( State <| resetMouseStateWithId config.separateSelections id state
            , config.onMouseClick id
            )

        NoOp ->
            ( State state, Nothing )



-- View ------------------------------------------------------------------------


type alias ViewConfig data =
    { toId : data -> String
    , attributes : List (Attribute Never)
    , viewItem : Bool -> Bool -> data -> HtmlDetails Never
    , itemLimit : Maybe Int
    }


view : ViewConfig data -> State -> List data -> Html Msg
view viewConfig (State state) items =
    let
        itemsToShow =
            maybe items (\limit -> List.take limit items) viewConfig.itemLimit

        li item =
            ( viewConfig.toId item, viewItem viewConfig state item )
    in
        Html.Keyed.ul
            (class "c-menu" :: List.map neverToMsg viewConfig.attributes)
            (List.map li items)


viewItem :
    ViewConfig data
    -> { keySelected : Maybe String, mouseSelected : Maybe String }
    -> data
    -> Html Msg
viewItem viewConfig { keySelected, mouseSelected } item =
    let
        id =
            viewConfig.toId item

        htmlDetails =
            viewConfig.viewItem
                (maybe False ((==) id) keySelected)
                (maybe False ((==) id) mouseSelected)
                item

        attributes =
            class "c-menu__item"
                :: onMouseEnter (MouseEnter id)
                :: onMouseLeave (MouseLeave id)
                :: onClick (MouseClick id)
                :: List.map neverToMsg htmlDetails.attributes
    in
        Html.li attributes <|
            List.map (Html.map (always NoOp)) htmlDetails.children


neverToMsg : Attribute Never -> Attribute Msg
neverToMsg =
    Html.Attributes.map (always NoOp)



-- Subsscription ---------------------------------------------------------------


subscription : Sub Msg
subscription =
    Keyboard.downs KeyDown



-- Helpers ---------------------------------------------------------------------


resetMouseStateWithId :
    Bool
    -> a
    -> { c | keySelected : Maybe a, mouseSelected : b }
    -> { keySelected : Maybe a, mouseSelected : Maybe a }
resetMouseStateWithId separateSelections id { keySelected, mouseSelected } =
    if separateSelections then
        { keySelected = keySelected, mouseSelected = Just id }
    else
        { keySelected = Just id, mouseSelected = Just id }


navigateWithKey : Int -> List String -> Maybe String -> Maybe String
navigateWithKey keyCode ids maybeId =
    case keyCode of
        38 ->
            Maybe.map (previous Nothing ids) maybeId

        40 ->
            Maybe.map (next ids) maybeId

        _ ->
            maybeId


previous : Maybe a -> List a -> a -> a
previous prev list target =
    case list of
        [] ->
            Maybe.withDefault target prev

        next :: rest ->
            if next == target then
                Maybe.withDefault target prev
            else
                previous (Just next) rest target


next : List a -> a -> a
next list target =
    case list of
        [] ->
            target

        x :: xs ->
            if x == target then
                Maybe.withDefault target <| List.head xs
            else
                next xs target
