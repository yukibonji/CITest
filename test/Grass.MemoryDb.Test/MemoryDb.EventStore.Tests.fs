module MemoryDb.EventStore.Tests

open System
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.DomainModel
open Expecto

// Check that in-memory implementation of the event store works

//TODO: Make this into a check of anything that implements IEventStore
//TODO: add property based tests

// ==========================================
// helpers
// ==========================================

let streamA = EventStreamId "A"
let streamB = EventStreamId "B"

let timestamp1 = DateTime(2001,1,1)
let timestamp2 = DateTime(2002,2,2)

let writeEvent() : WriteEvent = {
    EventId = Guid.NewGuid()
    EventType = "Unknown"
    Data = ""
    Metadata = ""
    }

let expectedAny = ExpectedVersion.Any

let writeEventA1 = {writeEvent() with Data = "A1"}
let writeEventA2 = {writeEvent() with Data = "A2"}
let writeEventB1 = {writeEvent() with Data = "B1"}
let writeEventB2 = {writeEvent() with Data = "B2"}

/// Convert a RecordedEvent back to a WriteEvent
let fromRecordedEvent (recordedEvent:RecordedEvent) :WriteEvent = {
    EventId = recordedEvent.EventId
    EventType = recordedEvent.EventType
    Data = recordedEvent.Data
    Metadata = recordedEvent.Metadata
    }

type StreamError = 
    | ReadError of EventStreamReadError
    | WriteError of EventStreamWriteError

// ==========================================
// Test writing process
// ==========================================

[<Tests>]   
let testWrite = 
    testList "MemoryDb.EventStore.Tests/testWrite" [

        testCase "when stream doesnt exist, expect Ok " <| fun () ->
            let eventStore = newEventStore()
            result {
                let version = expectedAny 
                let events = [writeEventA1]
                let result = eventStore.AppendEventsToStream streamA version events
                result |> expectOk
            } |> notExpectingError


        testCase "when stream exists and correct version, expect Ok" <| fun () ->
            let eventStore = newEventStore()
            result {
                let events1 = [writeEventA1]
                let! result1 = eventStore.AppendEventsToStream streamA expectedAny events1
                let events2 = [writeEventA2]
                let expectedVersion = ExpectedVersion.Expected result1.StreamVersion 
                let result = eventStore.AppendEventsToStream streamA expectedVersion events2
                result |> expectOk
            } |> notExpectingError
            
        testCase "when stream exists and incorrect version, expect Error" <| fun () ->
            let eventStore = newEventStore()
            result {
                let events1 = [writeEventA1]
                let! _ = eventStore.AppendEventsToStream streamA expectedAny events1
                let events2 = [writeEventA2]
                let badVersion = EventStreamVersion 99
                let expectedVersion = ExpectedVersion.Expected badVersion 
                let result = eventStore.AppendEventsToStream streamA expectedVersion events2
                result |> expectErrorWithValue (function
                    | VersionConflict _ -> asExpected()
                    | err -> failwithf "Not expecting %A" err
                    )
            } |> notExpectingError

    ]
    
// ==========================================
// Test reading process
// ==========================================
   
[<Tests>]   
let testRead = 
    testList "EventStore.Tests/testRead" [

        //testCase "when stream doesnt exist, expect StreamNotFound" <| fun () ->
        //    let eventStore = newEventStore()
        //    let result = eventStore.ReadEventsForwards streamA 
        //    result |> expectErrorWithValue (function
        //        | StreamNotFound _ -> asExpected()
        //        | err -> failwithf "Not expecting %A" err
        //        )
            
        testCase "when stream doesnt exist, expect Ok and empty list" <| fun () ->
            let eventStore = newEventStore()
            let result = eventStore.ReadEventsForwards streamA 
            result |> expectOkWithValue (fun data ->
                Expect.equal data.StreamId streamA "streamA"
                Expect.equal data.Events [] "events are empty"
                )
            

        testCase "when stream exists, expect events returned Ok" <| fun () ->
            let time() = timestamp1
            let eventStore = newEventStoreWithTime(time)
            result {
                let writeEvents = [writeEventA1; writeEventA2]
                let! _ = 
                    eventStore.AppendEventsToStream streamA expectedAny writeEvents
                    |> Result.mapError WriteError
                let! readResult = 
                    eventStore.ReadEventsForwards streamA 
                    |> Result.mapError ReadError
                let actualEvents = 
                    readResult.Events |> List.map fromRecordedEvent
                Expect.equal writeEvents actualEvents ""
            } |> notExpectingError

        testCase "when stream exists, expect last version returned Ok" <| fun () ->
            let time() = timestamp1
            let eventStore = newEventStoreWithTime(time)
            result {
                let events = [writeEventA1]
                let! writeResult = 
                    eventStore.AppendEventsToStream streamA expectedAny events 
                    |> Result.mapError WriteError
                let expectedVersion = writeResult.StreamVersion
                let! readResult = 
                    eventStore.ReadEventsForwards streamA 
                    |> Result.mapError ReadError
                Expect.equal expectedVersion readResult.StreamVersion ""
            } |> notExpectingError
            
        testCase "when multiple events, expect oldest first" <| fun () ->
            let time() = timestamp1
            let eventStore = newEventStoreWithTime(time)
            result {
                let events1 = [writeEventA1]
                let! _ = 
                    eventStore.AppendEventsToStream streamA expectedAny events1 
                    |> Result.mapError WriteError
                let events2 = [writeEventA2]
                let! _ = 
                    eventStore.AppendEventsToStream streamA expectedAny events2 
                    |> Result.mapError WriteError

                let! readResult = 
                    eventStore.ReadEventsForwards streamA 
                    |> Result.mapError ReadError

                let expectedEvents = events1 @ events2
                let actualEvents = 
                    readResult.Events |> List.map fromRecordedEvent
                Expect.equal expectedEvents actualEvents ""
            } |> notExpectingError
            
    ]
