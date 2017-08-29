/// Helpers for testing
[<AutoOpen>]
module TestingUtilities

open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.ReadModelStore 


open System
open Expecto

let utcNow() = DateTime.UtcNow

let newEventStoreWithTime time = 
    MemoryDb.EventStore(time) :> IEventStore 

let newEventStore() = 
    newEventStoreWithTime utcNow

let newReadModelStore() = 
    MemoryDb.ReadModelStore() :> IReadModelStore 

let asExpected() =
    Expect.isTrue true "" // no easy way to signal OK

let expectOk result =
    Expect.isOk result ""

let expectError result =
    Expect.isError result ""

let expectOkWithValue (expectation:'a ->unit) result =
    match result with
    | Ok value -> expectation value
    | Error _ -> failwithf "Not expecting %A" result

let expectErrorWithValue (expectation:'a ->unit) result =
    match result with
    | Ok _ -> failwithf "Not expecting %A" result
    | Error value -> expectation value 

let notExpectingError result =
    match result with
    | Ok _ -> ()
    | Error _ -> failwithf "Not expecting %A" result

let notExpectedToGetHere() =
    failwith "Not expected to get here"

