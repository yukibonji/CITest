namespace XLCatlin.DataLab.XCBRA.DomainModel

/// Errors associated with a bad Dazzle execution
type DazzleError =
    /// 400: The data in the input is invalid, return a list of errors
    | ValidationError of string
    /// 400: The input for the dazzle calculation cannot be executed 
    | DazzleError of string
    /// 403: Credentials are bad
    | AuthenticationError of string
    /// 500: A server side error
    | ServerError of string

/// Information needed to start a calculation
type DazzleStartCommand = {
    Id: DazzleCalculationId
    Input: DazzleCalculationInput 
    }

/// The Dazzle service has one method/endpoint for each possible command/query
type IApiDazzleService = 
    /// Start a calculation and return its status
    abstract member Start: DazzleStartCommand -> Async<Result<DazzleCalculationStatus,DazzleError>>
    // Might have `Cancel` or `Recalculate` later

    /// Status of calculation with given id or DazzleError if id is not found
    abstract member Status: DazzleCalculationId -> Async<Result<DazzleCalculationStatus,DazzleError>>
    /// List of all pending calculations 
    abstract member PendingCalculations: unit -> Async<Result<DazzleCalculation list,DazzleError>>
    /// List of all pending and completed calculations 
    abstract member AllCalculations: unit -> Async<Result<DazzleCalculation list,DazzleError>>
