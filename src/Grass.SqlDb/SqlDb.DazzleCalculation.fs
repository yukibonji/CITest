module XLCatlin.DataLab.XCBRA.SqlDb.DazzleCalculation

open System
open Utilities
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.Dazzle
open System.Data
open System.Data.SqlClient
open System.Collections.Generic  // for Dictionary
open FSharp.Data
open FSharp.Data.SqlClient

// ================================================
// Type Providers
// ================================================

// ResultType.DataReader is used because we will pipe this into a table using "Load"
type GetDazzleCalculationById = SqlCommandProvider<"""
    SELECT * 
    FROM dbo.DazzleCalculation
    WHERE DazzleCalculationId = @DazzleCalculationId 
    """ , "name=Dazzle",ResultType.DataReader>

type DeleteDazzleCalculationById = SqlCommandProvider<"""
    DELETE
    FROM dbo.DazzleCalculation
    WHERE DazzleCalculationId = @DazzleCalculationId 
    """ , "name=Dazzle">

type DazzleDb = SqlProgrammabilityProvider<"name=Dazzle">

// ================================================
// Query
// ================================================

/// Create a DazzleCalculation from a DazzleCalculation.Row 
let rowToDazzleCalculation (row:DazzleDb.dbo.Tables.DazzleCalculation.Row) : DazzleCalculation =
    let duration = row.DurationMs * 1<_>
    let status = 
        match row.Status with
        | "InProcess" -> 
            InProcess duration
        | "Completed" -> 
            Completed (duration,row.Output)
        | _ -> 
            failwithf "DazzleCalculation.Row.Status not recognized:'%s'" row.Status 
    {
    Id = DazzleCalculationId row.DazzleCalculationId
    Input = row.Input
    Status = status
    }


// ================================================
// Update
// ================================================

let updateRowStatus isStart (row:DazzleDb.dbo.Tables.DazzleCalculation.Row) status =
    match status with
    | InProcess remaining ->
        row.Status <- "InProcess"
        row.DurationMs <- remaining * 1<_>
        if isStart then
            row.Started <- DateTime.UtcNow
    | Completed (duration,output) ->
        row.Status <- "Completed"
        row.DurationMs <- duration * 1<_>
        row.Output <- output
        row.Completed <- DateTime.UtcNow |> Some

let updateRowStatusOnStart row status = 
    updateRowStatus true row status 
let updateRowStatusOnStatusChange row status =
    updateRowStatus false row status 

/// Update used by IReadModel        
let createDazzleCalculation (connection:SqlConnection) (value:DazzleCalculation) =

    // get the existing row (if any)
    use dazzleTable = new DazzleDb.dbo.Tables.DazzleCalculation() 
    use cmd = new GetDazzleCalculationById(connection) 
    cmd.Execute(value.Id.Value)  
    |> dazzleTable.Load 

    // update the table
    match dazzleTable.Rows |> Seq.toList with
    | [] ->
        let row = dazzleTable.NewRow() 
        row.DazzleCalculationId <- value.Id.Value
        row.Input <- value.Input
        updateRowStatusOnStart row value.Status
    | _ ->
        failwithf "DazzleCalculation already exists: %s" value.Id.Value

    // save to DB
    dazzleTable.AcceptChanges()

    
    // save to DB
    dazzleTable.AcceptChanges()

/// Update used by IReadModel        
let updateDazzleCalculationStatus (connection:SqlConnection) (DazzleCalculationId id) status =

    // get the existing row (if any)
    use dazzleTable = new DazzleDb.dbo.Tables.DazzleCalculation() 
    use cmd = new GetDazzleCalculationById(connection) 
    cmd.Execute(id)  
    |> dazzleTable.Load 

    // update the table
    match dazzleTable.Rows |> Seq.toList with
    | [] ->
        failwithf "DazzleCalculation not found: %s" id
    | [row] 
    | row::_ ->
        updateRowStatusOnStatusChange row status 

    // save to DB
    dazzleTable.AcceptChanges()

/// Delete used by IReadModel        
let deleteDazzleCalculation (connection:SqlConnection) (locationId:DazzleCalculationId) =
    use cmd = new DeleteDazzleCalculationById(connection) 
    cmd.Execute(locationId.Value)  
    |> ignore

    