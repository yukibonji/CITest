namespace XLCatlin.DataLab.XCBRA.ReadModelStore

(*
Types and interfaces for the ReadModelStore, the backing storage for queries on the read model.

The ReadModelStore will likely be implemented as a SQL database or Azure Tables or Redis cache.

Each possible query is explicitly documented.
By being explicit about every possible access path, implementations are strongly guided
*)

open XLCatlin.DataLab.XCBRA.DomainModel

type ReadModelError =
    /// Id or other key not found
    | NotFound of string
    /// Expecting only one record
    | MoreThanOneFound of string
    
type IReadModelStore = 
    //TODO: Convert these into AsyncResult

    abstract member UpdateLocation : value:Location -> Result<unit,ReadModelError>
    abstract member DeleteLocation : key:LocationId -> Result<unit,ReadModelError>
    abstract member GetLocationById : key:LocationId -> Result<Location,ReadModelError>
    abstract member GetLocations : LocationFilter * ApiPaging -> Result<LocationSummary list,ReadModelError>

    abstract member UpdateWarehouse : value:Warehouse-> Result<unit,ReadModelError>
    abstract member DeleteWarehouse : key:BuildingId -> Result<unit,ReadModelError>
    abstract member GetWarehouseById : key:BuildingId -> Result<Warehouse,ReadModelError>
    abstract member GetBuildingsAtLocation : BuildingFilter * ApiPaging -> Result<BuildingSummary list,ReadModelError> 

    abstract member CreateDazzleCalculation : value:DazzleCalculation -> Result<unit,ReadModelError>
    abstract member UpdateDazzleCalculationStatus : id:DazzleCalculationId -> status:DazzleCalculationStatus -> Result<unit,ReadModelError>
    abstract member DeleteDazzleCalculation : key:DazzleCalculationId -> Result<unit,ReadModelError>
