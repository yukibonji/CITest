module XLCatlin.DataLab.XCBRA.Dazzle

open MockDazzle
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.ReadModelStore

/// Manages the background tasks
type DazzleCalculationManager(readModelStore:IReadModelStore) =
    
    let calculations = System.Collections.Generic.Dictionary<DazzleCalculationId,IDazzleCalculation>()
    
    let statusChanged = new Event<DazzleCalculationId * DazzleCalculationStatus>()

    let storeStatus(id,status) =
        readModelStore.UpdateDazzleCalculationStatus id status
        |> ignore

    /// Start a calculation and return it
    member this.Start id input = 
        let calculation = new MockDazzleCalculation(id,input) :> IDazzleCalculation
        calculations.Add(id,calculation) 
        calculation.StatusChanged.Add(fun status -> 
            storeStatus(id,status)
            statusChanged.Trigger(id,status)
            )
        calculation

    /// Status of calculation with given id or None if id is not found
    member this.Status id =
        match calculations.TryGetValue(id) with
        | true,calculation -> calculation.Status |> Some
        | false,_ -> None

    /// Triggered when the status of any calculation changes
    member this.StatusChanged = 
        statusChanged.Publish

    /// List of all calculations 
    member this.Calculations() = 
        calculations
        |> Seq.cast<IDazzleCalculation>

// ================================================
// Implementation of IApiDazzleService 
// ================================================
    
type ApiDazzleService(readModelStore:IReadModelStore) =
    let calculationManager = DazzleCalculationManager(readModelStore)

    let toRecord (calc:IDazzleCalculation) :DazzleCalculation =
        {
        Id = calc.Id
        Input = calc.Input
        Status = calc.Status
        }

    let isPending (calc:IDazzleCalculation) =
        match calc.Status with
        | InProcess _ -> true
        | Completed _ -> false

    with 
    interface IApiDazzleService with
    
        /// Start a calculation and return its status
        member this.Start(cmd) =
            let calculation = calculationManager.Start cmd.Id cmd.Input
            calculation.Status
            |> AsyncResult.retn

        /// Status of calculation with given id or DazzleError if id is not found
        member this.Status id =
            match calculationManager.Status id with
                | Some status -> 
                    status |> Ok
                | None -> 
                    DazzleError.DazzleError (sprintf "Id not found: %A" id)
                    |> Error
            |> Async.retn

        /// List of all pending calculations 
        member this.PendingCalculations() =
            calculationManager.Calculations()
            |> Seq.filter isPending
            |> Seq.map toRecord
            |> Seq.toList
            |> AsyncResult.retn
        
        /// List of all pending and completed calculations 
        member this.AllCalculations() =
            calculationManager.Calculations()
            |> Seq.map toRecord
            |> Seq.toList
            |> AsyncResult.retn
