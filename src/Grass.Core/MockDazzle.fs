/// A mock dazzle engine for testing
module MockDazzle

open XLCatlin.DataLab.XCBRA.DomainModel

/// Dazzle interface (could be moved to Dazzle project?)
type IDazzleCalculation =
    abstract member Id : DazzleCalculationId 

    /// The input used to drive the calculation
    abstract member Input : DazzleCalculationInput
    
    /// Current status
    abstract member Status : DazzleCalculationStatus 

    /// Triggered whenever the calculation has made progress. 
    /// On completion, one final status change is triggered.
    abstract member StatusChanged : IEvent<DazzleCalculationStatus>

// ===========================
// Mock implementation for testing
// ===========================

/// Initial estimate. Will be changed by long-running process as it progresses.
let mutable defaultDuration = 10000<Ms>

// Change the default duration (eg for unit tests)
let setDuration duration = 
    defaultDuration <- duration


type MockDazzleCalculation(id, input) =
    
    let mutable remainingMs = defaultDuration 
    let loopDuration = defaultDuration / 10

    let status() = 
        if remainingMs > 0<_> then
            DazzleCalculationStatus.InProcess remainingMs
        else
            let output = ""
            DazzleCalculationStatus.Completed (defaultDuration,output)

    let statusChanged = new Event<DazzleCalculationStatus>()

    let calculation = async {
        // in process
        while remainingMs > 0<_> do
            do! Async.Sleep (loopDuration * 1<_>)
            // System.Console.WriteLine("{0} remaining {1}",id,remainingMs)
            statusChanged.Trigger( status() ) 
            remainingMs <- (remainingMs - loopDuration)

        // completed
        // System.Console.WriteLine("Completed")
        remainingMs <- 0<_>
        statusChanged.Trigger( status() ) 
        } 


    // start now
    do calculation |> Async.Start
    
    with 
    interface IDazzleCalculation with
        member this.Id = id
        member this.Input = input
        member this.Status = status()
        member this.StatusChanged = statusChanged.Publish

