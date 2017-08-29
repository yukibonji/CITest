namespace XLCatlin.DataLab.XCBRA.MemoryDb

open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.CommandStatusStore
open System.Collections.Generic

/// An in-memory implementation of ICommandStatusStore
type CommandStatusStore() =
        
    let statuses = Dictionary<ApiCommandId,ApiCommandStatus>()

    /// object to act as lock monitor. Could use agents instead.
    let lockMonitor = obj()

    /// Convert the result of a TryGet to a Result
    let tryGetToResult tryGet = 
        match tryGet with
        | true,v ->  Ok v
        | false,_ -> CommandStatusStoreError.NotFound |> Error


    interface ICommandStatusStore with

        member __.SetCommandStatus(commandId,status) =
            lock lockMonitor (fun () -> 
                statuses.[commandId] <- status
                )
            Ok ()

        member __.GetCommandStatus(commandId) =
            statuses.TryGetValue(commandId) 
            |> tryGetToResult 

                