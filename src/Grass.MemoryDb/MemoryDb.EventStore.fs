namespace XLCatlin.DataLab.XCBRA.MemoryDb

open System
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.EventStore

/// Define EventStream as a list of events plus a version
type EventStream = {
    Id : EventStreamId
    Version : EventStreamVersion
    Events : RecordedEvent list // most recent events first
    }

module private Implementation = 

    let toRecordedEvent streamId utcNow eventNumber (writeEvent:WriteEvent) : RecordedEvent =
        {
        StreamId = streamId
        EventId = writeEvent.EventId
        EventNumber = eventNumber 
        EventType = writeEvent.EventType
        Data = writeEvent.Data
        Metadata = writeEvent.Metadata
        Created = utcNow()
        } 

    let createNewStream utcNow streamId (writeEvents:WriteEvent list) = 
        let recordedEvents = 
            writeEvents 
            |> List.rev
            |> List.mapi (fun i ev ->
                toRecordedEvent streamId utcNow i ev
                )
        let newStream = {
            Id  = streamId
            Version = EventStreamVersion 1
            Events = recordedEvents
            }
        let writeResult : EventStreamWriteResult = {
            StreamId = newStream.Id
            StreamVersion = newStream.Version 
            Timestamp = DateTime.UtcNow
            RecordedEvents = recordedEvents
            }
        (newStream,writeResult)

    let incrementVersion (EventStreamVersion version) = 
        EventStreamVersion (version + 1)

    let getEventsForwards eventStream = 
        eventStream.Events |> List.rev // return with oldest event first

    let appendEvents utcNow expectedVersion (events:WriteEvent list) eventStream = 
        match expectedVersion with
        | Expected expected when eventStream.Version <> expected ->
            let conflict = {
                StreamId =  eventStream.Id
                ExpectedStreamVersion = eventStream.Version 
                }
            Error (VersionConflict conflict)
        | Expected _ // when eventStream.Version = version 
        | Any -> 
            // return a new stream with the events prepended, newest first
            let streamId = eventStream.Id
            let startNumber = eventStream.Events.Length
            let recordedEvents = 
                events 
                |> List.rev
                |> List.mapi (fun i ev ->
                    toRecordedEvent streamId utcNow (startNumber+i) ev
                    )
            let newStream = {
                eventStream with
                    Version = incrementVersion eventStream.Version 
                    Events = recordedEvents @ eventStream.Events
                }
            let writeResult: EventStreamWriteResult = {
                StreamId = newStream.Id
                StreamVersion = newStream.Version 
                Timestamp = DateTime.UtcNow
                RecordedEvents = recordedEvents
                }
            (newStream,writeResult) |> Ok


/// An in-memory implementation of an event store
type EventStore(utcNow: unit->DateTime) =
        
    /// Each stream is keyed by the EventStreamId. 
    let streams = System.Collections.Generic.Dictionary<EventStreamId,EventStream>()

    /// object to act as lock monitor
    let lockMonitor = obj()

    let withStreamDo streamId mapInfrastructureError handleExistingStream handleMissingStream =
        // lock the dictionary during a transaction. Could be replaced by an agent,
        // but this is just for testing the PoC
        lock lockMonitor (fun () -> 
        try                
            match streams.TryGetValue(streamId) with
            | true,stream ->
                handleExistingStream stream 
            | false, _ ->
                handleMissingStream() 
        with
        | ex -> 
            (ex.Message, ex) 
            |> EventStreamInfrastructureError.ExceptionError
            |> mapInfrastructureError 
            |> Error
        )


    let updateStream eventStream =
        streams.[eventStream.Id] <- eventStream

    interface IEventStore with

        member __.AppendEventsToStream streamId expectedVersion (events:WriteEvent list) =
            let mapInfrastructureError = EventStreamWriteError.InfrastructureError
            let handleExistingStream stream = 
                stream 
                |> Implementation.appendEvents utcNow expectedVersion events
                |> Result.map (fun (stream,commit) ->
                    updateStream stream
                    commit
                    )
            let handleMissingStream()  = 
                let stream,commit = Implementation.createNewStream utcNow streamId events
                updateStream stream 
                commit |> Ok 
            withStreamDo streamId mapInfrastructureError handleExistingStream handleMissingStream
           
        member __.ReadEventsForwards streamId =
            let mapInfrastructureError = EventStreamReadError.InfrastructureError
            let handleExistingStream stream = 
                let recordedEvents = stream |> Implementation.getEventsForwards
                let readResult = {
                    StreamId = streamId
                    StreamVersion = stream.Version
                    Events = recordedEvents
                    }
                readResult |> Ok
            let handleMissingStream() = 
            //    let msg = sprintf "%A" streamId
            //    StreamNotFound msg 
            //    |> Error 
                let readResult = {
                    StreamId = streamId
                    StreamVersion = EventStreamVersion 0 
                    Events = []
                    }
                readResult |> Ok

            withStreamDo streamId mapInfrastructureError handleExistingStream handleMissingStream
            
        /// List all event streams. 
        /// Used when rebuilding the read model from scratch.
        member this.AllEventStreamIds() =
            streams.Keys |> Seq.toList |> Ok


