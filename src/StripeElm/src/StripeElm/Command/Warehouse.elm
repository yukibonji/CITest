module Command.Warehouse exposing (create, delete)

import Http
import HttpBuilder
import Json.Decode as Decode
import Data.AppConfig exposing (AppConfig)
import Data.AuthToken as AuthToken exposing (AuthToken)
import Data.Building exposing (BuildingId)
import Data.Command as Command exposing (ExpectedVersion(..))
import Data.Location exposing (LocationId)
import Data.UUID exposing (UUID)
import Data.Warehouse as Warehouse
import Query.Helpers exposing (apiUrl)


create :
    AppConfig
    -> Maybe AuthToken
    -> Data.Building.BuildingId
    -> Data.Location.LocationId
    -> String
    -> Data.UUID.UUID
    -> Http.Request String
create appConfig maybeToken buildingId locationId name commandId =
    let
        apiCommand =
            { commandId = commandId
            , expectedVersion = Any
            , domainCommand = Command.Warehouse <| Warehouse.Create buildingId locationId name
            }

        body =
            Command.encodeApiCommand apiCommand

        expecting =
            Http.expectString
    in
        HttpBuilder.toRequest <|
            AuthToken.withAuthorization maybeToken <|
                HttpBuilder.withExpect expecting <|
                    HttpBuilder.withJsonBody body <|
                        HttpBuilder.post <|
                            apiUrl appConfig "/command"


delete :
    AppConfig
    -> Maybe AuthToken
    -> Data.Building.BuildingId
    -> Data.UUID.UUID
    -> Http.Request String
delete appConfig maybeToken buildingId commandId =
    let
        apiCommand =
            { commandId = commandId
            , expectedVersion = Any
            , domainCommand = Command.Warehouse <| Warehouse.Delete buildingId
            }

        body =
            Command.encodeApiCommand apiCommand

        expecting =
            Http.expectString
    in
        HttpBuilder.toRequest <|
            AuthToken.withAuthorization maybeToken <|
                HttpBuilder.withExpect expecting <|
                    HttpBuilder.withJsonBody body <|
                        HttpBuilder.post <|
                            apiUrl appConfig "/command"
