module XLCatlin.DataLab.XCBRA.SqlDb.Warehouse

open System
open Utilities
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.ReadModelStore
open XLCatlin.DataLab.XCBRA.Query
open System.Data
open System.Data.SqlClient
open System.Collections.Generic  // for Dictionary
open FSharp.Data
open FSharp.Data.SqlClient

// ================================================
// Type Providers
// ================================================

// ResultType.DataReader is used because we will pipe this into a table using "Load"
type GetWarehousesAtLocation = SqlCommandProvider<"""
    SELECT * 
    FROM dbo.Warehouse
    WHERE LocationId = @LocationId 
    """ , "name=Dazzle",ResultType.DataReader>

// ResultType.DataReader is used because we will pipe this into a table using "Load"
type GetWarehouseById = SqlCommandProvider<"""
    SELECT * 
    FROM dbo.Warehouse
    WHERE WarehouseId = @WarehouseId 
    """ , "name=Dazzle",ResultType.DataReader>

type GetAssetValues = SqlCommandProvider<"""
    SELECT * 
    FROM dbo.WarehouseAssetValue
    WHERE WarehouseId = @WarehouseId 
    """ , "name=Dazzle",ResultType.DataReader>

type GetConstructionScores = SqlCommandProvider<"""
    SELECT * 
    FROM dbo.WarehouseConstructionScore
    WHERE WarehouseId = @WarehouseId 
    """ , "name=Dazzle",ResultType.DataReader>


type DeleteWarehouseById = SqlCommandProvider<"""
    DELETE
    FROM dbo.Warehouse
    WHERE WarehouseId = @WarehouseId 
    """ , "name=Dazzle">

type DazzleDb = SqlProgrammabilityProvider<"name=Dazzle">

// ================================================
// Query
// ================================================

/// Create a BuildingSummary from a Warehouse.Row
let rowToBuildingSummary (row:DazzleDb.dbo.Tables.Warehouse.Row) : BuildingSummary =
    {
    BuildingSummary.Id = BuildingId row.WarehouseId
    BuildingSummary.LocationId = LocationId row.LocationId
    Name = row.Name
    }

/// Create an AssetValue from a WarehouseAssetValue.Row
let rowToAssetValue (row:DazzleDb.dbo.Tables.WarehouseAssetValue.Row) : string * AssetValue =
    row.AssetName, {CurrencyCode=CurrencyCode row.CurrencyCode; Value=row.Value}

/// Create a Construction from a string
let toConstruction str : Construction = 
    match str with 
    | null -> 
        Construction.Combustible 
    | "FireResistant" -> 
        Construction.FireResistant
    | "LightNonCombustible" -> 
        Construction.LightNonCombustible
    | "LimitedCombustible" -> 
        Construction.LimitedCombustible
    | _ -> 
        //TODO log this as it should not happen
        Construction.Combustible 

/// Create a ConstructionScore from a WarehouseConstructionScore.Row
let rowToConstructionScore (row:DazzleDb.dbo.Tables.WarehouseConstructionScore.Row) : string * ConstructionScore =
    row.FeatureName, {Construction=row.ConstructionMethod |> toConstruction; Score=row.Score}

/// Create an Occupancy from a string
let toOccupancy str : Occupancy = 
    match str with 
    | null -> 
        Occupancy.Warehouse // default
    | "Warehouse" -> 
        Occupancy.Warehouse
    | _ -> 
        //TODO log this as it should not happen
        Occupancy.Warehouse

/// Create a Warehouse from a Warehouse.Row, plus assetValues and constructionScores
let rowToWarehouse (row:DazzleDb.dbo.Tables.Warehouse.Row) dbAssetValues dbConstructionScores : Warehouse =
    let assetValues : IDictionary<_,_> = dbAssetValues |> dict
    let constructionScores : IDictionary<_,_> = dbConstructionScores |> dict
    {
    Id = BuildingId row.WarehouseId
    LocationId = LocationId row.LocationId 
    Name = row.Name
    Occupancy = row.Occupancy |> toOccupancy 
    YearBuilt = row.YearBuilt |> Option.map Year
    FloorArea = row.FloorAreaSqM |> Option.map toArea
    Compartmentation = row.CompartmentationSqM |> Option.map toArea  
    NumberOfStories = row.NumberOfStories 
    Height = row.HeightM |> Option.map toDistance
    NearestNeighbour = row.NearestNeighbourM |> Option.map toDistance 
    RoofConstruction = constructionScores |> tryGetValue "Roof" |> ifNone []
    WallConstruction = constructionScores |> tryGetValue "Wall" |> ifNone []
    BuildingValue = assetValues |> tryGetValue "Building" 
    MachineryValue = assetValues |> tryGetValue "Machinery" 
    StockValue = assetValues |> tryGetValue "Stock"
    BusinessInterruptionValue = assetValues |> tryGetValue "BusinessInterruption" 
    }

