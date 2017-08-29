module Page.Login exposing (view, update, Model, Msg, initialModel, ExternalMsg(..))

{-| The login page.
-}

import Tuple exposing (first, second)
import Http
import Json.Decode as Decode exposing (field, decodeString, string, Decoder)
import Json.Decode.Pipeline as Pipeline exposing (optional, decode)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Validate exposing (..)
import Input.Text as Text
import Helpers exposing ((=>), isJust)
import Route exposing (Route)
import Data.AppConfig exposing (AppConfig)
import Data.Session as Session exposing (Session)
import Data.User as User exposing (User)
import Query.User exposing (storeSession)


-- MODEL --


type alias Model =
    { errors : List Error
    , email : String
    , password : String
    }


initialModel : Model
initialModel =
    { errors = []
    , email = ""
    , password = ""
    }



-- VIEW --


view : AppConfig -> Session -> Model -> Html Msg
view appConfig session model =
    div [ class "ui middle aligned center aligned grid" ]
        [ div [ class "column" ]
            [ h2 [ class "ui image header" ]
                [ div [ class "content" ]
                    [ text "Log-in to your account      " ]
                ]
            , Html.form [ onSubmit SubmitForm, class "ui large form" ]
                [ div [ class "ui stacked secondary  segment" ]
                    [ viewField Email
                        (div [ class "ui left icon input" ]
                            [ i [ class "user icon" ]
                                []
                            , Text.input
                                (Text.defaultOptions SetEmail)
                                [ name "email", placeholder "E-mail address" ]
                                model.email
                            ]
                        )
                        model.errors
                    , viewField Password
                        (div [ class "ui left icon input" ]
                            [ i [ class "lock icon" ]
                                []
                            , Text.input
                                (Text.defaultOptions SetPassword)
                                [ name "password", placeholder "Password", type_ "password" ]
                                model.password
                            ]
                        )
                        model.errors
                    , div [ class "ui fluid large teal submit button" ]
                        [ text "Login" ]
                    ]
                ]
            ]
        ]


viewField : Field -> Html Msg -> List Error -> Html Msg
viewField field content errors =
    div [ classList [ "field" => True, "error" => (not <| List.isEmpty errors) ] ] <|
        [ content, viewFormErrors field errors ]


viewFormErrors : Field -> List Error -> Html msg
viewFormErrors field =
    div [ class "ui basic red pointing prompt label transition visible" ]
        << List.map
            (\( _, msg ) ->
                text msg
            )
        << List.filter (first >> ((==) field))



-- UPDATE --


type Msg
    = SubmitForm
    | SetEmail String
    | SetPassword String
    | LoginCompleted (Result Http.Error User)


type ExternalMsg
    = NoOp
    | SetUser User


update : AppConfig -> Msg -> Model -> ( ( Model, Cmd Msg ), ExternalMsg )
update appConfig msg model =
    case msg of
        SubmitForm ->
            case validate model of
                [] ->
                    { model | errors = [] }
                        => Http.send LoginCompleted (Query.User.login appConfig model)
                        => NoOp

                errors ->
                    { model | errors = errors }
                        => Cmd.none
                        => NoOp

        SetEmail email ->
            { model | email = email }
                => Cmd.none
                => NoOp

        SetPassword password ->
            { model | password = password }
                => Cmd.none
                => NoOp

        LoginCompleted (Err error) ->
            let
                errorMessages =
                    case error of
                        Http.BadStatus response ->
                            response.body
                                |> decodeString (field "errors" errorsDecoder)
                                |> Result.withDefault []

                        _ ->
                            [ "unable to process registration" ]
            in
                { model | errors = List.map (\errorMessage -> Form => errorMessage) errorMessages }
                    => Cmd.none
                    => NoOp

        LoginCompleted (Ok user) ->
            model
                => Cmd.batch [ storeSession user, Route.modifyUrl Route.Home ]
                => SetUser user



-- VALIDATION --


type Field
    = Form
    | Email
    | Password


{-| Recording validation errors on a per-field basis facilitates displaying
them inline next to the field where the error occurred.

I implemented it this way out of habit, then realized the spec called for
displaying all the errors at the top. I thought about simplifying it, but then
figured it'd be useful to show how I would normally model this data - assuming
the intended UX was to render errors per field.

(The other part of this is having a view function like this:

viewFormErrors : Field -> List Error -> Html msg

...and it filters the list of errors to render only the ones for the given
Field. This way you can call this:

viewFormErrors Email model.errors

...next to the `email` field, and call `viewFormErrors Password model.errors`
next to the `password` field, and so on.

-}
type alias Error =
    ( Field, String )


validate : Model -> List Error
validate =
    Validate.all
        [ .email >> ifBlank (Email => "email can't be blank.")
        , .password >> ifBlank (Password => "password can't be blank.")
        ]


errorsDecoder : Decoder (List String)
errorsDecoder =
    decode (\email username password -> List.concat [ email, username, password ])
        |> optionalError "email"
        |> optionalError "username"
        |> optionalError "password"


optionalError : String -> Decoder (List String -> a) -> Decoder a
optionalError fieldName =
    let
        errorToString errorMessage =
            String.join " " [ fieldName, errorMessage ]
    in
        optional fieldName (Decode.list (Decode.map errorToString string)) []
