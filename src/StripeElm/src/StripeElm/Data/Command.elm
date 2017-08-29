module Data.Command exposing (..)

import Json.Encode as Encode exposing (Value)
import Data.UUID exposing (UUID)
import Data.Location as Location
import Data.Warehouse as Warehouse
import Helpers exposing ((=>))


type EventStreamId
    = EventStreamId String


{-| A stream has a monotonically increasing version which is changed
    every time the stream is modified.

    When saving an event, an expected stream version must be provided,
    and if the version does match current version of the stream,
    a VersionConflict error is returned.
-}
type EventStreamVersion
    = EventStreamVersion Int


first : EventStreamVersion
first =
    EventStreamVersion 0


{-| Passed in to Write operation for concurrency; see
    http://docs.geteventstore.com/dotnet-api/3.0.1/optimistic-concurrency-and-idempotence/
-}
type ExpectedVersion
    = Expected EventStreamVersion
    | Any


encodeExpectedVersion : ExpectedVersion -> Value
encodeExpectedVersion expectedVersion =
    case expectedVersion of
        Any ->
            Encode.string "Any"

        Expected (EventStreamVersion v) ->
            Encode.object
                [ "Expected"
                    => Encode.object
                        [ "EventStreamVersion" => Encode.int v ]
                ]



--  Commands -------------------------------------------------------------------


{-| A union of all possible commands in the domain
-}
type DomainCommand
    = Location Location.ApiCommand
    | Warehouse Warehouse.ApiCommand


encodeDomainCommand : DomainCommand -> Value
encodeDomainCommand domainCommand =
    Encode.object <|
        case domainCommand of
            Location command ->
                [ "Location" => Location.encodeApiCommand command ]

            Warehouse command ->
                [ "Warehouse" => Warehouse.encodeApiCommand command ]


{-| Everything needed for a API command
    Authentication/Authorization is performed out-of-band.
-}
type alias ApiCommand =
    { commandId : UUID
    , expectedVersion : ExpectedVersion
    , domainCommand : DomainCommand
    }


encodeApiCommand : ApiCommand -> Value
encodeApiCommand { commandId, expectedVersion, domainCommand } =
    Encode.object
        [ "commandId" => Encode.string commandId
        , "expectedVersion" => encodeExpectedVersion expectedVersion
        , "domainCommand" => encodeDomainCommand domainCommand
        ]



-- Errors ----------------------------------------------------------------------


type alias ApiValidationError =
    { field : String
    , error : String
    }


{-| Errors associated with a bad ApiCommand execution

    - 400: The data in the command is invalid, return a list of errors
    - 400: The command cannot be executed correctly - e.g. deleting an non-existent object
    - 401: Credentials are bad
    - 500: A server side error
-}
type ApiCommandError
    = ValidationError String
    | ExecutionError String
    | AuthenticationError String
    | ServerError String



-- CommandExecution service ----------------------------------------------------


{-| The result of a ApiCommand execution
-}
type alias ApiCommandExecutionResult =
    Result () ApiCommandError



-- CommandStatus service -------------------------------------------------------


type ApiCommandStatus
    = Success
    | Pending
    | Failure String


{-| The result of a status check
-}
type ApiCommandStatusResult
    = Result ApiCommandStatus ApiCommandError
