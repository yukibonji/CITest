namespace XLCatlin.DataLab.XCBRA.Command

/// Main entry point for handling domain commands.
/// ICommandService from DomainModel is implemented at the bottom of the file.

open XLCatlin.DataLab.XCBRA

open Savanna
open Savanna.Utilities
open Savanna.Domain
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.ReadModelStore
open XLCatlin.DataLab.XCBRA.CommandStatusStore
open XLCatlin.DataLab.XCBRA.DomainModel


type DeserializationError = string * exn

type DomainCommandError = 
    | ReadError of EventStreamReadError
    | ReadModelError of ReadModelError
    | WriteError of EventStreamWriteError
    | DeserializationError of DeserializationError 
    | CommandExecutionError of CommandExecutionError 
    | CommandStatusError of string


module private CommandHandlerImplementation =

    let deserializeRecordedEvent (fromRecordedEvent:FromRecordedEvent<_>) event = 
        try
            fromRecordedEvent event |> Ok
        with
        | ex -> 
            DeserializationError (event.Data, ex) |> Error

    /// Apply an event to a state. Return Result<newState,error>
    /// First, deserialize the RecordedEvent to a domain event, then apply it. 
    let applyRecordedEvent fromRecordedEvent (applyDomainEvent:ApplyDomainEvent<'DomainEvent,'State>) state recordedEvent =
        result {
            let! domainEvent =  deserializeRecordedEvent fromRecordedEvent recordedEvent
            let newState = applyDomainEvent state domainEvent 
            return newState
        }
        

    /// Apply a list of events starting at the initial state. 
    /// Return Result<newState,error>
    let applyEvents fromRecordedEvent applyDomainEvent initialState recordedEvents =

        // partially apply `applyRecordedEvent` to bake in dependencies
        let apply = applyRecordedEvent fromRecordedEvent applyDomainEvent 
        // ... and lift to Result -> Result 
        let applyR stateR event =  
            stateR |> Result.bind (fun state -> apply state event)
        
        // lift initial state to Result
        let initialStateR = Ok initialState 
        
        // fold over the events
        recordedEvents 
        |> List.fold applyR initialStateR 

    /// Using the historical events, rebuild the current state (None if there are no events yet).
    /// return the stream version and the current state
    let restoreStateFromHistory fromRecordedEvent applyDomainEvent (eventStore:IEventStore) eventStreamId =
        result {
            // Read all the historical events
            // An errors are lifted to DomainCommandError
            let! readResult = 
                eventStore.ReadEventsForwards(eventStreamId)
                |> Result.mapError ReadError

            // replay the events
            let initialState = None
            let! preCommandState = applyEvents fromRecordedEvent applyDomainEvent initialState readResult.Events 
            
            // return the stream version and the preCommandState 
            return readResult.StreamVersion, preCommandState 
            }

    /// Process a command -- save the new events to the event store
    /// and return the WriteResult object.
    let processCommand (eventStore:IEventStore) eventStreamId (functions:EventSourcingFunctions<'DomainEvent,'State>) =
        // these are the custom functions needed for a specific domain
        let fromRecordedEvent = functions.FromRecordedEvent
        let toWriteEvent = functions.ToWriteEvent
        let applyDomainEvent = functions.ApplyDomainEvent
        let executeCommand = functions.ExecuteCommand
        let updateReadModel = functions.UpdateReadModel
        let deleteReadModel = functions.DeleteReadModel
         
        // common logic for all commands
        result {
            // Get preCommand state by reading the historical events
            // from the IEventStore (a database of some sort)
            // If there are no events, the currentState is None
            let! streamVersion,preCommandStateOpt = 
                restoreStateFromHistory fromRecordedEvent applyDomainEvent eventStore eventStreamId 
            
            // Execute the command in the context of the preCommand state.
            // This is a pure function that just generates events, but doesn't apply them to the state.
            // If the command cannot be executed (eg. create a location that already exists)
            // then a Result.Error is returned (which must be lifted from CommandExecutionError to DomainCommandError)
            let! newDomainEvents = 
                executeCommand preCommandStateOpt 
                |> Result.mapError CommandExecutionError

            // Then apply the new events to get the latest state.
            // Since the validity of the events is ensured (by the execution above)
            // this is guaranteed to work.
            // The new state is also an option, (e.g A deletion event may set it back to None)
            let postCommandStateOpt = 
                newDomainEvents |> List.fold applyDomainEvent preCommandStateOpt
            
            // Update the read model 
            do! match preCommandStateOpt,postCommandStateOpt with
                | _,Some postCommandState-> 
                    // the postCommandState exists
                    updateReadModel postCommandState
                    |> Result.mapError ReadModelError 
                | Some preCommandState, None->
                    // the state existed before and doesn't exist now
                    // so remove from the read model
                    deleteReadModel preCommandState
                    |> Result.mapError ReadModelError 
                | None,None -> 
                    // the state didnt exist before and doesn't exist now
                    Ok ()

            // Commit the new events to the eventStore
            // Note: if the IEventStore and IReadModelStore are SQL backed, 
            // this should be done in the same transaction.
            let! writeResult = 
                let writableEvents = newDomainEvents |> List.map toWriteEvent
                let expectedVersion = Expected streamVersion
                eventStore.AppendEventsToStream eventStreamId expectedVersion writableEvents 
                |> Result.mapError WriteError

            // return the new stream version, etc
            return writeResult 
        }

    /// Process all events on the stream and update the read model
    let processReplayEventStream (eventStore:IEventStore) eventStreamId (functions:ReplayEventFunctions<'DomainEvent,'State>) =
        // these are the custom functions needed for a specific domain
        let fromRecordedEvent = functions.FromRecordedEvent
        let applyDomainEvent = functions.ApplyDomainEvent
        let updateReadModel = functions.UpdateReadModel

        // common logic for all commands
        result {
            // get current state
            let! version,currentState = 
                restoreStateFromHistory fromRecordedEvent applyDomainEvent eventStore eventStreamId 
            // update the read model
            do! match currentState with
                | None -> 
                    Ok ()
                | Some newState ->
                    updateReadModel newState |> Result.mapError ReadModelError 
            // return the stream version
            return version 
        }