/// Query used by IReadModel
let getWarehousesAtLocation (connection:SqlConnection) (LocationId locationId) (paging:ApiPaging) :BuildingSummary list =
    use cmd = new GetWarehousesAtLocation(connection) 
    //TODO paging not implemented
    let _offset, _fetchNext = int64 paging.Offset,int64 paging.FetchNext
    use warehouseTable = new DazzleDb.dbo.Tables.Warehouse() 
    cmd.Execute(locationId)  
    |> warehouseTable.Load 

    warehouseTable.Rows
    |> Seq.map rowToBuildingSummary
    |> Seq.toList

/// Query used by IReadModel
let getWarehouseById (connection:SqlConnection) (BuildingId id) =

    // load warehouseTable 
    use warehouseTable = new DazzleDb.dbo.Tables.Warehouse() 
    use cmd = new GetWarehouseById(connection) 
    cmd.Execute(id)  
    |> warehouseTable.Load 

    // load assetValueTable 
    use assetValueTable = new DazzleDb.dbo.Tables.WarehouseAssetValue() 
    use cmdAssets = new GetAssetValues(connection) 
    cmdAssets.Execute(id)  
    |> assetValueTable.Load 

    let assetRows = 
        assetValueTable.Rows
        |> Seq.map rowToAssetValue
        |> Seq.toList

    // load constructionScoreTable
    use constructionScoreTable = new DazzleDb.dbo.Tables.WarehouseConstructionScore() 
    use cmdConstructionScores = new GetConstructionScores(connection) 
    cmdConstructionScores.Execute(id)  
    |> constructionScoreTable.Load 

    let constructionScoreRows = 
        constructionScoreTable.Rows
        |> Seq.map rowToConstructionScore
        // convert from list of (featureName,score)
        // into list of (featureName,list of scores)
        |> Seq.groupBy fst
        |> Seq.map (fun (k,kvList) -> k,kvList |> Seq.map snd |> Seq.toList)
        |> Seq.toList

    warehouseTable.Rows
    |> Seq.toList
    |> function
        | [row] -> 
            let warehouse = rowToWarehouse row assetRows constructionScoreRows
            Ok warehouse 
        | [] -> Error (ReadModelError.NotFound "")
        | _ -> Error (ReadModelError.MoreThanOneFound "")


// ================================================
// Update
// ================================================

/// Create a string from an Occupancy 
let fromOccupancy occupancy = 
    match occupancy with 
    | Occupancy.Warehouse -> "Warehouse" 

/// Create a string from a Construction 
let fromConstruction construction = 
    match construction with 
    | Construction.FireResistant -> "FireResistant" 
    | Construction.Combustible -> "Combustible" 
    | Construction.LightNonCombustible -> "LightNonCombustible" 
    | Construction.LimitedCombustible -> "LimitedCombustible" 

/// Update Warehouse.Row from a Warehouse
let updateWarehouseRow (row:DazzleDb.dbo.Tables.Warehouse.Row) (warehouse:Warehouse) =
    row.Name <- warehouse.Name
    row.Occupancy <- warehouse.Occupancy |> fromOccupancy 
    row.YearBuilt <- warehouse.YearBuilt |> Option.map (fun y -> y.Value)
    row.FloorAreaSqM <- warehouse.FloorArea |> Option.map fromArea
    row.CompartmentationSqM <- warehouse.Compartmentation |> Option.map fromArea  
    row.NumberOfStories <- warehouse.NumberOfStories 
    row.HeightM <- warehouse.Height |> Option.map fromDistance
    row.NearestNeighbourM <- warehouse.NearestNeighbour |> Option.map fromDistance 
    // RoofConstruction -- see updateWarehouseConstructionScores below
    // WallConstruction 
    // BuildingValue = see updateWarehouseAssetValueRows below
    // MachineryValue = 
    // StockValue = 
    // BusinessInterruptionValue = 
    row.LastUpdated <- DateTime.UtcNow

/// Update WarehouseAssetValue.Row from an AssetValue
let updateWarehouseAssetValueRow (assetValue:AssetValue) (row:DazzleDb.dbo.Tables.WarehouseAssetValue.Row) =
    row.CurrencyCode <- assetValue.CurrencyCode.Value
    row.Value <- assetValue.Value
    row.LastUpdated <- DateTime.UtcNow
    
/// Create/Update/Delete WarehouseAssetValue rows from a Warehouse
let updateWarehouseAssetValueRows (warehouse:Warehouse) (assetValueTable:DazzleDb.dbo.Tables.WarehouseAssetValue) =
    let tryFindRow assetName = 
        assetValueTable.Rows 
        |> Seq.tryFind (fun row -> row.AssetName = assetName)

    /// Try to pair assetValues with same assetName 
    /// If both found, do update
    /// If score found, do add
    /// If score not found, but row found, do delete
    let matchAssets (assetValueOpt:AssetValue option) assetName =
        let assetValueRowOpt = tryFindRow assetName 
        match assetValueOpt,assetValueRowOpt with
        | Some av, Some row ->
            updateWarehouseAssetValueRow av row 
        | None, Some row ->
            row.Delete()
        | Some av, None ->
            let row = assetValueTable.NewRow()
            // set up the key
            row.WarehouseId <- warehouse.Id.Value
            row.AssetName <- assetName 
            // update the body
            updateWarehouseAssetValueRow av row 
        | None, None ->
            ()
        
    matchAssets warehouse.BuildingValue "Building"
    matchAssets warehouse.BusinessInterruptionValue "BusinessInterruption"
    matchAssets warehouse.MachineryValue "Machinery"
    matchAssets warehouse.StockValue "Stock"

