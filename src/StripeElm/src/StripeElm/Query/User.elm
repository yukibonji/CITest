module Query.User exposing (login, storeSession)

import Http
import Json.Encode as Encode
import Json.Decode as Decode
import Ports
import Helpers exposing ((=>))
import Data.User as User exposing (User)
import Query.Helpers exposing (apiUrl)


storeSession : User -> Cmd msg
storeSession =
    Ports.storeSession
        << Just
        << Encode.encode 0
        << User.encode


login :
    { r
        | apiHost : String
        , apiPort : Int
        , apiProtocol : String
        , apiVersion : Int
    }
    -> { a | email : String, password : String }
    -> Http.Request User
login appConfig { email, password } =
    let
        user =
            Encode.object
                [ "email" => Encode.string email
                , "password" => Encode.string password
                ]

        body =
            Http.jsonBody <|
                Encode.object [ "user" => user ]
    in
        Http.post (apiUrl appConfig "/user/login") body <|
            Decode.field "user" User.decoder



--
-- edit :
--     ApiConfig r
--     -> { a
--         | bio : String
--         , email : String
--         , password : Maybe String
--         , username : String
--        }
--     -> Maybe AuthToken
--     -> Http.Request User
-- edit appConfig { username, email, bio, password } maybeToken =
--     let
--         updates =
--             [ Just ("username" => Encode.string username)
--             , Just ("email" => Encode.string email)
--             , Just ("bio" => Encode.string bio)
--             , Maybe.map (\pass -> "password" => Encode.string pass) password
--             ]
--                 |> List.filterMap identity
--
--         body =
--             ("user" => Encode.object updates)
--                 |> List.singleton
--                 |> Encode.object
--                 |> Http.jsonBody
--
--         expect =
--             User.decoder
--                 |> Decode.field "user"
--                 |> Http.expectJson
--     in
--         apiUrl appConfig "/user"
--             |> HttpBuilder.put
--             |> HttpBuilder.withExpect expect
--             |> HttpBuilder.withBody body
--             |> withAuthorization maybeToken
--             |> HttpBuilder.toRequest