// ==========================================
// Public interface    
// ==========================================    

module DomainCommandHandler =

    /// Process a command and return the WriteResult
    let executeCommand (eventStore:IEventStore) (readModelStore:IReadModelStore) (cmd:DomainCommand) =
        match cmd with
        | DomainCommand.Location cmd ->
            let eventStreamId = Location.eventStreamIdFromCommand cmd
            let functions = Location.eventSourcingFunctions cmd readModelStore
            CommandHandlerImplementation.processCommand eventStore eventStreamId functions  
        | DomainCommand.Warehouse cmd ->
            let eventStreamId = Warehouse.eventStreamIdFromCommand cmd
            let functions = Warehouse.eventSourcingFunctions cmd readModelStore
            CommandHandlerImplementation.processCommand eventStore eventStreamId functions  

    /// Replay all the events for a stream and update the read model
    let replayAllEventsForStream (eventStore:IEventStore) (readModelStore:IReadModelStore) eventStreamId =

        (*
        // Helper function to do Prefix matching.
        let streamIdTxt = "BuildingId:2" 
        match streamIdTxt with
        | Prefix "Location" rest -> printfn "found Location %s" rest
        | Prefix "Building" rest -> printfn "found Building %s" rest
        *)
        let (|Prefix|_|) prefix (s:string) = 
            if s.StartsWith(prefix) then Some (s.Substring(prefix.Length)) else None

        let (EventStreamId streamIdTxt) = eventStreamId
        match streamIdTxt with
        | Prefix Location.EventStreamPrefix _ ->
            let functions = Location.replayEventFunctions readModelStore
            CommandHandlerImplementation.processReplayEventStream eventStore eventStreamId functions  
        | Prefix Warehouse.EventStreamPrefix _ ->
            let functions = Warehouse.replayEventFunctions readModelStore
            CommandHandlerImplementation.processReplayEventStream eventStore eventStreamId functions  
        | _ ->
            ConsoleLogger.Default.Error("EventStream prefix not recognized", "StreamId", streamIdTxt)
            // ignore -- act as if the steam is not found
            Ok (EventStreamVersion.First)


    /// Replay all events to rebuild the read model from scratch
    let replayEventsForAllStreams (eventStore:IEventStore) (readModelStore:IReadModelStore) =
        result {
            let! eventStreamIds = 
                eventStore.AllEventStreamIds()
                |> Result.mapError ReadError
            let versions = 
                eventStreamIds 
                |> List.map (replayAllEventsForStream eventStore readModelStore)
                |> Result.sequence
            versions |> ignore                
            }

/// Implementation of IApiCommandService from DomainModel project
type ApiCommandService (eventStore:IEventStore, readModelStore:IReadModelStore,commandStatusStore:ICommandStatusStore) =
    
    /// Convert a DomainCommandError into a ApiCommandError
    let toApiCommandError (e:DomainCommandError) =
        let msg = sprintf "%A" e   // convert to string for now
        ApiCommandError.ExecutionError msg 

    let toApiCommandResult result = 
        result 
        |> Result.map (ignore)  // map the success to unit
        |> Result.mapError toApiCommandError // map the error to different type


    let setCommandStatus cmdId status =
        commandStatusStore.SetCommandStatus(cmdId,status)
        // lift to common DomainCommandError
        |> Result.mapError (fun s -> s |> sprintf "%A" |> CommandStatusError)

    let setCommandStatusPending cmdId =
        setCommandStatus cmdId ApiCommandStatus.Pending

    let setCommandStatusSuccess cmdId =
        setCommandStatus cmdId ApiCommandStatus.Success

    let onErrorSetCommandStatusFailure cmdId result =
        result |> Result.mapError (fun err ->
            let errString = sprintf "%A" err 
            setCommandStatus cmdId (ApiCommandStatus.Failure errString) 
            // |> //TODO add error to log
            |> ignore // if we can't set the status, log and ignore rather than changing the existing error
            
            // return the original error
            err
            )

    interface IApiCommandExecutionService with
        member this.Execute(cmd) = 
            let cmdId = cmd.commandId
            result {
                do! setCommandStatusPending cmdId 
                let! success = DomainCommandHandler.executeCommand eventStore readModelStore cmd.domainCommand
                do! setCommandStatusSuccess cmdId 
                return success 
            }
            |> onErrorSetCommandStatusFailure cmdId 
            |> toApiCommandResult 
            |> async.Return  //TODO Async not implemented yet
