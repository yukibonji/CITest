module XLCatlin.DataLab.XCBRA.SqlDb.EventStore

open System
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.EventStore

open FSharp.Data
open FSharp.Data.SqlClient
open System.Data
open System.Data.SqlClient

// ================================================
// Type Providers
// ================================================

/// Get all stream ids (used for replaying events)
type AllEventStreamIds = SqlCommandProvider<"""
    SELECT DISTINCT StreamId  
    FROM dbo.RecordedEvent
    """ , "name=Dazzle">

/// Get the number of events (=stream version)
type GetEventCount = SqlCommandProvider<"""
    SELECT COUNT(*) as Count
    FROM dbo.RecordedEvent
    WHERE StreamId = @StreamId
    """ , "name=Dazzle",SingleRow=true>

/// Get all events for a particular stream
type GetEventsForwards = SqlCommandProvider<"""
    SELECT *
    FROM dbo.RecordedEvent
    WHERE StreamId = @StreamId
    ORDER BY EventNumber
    """ , "name=Dazzle">

/// Udate events for a particular stream
type AppendEventToStream = SqlCommandProvider<"""
    INSERT INTO dbo.RecordedEvent (
        StreamId 
        ,EventId 
        ,EventNumber 
        ,EventType 
        ,Data 
        ,Metadata 
        ,Created 
        )
    VALUES(
        @StreamId 
        ,@EventId 
        ,@EventNumber 
        ,@EventType
        ,@Data 
        ,@Metadata 
        ,@Created 
        )
    """ , "name=Dazzle">

type DazzleDb = SqlProgrammabilityProvider<"name=Dazzle">


// ================================================
// Query
// ================================================

/// Query used by IEventStore
let getStreamVersion (connection:SqlConnection) (EventStreamId eventStreamId) =
    use cmd = new GetEventCount(connection) 
    let result = cmd.Execute(eventStreamId)  
    match result with
        | Some (Some count) -> count
        | Some None
        | None -> 0
    |> EventStreamVersion

/// Query used by IEventStore
let allEventStreamIds (connection:SqlConnection) =
    use cmd = new AllEventStreamIds(connection) 
    cmd.Execute()  
    |> Seq.map EventStreamId
    |> Seq.toList


let rowToRecordedEvent (row:GetEventsForwards.Record) : RecordedEvent =
    {
    StreamId = row.StreamId |> EventStreamId
    EventId = row.EventId
    EventNumber = row.EventNumber 
    EventType = row.EventType
    Data = row.Data
    Metadata = row.Metadata
    Created = row.Created
    } 


/// Query used by IEventStore
let readEventsForwards (connection:SqlConnection) eventStreamId =
    let (EventStreamId id) = eventStreamId 
    use cmd = new GetEventsForwards(connection) 
    let events =
        cmd.Execute(id)  
        |> Seq.map rowToRecordedEvent 
        |> Seq.toList

    {
    EventStreamReadResult.StreamId = eventStreamId
    StreamVersion = EventStreamVersion.First
    Events = events 
    }

// ================================================
// Update
// ================================================

/// Increment the stream version by the number of events recorded.
/// (the stream version is just the number of events)
let incrementStreamVersion (EventStreamVersion version) eventCount = 
    EventStreamVersion (version + eventCount)

/// Insert one record and return the recorded event
let appendEventToStream (connection:SqlConnection) timestamp eventStreamId eventNumber (event:WriteEvent) =
    use cmd = new AppendEventToStream(connection) 
    let (EventStreamId streamId) = eventStreamId 
    cmd.Execute(
        StreamId = streamId,
        EventId = event.EventId,
        EventType = event.EventType,
        EventNumber = eventNumber,
        Data = event.Data,
        Metadata = event.Metadata, 
        Created = timestamp)  
    |> ignore

    // create a recorded event
    {
    StreamId = eventStreamId 
    EventId = event.EventId
    EventNumber = eventNumber 
    EventType = event.EventType
    Data = event.Data
    Metadata = event.Metadata
    Created = timestamp
    } 

/// Insert a list of events and return
/// Ok Error VersionConflict or 
let appendEventsToStream (connection:SqlConnection) utcNow expectedVersion eventStreamId (events:WriteEvent list) = 
    let tran = connection.BeginTransaction()
    try
        let timestamp = utcNow() 
        let actualStreamVersion = getStreamVersion connection eventStreamId
        match expectedVersion with
        | Expected expected when actualStreamVersion <> expected ->
            tran.Rollback()
            let conflict = {
                StreamId =  eventStreamId
                ExpectedStreamVersion = actualStreamVersion  
                }
            Error (VersionConflict conflict)
        | Expected _ // when actualStreamVersion  = expectedVersion 
        | Any -> 
            let recordedEvents = 
                let append = appendEventToStream connection timestamp eventStreamId 
                events
                |> List.mapi append 
            tran.Commit()

            let newStreamVersion = incrementStreamVersion actualStreamVersion events.Length 
            let writeResult : EventStreamWriteResult =
                {
                StreamId = eventStreamId 
                StreamVersion = newStreamVersion 
                Timestamp = timestamp
                RecordedEvents = recordedEvents 
                }
            writeResult |> Ok
    with
    | ex ->
        tran.Rollback()
        ExceptionError (ex.Message,ex)
        |> EventStreamWriteError.InfrastructureError
        |> Error 

// ================================================
// IEventStore implementation
// ================================================

/// A SQL implementation of an event store
type EventStore(connection:SqlConnection,utcNow: unit->DateTime) =
        
    interface IEventStore with

        member __.AppendEventsToStream streamId expectedVersion (events:WriteEvent list) =
            appendEventsToStream connection utcNow expectedVersion streamId events
           
        member __.ReadEventsForwards streamId =
            readEventsForwards connection streamId 
            |> Ok
            
        /// List all event streams. 
        /// Used when rebuilding the read model from scratch.
        member this.AllEventStreamIds() =
            allEventStreamIds(connection) |> Ok


