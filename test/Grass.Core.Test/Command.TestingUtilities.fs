/// Helper functions for testing Command
module Command.TestingUtilities

open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.Command
open Expecto

let expectDomainEvent toDomainEvent expectedEvent (recordedEvents:RecordedEvent list) = 
    let domainEvents = recordedEvents |> List.map toDomainEvent
    match domainEvents with
    | [] -> 
        failwith "No events found."
    | events -> 
        let label = (sprintf "Event found like %A" expectedEvent)
        Expect.contains events expectedEvent label 


/// Handle the command and return the list of RecordedEvents (or Error)
let eventsForCommand eventStore cmd =  
    result {
    let readModelStore = newReadModelStore()
    let! writeResult = Command.DomainCommandHandler.executeCommand eventStore readModelStore cmd
    return writeResult.RecordedEvents 
    }


/// Process a list of commands and return the ReadModel or error
let executeCommands cmds =  
    let eventStore = newEventStore() 
    let readModelStore = newReadModelStore()
    cmds
    |> List.map (Command.DomainCommandHandler.executeCommand eventStore readModelStore)
    |> Result.sequence
    |> Result.map (fun _ -> readModelStore)

