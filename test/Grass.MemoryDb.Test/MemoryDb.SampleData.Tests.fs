module MemoryDb.SampleData.Tests

// test that the sample data is built correctly

open System
open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.DomainModel

open Expecto

[<Tests>]   
let testWrite = 
    testList "MemoryDb.SampleData.Tests" [

        testCase "create sample eventStore without errors" <| fun () ->
            let _eventStore = MemoryDb.SampleData.eventStore()
            ()

        testCase "create sample readModelStore without errors" <| fun () ->
            let eventStore = MemoryDb.SampleData.eventStore()
            let _readModelStore = MemoryDb.SampleData.readModelStore(eventStore)
            ()

    ]
