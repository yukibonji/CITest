namespace XLCatlin.DataLab.XCBRA.Command

(*
Command and event-sourcing code for Location aggregates

* Commands and Events are defined in the DomainModel project
* Functions that are specific to this aggregate:
    * applyEvent - given a event and a precommand state, generate a new state
    * executeCommand - given a command and a state, generate a list of domain events
    * toWriteEvent - converts a domain event into a standard WriteEvent that can be serialized
    * fromRecordedEvent - converts a commited RecordedEvent into a domain event

*)

open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.ReadModelStore

module Location =

    open System
    open Savanna.Utilities

    // ==========================
    // Command handling and event application
    // ==========================

    let create id name address maybeGeocode : Location = 
        { id = id
        ; name = name
        ; address = address
        ; description = None
        ; geocode = maybeGeocode
        ; siteCondition = None
        ; plantLayout = None
        ; ownership = None
        ; lastSurveyDate = None
        }

    /// Apply one event to the (optional) state and return the new (optional) state
    let applyEvent : ApplyDomainEvent<LocationEvent,Location> =
        let (<??>) newVal oldVal = 
            match newVal with 
            | Some v -> Some v 
            | _ -> oldVal

        fun stateOpt event ->

            match stateOpt, event with
            | None , LocationEvent.Created (locationId , name, address, maybeGeocode) ->
                Some <|
                    create locationId name address maybeGeocode

            | Some state , LocationEvent.NameUpdated(_ , name) ->
                Some <|
                    { state with name = name }

            | Some state , LocationEvent.DescriptionUpdated(_ , description) ->
                Some <|
                    { state with description = Some description }

            | Some state , LocationEvent.AddressUpdated(_ , address , maybeGeocode) ->
                    Some <|
                        { state with 
                            address = address 
                            geocode = maybeGeocode <??> state.geocode 
                        }    

            | Some state , LocationEvent.SiteConditionUpdated(_ , siteCondition) ->
                Some <|
                    { state with siteCondition = Some siteCondition }

            | Some state , LocationEvent.PlantLayoutUpdated(_ , plantLayout) ->
                Some <|
                    { state with plantLayout = Some plantLayout }

            | Some state , LocationEvent.OwnershipUpdated(_ , ownership) ->
                Some <|
                    { state with ownership = Some ownership }

            | Some state , LocationEvent.LastSurveyDateUpdated(_ , date) ->
                Some <|
                    { state with lastSurveyDate = Some date }

            | _,_ ->
                failwithf 
                    "Unexpected combination of event and state: %A %A" 
                    event 
                    stateOpt 


    let locationIdFromCommand cmd =
        match cmd with
        | LocationCommand.Create (locationId , _ , _ , _) -> 
            locationId

        | LocationCommand.UpdateName(locationId, _ ) ->
            locationId

        | LocationCommand.UpdateDescription(locationId, _ ) ->
            locationId

        | LocationCommand.UpdateAddress(locationId, _ , _ ) ->
            locationId

        | LocationCommand.UpdateSiteCondition(locationId, _ ) ->
            locationId

        | LocationCommand.UpdatePlantLayout(locationId, _ ) ->
            locationId

        | LocationCommand.UpdateOwnership(locationId, _ ) ->
            locationId

        | LocationCommand.UpdateLastSurveyDate(locationId, _ ) ->
            locationId


    
    // A literal so we can use it in pattern matching
    let [<Literal>] EventStreamPrefix = "LocationId:"


    /// Get the stream id for a location
    let idToEventStream (LocationId id) =
        sprintf "%s%s" EventStreamPrefix id |> EventStreamId


    let eventStreamIdFromCommand cmd = 
        let locationId = locationIdFromCommand cmd
        locationId |> idToEventStream 


    /// Execute the command against the (optional) precommand state and 
    /// return the events arising, or Result.Error if the command was invalid.
    let executeCommand cmd : ExecuteCommand<LocationEvent,Location> =
        fun precommandStateOpt -> 
            let locationId = cmd |> locationIdFromCommand
            match precommandStateOpt,cmd with
            | None, LocationCommand.Create(locationId, name, address, maybeGeocode) ->
                Ok <|
                    [ LocationEvent.Created(locationId, name, address, maybeGeocode)
                    ]

            | Some _, LocationCommand.Create _ ->
                Error << CommandExecutionError 
                <| sprintf "Can't create. %A already exists" locationId 

            | None, _ ->
                Error << CommandExecutionError 
                <| sprintf "%A has not been created yet" locationId 

            | _ , LocationCommand.UpdateName(locationId, name ) ->
                Ok <|
                    [ LocationEvent.NameUpdated(locationId, name )
                    ]

            | _ , LocationCommand.UpdateDescription(locationId, description ) ->
                Ok <|
                    [ LocationEvent.DescriptionUpdated(locationId, description)
                    ]

            | _ , LocationCommand.UpdateAddress(locationId, address , geocode ) ->
                Ok <|
                    [ LocationEvent.AddressUpdated(locationId, address , geocode )
                    ]

            | _ , LocationCommand.UpdateSiteCondition(locationId, siteCondition ) ->
                Ok <|
                    [ LocationEvent.SiteConditionUpdated(locationId, siteCondition )
                    ]

            | _ , LocationCommand.UpdatePlantLayout(locationId, plantLayout) ->
                Ok <|
                    [ LocationEvent.PlantLayoutUpdated(locationId, plantLayout)
                    ]

            | _ , LocationCommand.UpdateOwnership(locationId, ownership ) ->
                Ok <|
                    [ LocationEvent.OwnershipUpdated(locationId, ownership )
                    ]

            | _ , LocationCommand.UpdateLastSurveyDate(locationId, date ) ->
                Ok <|
                    [ LocationEvent.LastSurveyDateUpdated(locationId, date )
                    ]



    /// Given the serialized data, create a LocationEvent
    let fromRecordedEvent : FromRecordedEvent<LocationEvent> =
        fun recordedEvent -> 
            //TODO: consider using a format that supports versioning
            Microsoft.FSharpLu.Json.Compact.deserialize<LocationEvent>(recordedEvent.Data)    
            // exception handling is done in the CommandService


    /// Given a LocationEvent, create a WriteEvent 
    let toWriteEvent : ToWriteEvent<LocationEvent> =
        fun domainEvent -> 
            let data = Microsoft.FSharpLu.Json.Compact.serialize(domainEvent)    
            {
            EventId = Guid.NewGuid()
            EventType = "LocationEvent"  // could store info to help the deserializer
            Data = data
            Metadata = ""
            }


    /// Update the entity in the read model (on disk on in cache)
    let updateReadModel (readModelStore:IReadModelStore) (state:Location) = 
        readModelStore.UpdateLocation state


    /// Delete the entity from the read model (on disk on in cache)
    let deleteReadModel (readModelStore:IReadModelStore) (state:Location) = 
        readModelStore.DeleteLocation state.id


    let eventSourcingFunctions cmd readModelStore = {
        FromRecordedEvent = fromRecordedEvent
        ToWriteEvent = toWriteEvent
        ApplyDomainEvent = applyEvent
        ExecuteCommand = executeCommand cmd 
        UpdateReadModel = updateReadModel readModelStore
        DeleteReadModel = deleteReadModel readModelStore
        }


    let replayEventFunctions readModelStore = {
        FromRecordedEvent = fromRecordedEvent
        ApplyDomainEvent = applyEvent
        UpdateReadModel = updateReadModel readModelStore
        }