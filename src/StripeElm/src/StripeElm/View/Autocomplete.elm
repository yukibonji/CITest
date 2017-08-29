module View.Autocomplete
    exposing
        ( view
        , State
        , Msg
        , empty
        , update
        , subscription
        , ViewConfig
        , UpdateConfig
        )

import Html exposing (Html, Attribute, div, input)
import Html.Attributes exposing (class, placeholder, type_)
import Html.Events exposing (onInput)
import Char exposing (KeyCode)
import View.HtmlDetails exposing (HtmlDetails)
import Helpers exposing (maybe, (=>))
import View.Menu as Menu
import View.Tag as Tag


-- Model -----------------------------------------------------------------------


type State
    = State { query : String, menuState : Menu.State, showMenu : Bool }


empty : State
empty =
    State { query = "", menuState = Menu.empty, showMenu = False }



-- Update ----------------------------------------------------------------------


type Msg
    = ChangeQuery String
    | AddItem String
    | RemoveItem String
    | MenuMsg Menu.Msg
    | NoOp


type alias UpdateConfig msg data =
    { toId : data -> String
    , onChangeQuery : String -> Maybe msg
    , onAddItem : String -> Maybe msg
    , onRemoveItem : String -> Maybe msg
    }


update : UpdateConfig msg data -> Msg -> State -> List data -> ( State, Maybe msg )
update updateConfig msg (State state) items =
    case msg of
        ChangeQuery query ->
            State { state | query = query } => updateConfig.onChangeQuery query

        AddItem id ->
            State state => updateConfig.onAddItem id

        RemoveItem id ->
            State state => updateConfig.onRemoveItem id

        MenuMsg subMsg ->
            let
                ( menuState, msg ) =
                    Menu.update (menuUpdateConfig updateConfig) subMsg state.menuState items
            in
                State { state | menuState = menuState } => msg

        NoOp ->
            State state => Nothing



-- View ------------------------------------------------------------------------


type alias ViewConfig data =
    { placeholder : String
    , inputAttributes : List (Attribute Never)
    , tagAttributes : List (Attribute Never)
    , viewTagItem : data -> HtmlDetails Never
    , menuAttributes : List (Attribute Never)
    , viewMenuItem : Bool -> Bool -> data -> HtmlDetails Never
    , menuItemLimit : Maybe Int
    , toId : data -> String
    }


view : ViewConfig data -> State -> List data -> List data -> Html Msg
view viewConfig (State state) data selected =
    let
        attrs =
            List.map neverAttribute viewConfig.inputAttributes
    in
        div
            [ class "c-multiselect" ]
            [ div
                [ class "o-field" ]
                (List.append
                    (List.map (viewTag viewConfig) selected)
                    [ input
                        (class "c-field"
                            :: placeholder viewConfig.placeholder
                            :: type_ "text"
                            :: onInput ChangeQuery
                            :: attrs
                        )
                        []
                    ]
                )
            ]


viewTag : ViewConfig data -> data -> Html Msg
viewTag viewConfig item =
    Tag.view (tagConfig viewConfig) item



-- Tag -------------------------------------------------------------------------


tagConfig : ViewConfig data -> Tag.Config Msg data
tagConfig { tagAttributes, viewTagItem, toId } =
    { onDismiss = Just << RemoveItem
    , attributes = List.map neverAttribute tagAttributes
    , toId = toId
    , viewItem = neverHelper << viewTagItem
    }


neverHelper : HtmlDetails Never -> HtmlDetails Msg
neverHelper { attributes, children } =
    { attributes = List.map neverAttribute attributes
    , children = List.map neverHtml children
    }


neverAttribute : Attribute b -> Attribute Msg
neverAttribute =
    Html.Attributes.map (always NoOp)


neverHtml : Html b -> Html Msg
neverHtml =
    Html.map (always NoOp)



-- Menu ------------------------------------------------------------------------


menuUpdateConfig : UpdateConfig msg data -> Menu.UpdateConfig msg data
menuUpdateConfig { toId, onAddItem } =
    { onKeyDown = onEnter onAddItem
    , onTooLow = Nothing
    , onTooHigh = Nothing
    , onMouseEnter = always Nothing
    , onMouseLeave = always Nothing
    , onMouseClick = onAddItem
    , toId = toId
    , separateSelections = True
    }


menuViewConfig : ViewConfig data -> Menu.ViewConfig data
menuViewConfig { toId, menuAttributes, viewMenuItem, menuItemLimit } =
    { toId = toId
    , attributes = menuAttributes
    , viewItem = viewMenuItem
    , itemLimit = menuItemLimit
    }


onEnter : (String -> Maybe msg) -> KeyCode -> Maybe String -> Maybe msg
onEnter onSelectItem keyCode str =
    if keyCode == 13 then
        Maybe.andThen onSelectItem str
    else
        Nothing



-- Subscriptions ---------------------------------------------------------------


subscription : Sub Msg
subscription =
    Sub.map MenuMsg Menu.subscription