/// Update WarehouseConstructionScore.Row from a ConstructionScore
let updateWarehouseConstructionScoreRow (constructionScore:ConstructionScore) (row:DazzleDb.dbo.Tables.WarehouseConstructionScore.Row) =
    row.ConstructionMethod <- constructionScore.Construction |> fromConstruction
    row.Score <- constructionScore.Score
    row.LastUpdated <- DateTime.UtcNow
    
/// Create/Update/Delete WarehouseConstructionScore rows from a Warehouse
let updateWarehouseConstructionScoreRows (warehouse:Warehouse) (constructionScoreTable:DazzleDb.dbo.Tables.WarehouseConstructionScore) =
    
    /// Try to pair scores with same featureName and constructionMethod
    /// If both found, do update
    /// If score found, do add
    /// If score not found, but row found, do delete
    let matchScores (constructionScores:ConstructionScore list) featureName constructionMethod =
        let constructionMethodStr = constructionMethod |> fromConstruction 

        let tryFindRow featureName = 
            constructionScoreTable.Rows 
            |> Seq.tryFind (fun row -> row.FeatureName = featureName && row.ConstructionMethod = constructionMethodStr )

        let tryFindScore constructionMethod = 
            constructionScores
            |> Seq.tryFind (fun score -> score.Construction = constructionMethod )

        let constructionScoreRowOpt = tryFindRow featureName  
        let constructionScoreOpt = tryFindScore constructionMethod 
        match constructionScoreOpt,constructionScoreRowOpt with
        | Some av, Some row ->
            updateWarehouseConstructionScoreRow av row 
        | None, Some row ->
            row.Delete()
        | Some av, None ->
            let row = constructionScoreTable.NewRow()
            // set up the key
            row.WarehouseId <- warehouse.Id.Value
            row.FeatureName <- featureName
            // update the body
            updateWarehouseConstructionScoreRow av row 
        | None, None ->
            ()

    let allConstructions =
        [
        Construction.Combustible
        Construction.FireResistant
        Construction.LightNonCombustible
        Construction.LimitedCombustible
        ] 
    let allFeaturesAndScores = 
        [
        "Roof",warehouse.RoofConstruction 
        "Wall",warehouse.WallConstruction 
        ]
    for construction in allConstructions do
        for feature,scores in allFeaturesAndScores do
            matchScores scores feature construction

/// Delete used by IReadModel        
let deleteWarehouse (connection:SqlConnection) (warehouseId:BuildingId) =
    use cmd = new DeleteWarehouseById(connection) 
    cmd.Execute(warehouseId.Value)  
    |> ignore

/// Update used by IReadModel    
let updateWarehouse (connection:SqlConnection) (warehouse:Warehouse) =

    // get the existing row (if any)
    use warehouseTable = new DazzleDb.dbo.Tables.Warehouse() 
    use cmd = new GetWarehouseById(connection) 
    cmd.Execute(warehouse.Id.Value)  
    |> warehouseTable.Load 

    // update the warehouseTable
    match warehouseTable.Rows |> Seq.toList with
    | [] ->
        // add new row
        let row = warehouseTable.NewRow()
        updateWarehouseRow row warehouse
    | [row] ->
        // update existing row
        updateWarehouseRow row warehouse
    | row::_ ->
        // Error? more than one row found
        updateWarehouseRow row warehouse

    // save to DB
    warehouseTable.AcceptChanges()
    
    // ------------------------------------
    // get the existing AssetValue rows (if any)
    use assetValueTable = new DazzleDb.dbo.Tables.WarehouseAssetValue() 
    use cmdAssets = new GetAssetValues(connection) 
    cmdAssets.Execute(warehouse.Id.Value)  
    |> assetValueTable.Load 

    // update the asset values
    updateWarehouseAssetValueRows warehouse assetValueTable

    // save to DB
    assetValueTable.AcceptChanges()

    // ------------------------------------
    // get the existing Score rows (if any)
    use constructionScoreTable = new DazzleDb.dbo.Tables.WarehouseConstructionScore() 
    use cmdConstructionScores = new GetConstructionScores(connection) 
    cmdConstructionScores.Execute(warehouse.Id.Value)  
    |> constructionScoreTable.Load 

    // update the scores
    updateWarehouseConstructionScoreRows warehouse constructionScoreTable

    // save to DB
    constructionScoreTable.AcceptChanges()



