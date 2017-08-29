module Command.Location.Tests

open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.Command
open XLCatlin.DataLab.XCBRA.MemoryDb
open Expecto
open Command.TestingUtilities

// =====================
// Tests to check that Location Commands works.
//
// These tests talk directly to the service, bypassing Suave. 
// The tests use mocks and dummy data, so there is no I/O.
//
// For tests that go through Suave, see Command.WebService.Tests in a different project
// =====================

open UnitTestData


// ==========================================
// Test Location
// ==========================================

let defaultId = locationA.id

let defaultCreateLocation = 
    LocationCommand.Create (defaultId,locationA.name,locationA.address,None)

let defaultLocationCreated =
    LocationEvent.Created (defaultId,locationA.name,locationA.address,None)

let updateName newName =
    LocationCommand.UpdateName (defaultId,newName)

let nameUpdated newName =
    LocationEvent.NameUpdated (defaultId,newName)

// Partially apply "fromRecordedEvent"
let expectDomainEvent expectedEvent = 
    expectDomainEvent Command.Location.fromRecordedEvent expectedEvent 


[<Tests>]   
let location = 
    testList "Command.Location.Tests" [
            
        testCase "when CreateLocation on empty stream, expect LocationCreated event" <| fun () ->
            let eventStore = newEventStore() // use same event store throughout test
            let cmdCreate = 
                defaultCreateLocation
                |> DomainCommand.Location 
            let expectedEvent = 
                defaultLocationCreated
            result {
                let! recordedEvents = eventsForCommand eventStore cmdCreate
                recordedEvents |> expectDomainEvent expectedEvent 
            } |> notExpectingError

        testCase "when CreateLocation on non empty stream, expect Error" <| fun () ->
            let eventStore = newEventStore() // use same event store throughout test
            let cmdCreate = 
                defaultCreateLocation
                |> DomainCommand.Location 
            result {
                let! _ = eventsForCommand eventStore cmdCreate
                // do again
                let! _ = eventsForCommand eventStore cmdCreate
                notExpectedToGetHere()
                return () 
            } |> expectErrorWithValue (fun err ->
                match err with
                | Command.DomainCommandError.CommandExecutionError _ -> 
                    asExpected()
                | _ -> 
                    notExpectedToGetHere()
                )

        testCase "when CreateLocation on empty stream, expect Location updated in ReadModel" <| fun () ->
            let cmdCreate = 
                defaultCreateLocation
                |> DomainCommand.Location 
            let expected =
                locationA
            result {
                let! readModelStore = executeCommands [cmdCreate] 
                let! actual = 
                    readModelStore.GetLocationById defaultId 
                    |> Result.mapError DomainCommandError.ReadModelError
                Expect.equal actual expected "location"
            } |> notExpectingError

        testCase "when UpdateName on empty stream, expect Error" <| fun () ->
            let eventStore = newEventStore() // use same event store throughout test
            let cmdUpdateName = 
                updateName "Updated"
                |> DomainCommand.Location 
            let result = eventsForCommand eventStore cmdUpdateName 
            result |> expectErrorWithValue (fun err ->
                match err with
                | Command.DomainCommandError.CommandExecutionError _ -> 
                    asExpected()
                | _ -> 
                    notExpectedToGetHere()
                )

        testCase "when UpdateName on non empty stream, expect PropertyChanged event" <| fun () ->
            let eventStore = newEventStore() // use same event store throughout test
            let cmdCreate = 
                defaultCreateLocation
                |> DomainCommand.Location 
            let cmdUpdateName = 
                updateName "Updated"
                |> DomainCommand.Location 
            let expectedEvent = 
                nameUpdated "Updated"
            result {
                let! _ = eventsForCommand eventStore cmdCreate
                // update
                let! recordedEvents = eventsForCommand eventStore cmdUpdateName
                recordedEvents |> expectDomainEvent expectedEvent 
            } |> notExpectingError

        testCase "when ChangeProperty on non empty stream, expect Location updated in ReadModel" <| fun () ->
            let cmdCreate = 
                defaultCreateLocation
                |> DomainCommand.Location 
            let cmdUpdateName = 
                updateName "Updated"
                |> DomainCommand.Location 
            let expected =
                {locationA with name = "Updated"}
            result {
                let! readModelStore = executeCommands [cmdCreate;cmdUpdateName] 
                let! actual = 
                    readModelStore.GetLocationById defaultId
                    |> Result.mapError DomainCommandError.ReadModelError
                Expect.equal actual expected "location"
            } |> notExpectingError

    // END testlist
    ]

