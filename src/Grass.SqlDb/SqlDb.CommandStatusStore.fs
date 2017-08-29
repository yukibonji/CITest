module XLCatlin.DataLab.XCBRA.SqlDb.CommandStatusStore

open System
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.CommandStatusStore

open FSharp.Data
open FSharp.Data.SqlClient
open System.Data
open System.Data.SqlClient

// ================================================
// Type Providers
// ================================================

/// Get CommandStatus for given Id
type GetCommandStatus = SqlCommandProvider<"""
    SELECT *
    FROM dbo.CommandStatus
    WHERE CommandId = @CommandId
    """ , "name=Dazzle">

/// Create a CommandStatus of pending
type InsertCommandStatus = SqlCommandProvider<"""
    INSERT dbo.CommandStatus (
        CommandId
        , Status
        , Data
        , Started
        ) 
    VALUES (
        @CommandId
        , @Status
        , @Data
        , @Started
        )
    """ , "name=Dazzle">

/// Completed a CommandStatus of Success/Failure
type UpdateCommandStatus = SqlCommandProvider<"""
    UPDATE dbo.CommandStatus
    SET 
        Status = @Status
        , Data = @Data 
        , Completed = @Completed 
    WHERE CommandId = @CommandId
    """ , "name=Dazzle">


// ================================================
// Query
// ================================================

let toCommandStatus (row:GetCommandStatus.Record) =
    match row.Status with
    | "Pending" -> 
        ApiCommandStatus.Pending
    | "Success" -> 
        Success
    | "Failure" -> 
        let msg = row.Data
        Failure msg 
    | _ ->
        failwithf "Unexpected status %s. CommandId=%A" row.Status row.CommandId

/// Query used by ICommandStatusStore
let getCommandStatus (connection:SqlConnection) commandId =
    use cmd = new GetCommandStatus(connection) 
    cmd.Execute(commandId)  
    |> Seq.map toCommandStatus
    |> Seq.toList
    |> function
        | [row] -> 
            Ok row
        | [] -> 
            CommandStatusStoreError.NotFound 
            |> Error
        | _ -> 
            let msg = sprintf "More than one row returned for commandId=%A" commandId
            CommandStatusStoreError.OtherError msg
            |> Error 

// ================================================
// Update
// ================================================

/// Create a string from a Status 
let fromStatus status = 
    match status with
    | Pending -> "Pending" 
    | Success -> "Success"
    | Failure _ -> "Failure"

/// Insert one record
let setCommandStatus (connection:SqlConnection) utcNow commandId status =
    match status with
    | ApiCommandStatus.Pending ->
        use cmd = new InsertCommandStatus(connection) 
        cmd.Execute(
            CommandId = commandId,
            Status = (status |> fromStatus),
            Data = null,
            Started = utcNow()
            )  
        |> ignore
    | ApiCommandStatus.Success ->
        use cmd = new UpdateCommandStatus(connection) 
        cmd.Execute(
            CommandId = commandId,
            Status = (status |> fromStatus),
            Data = null,
            Completed = utcNow()
            )  
        |> ignore
    | ApiCommandStatus.Failure data ->
        use cmd = new UpdateCommandStatus(connection) 
        cmd.Execute(
            CommandId = commandId,
            Status = (status |> fromStatus),
            Data = data,
            Completed = utcNow()
            )  
        |> ignore



// ================================================
// ICommandStatusStore implementation
// ================================================

/// A SQL implementation of an event store
type CommandStatusStore(connection:SqlConnection,utcNow: unit->DateTime) =
        
    interface ICommandStatusStore with

        member __.GetCommandStatus commandId =
            try
                getCommandStatus connection commandId 
            with
            | ex ->
                ex.Message
                |> CommandStatusStoreError.OtherError
                |> Error
           
        member __.SetCommandStatus(commandId,status) =
            try
                setCommandStatus connection utcNow commandId status
                |> Ok
            with
            | ex ->
                ex.Message
                |> CommandStatusStoreError.OtherError
                |> Error
