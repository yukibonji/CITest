module XLCatlin.DataLab.XCBRA.SqlDb.Location

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

// NOTE this is not optimal code
// See https://stackoverflow.com/questions/2161573/how-to-optimize-the-use-of-the-or-clause-when-used-with-parameters-sql-server
// ResultType.DataReader is used because we will pipe this into a table using "Load"
type GetLocations = SqlCommandProvider<"""
    -- workaround for one-param limitation
    DECLARE @NamePattern1 nvarchar(100)
    SET @NamePattern1 = @NamePattern
    DECLARE @PostCodePattern1 nvarchar(100)
    SET @PostCodePattern1 = @PostCodePattern
    SELECT * 
    FROM dbo.Location
    WHERE ((@NamePattern1 = '') OR 
           (Name LIKE @NamePattern1))
    AND ((@PostCodePattern1 = '') OR 
         (Address_PostalCode LIKE @PostCodePattern1)) 
    ORDER BY Address_PostalCode
    OFFSET @Offset ROWS
    FETCH NEXT @FetchNext ROWS ONLY
    """ , "name=Dazzle",ResultType.DataReader>

// ResultType.DataReader is used because we will pipe this into a table using "Load"
type GetLocationById = SqlCommandProvider<"""
    SELECT * 
    FROM dbo.Location
    WHERE LocationId = @LocationId 
    """ , "name=Dazzle",ResultType.DataReader>

type GetAssetValues = SqlCommandProvider<"""
    SELECT * 
    FROM dbo.LocationAssetValue
    WHERE LocationId = @LocationId 
    """ , "name=Dazzle",ResultType.DataReader>

type DeleteLocationById = SqlCommandProvider<"""
    DELETE
    FROM dbo.Location
    WHERE LocationId = @LocationId 
    """ , "name=Dazzle">

type DazzleDb = SqlProgrammabilityProvider<"name=Dazzle">

// ================================================
// Query
// ================================================

/// Create an Address from a Location.Row
let rowToAddress (row:DazzleDb.dbo.Tables.Location.Row) : Address =
    {
    PostalCountry = row.Address_PostalCountry
    AddressLines =  
        let firstLine = row.Address_AddressLine1
        let otherLines = 
            [
            row.Address_AddressLine2
            row.Address_AddressLine3
            row.Address_AddressLine4
            row.Address_AddressLine5
            ] |> List.choose id
        firstLine :: otherLines
    AdministrativeArea = row.Address_AdministrativeArea |> ifNone ""
    Locality = row.Address_Locality |> ifNone ""
    DependentLocality = row.Address_DependentLocality 
    PostalCode = row.Address_PostalCode
    SortingCode = row.Address_SortingCode
    LanguageCode = row.Address_LanguageCode
    }

/// Create a Geocode from a Location.Row
let rowToGeocode (row:DazzleDb.dbo.Tables.Location.Row) : Coordinate option =
    match row.Geocode_Latitude, row.Geocode_Longitude with
    | Some lat, Some long ->
        {Latitude=lat; Longitude=long} |> Some
    | _ -> 
        None

/// Create a LocationSummary from a Location.Row
let rowToSummary (row:DazzleDb.dbo.Tables.Location.Row) : LocationSummary =
    {
    LocationSummary.Id = LocationId row.LocationId
    Name = row.Name
    Description = row.Description
    LocationSummary.Address = (rowToAddress row).Summary
    }

/// Create an AssetValue from a LocationAssetValue.Row
let rowToAssetValue (row:DazzleDb.dbo.Tables.LocationAssetValue.Row) : string * AssetValue =
    row.AssetName, {CurrencyCode=CurrencyCode row.CurrencyCode; Value=row.Value}

/// Create an QualitativeRating from a string
let toQualitativeRating str : QualitativeRating option = 
    match str with 
    | null -> None
    | "Excellent" -> 
        Some QualitativeRating.Excellent
    | "Good" -> 
        Some QualitativeRating.Good
    | "Fair" -> 
        Some QualitativeRating.Fair
    | "Poor" -> 
        Some QualitativeRating.Poor
    | _ -> 
        //TODO log this as it should not happen
        None // unknown

/// Create an Ownership from a string
let toOwnership str : Ownership option = 
    match str with 
    | null -> None
    | "Leased" -> 
        Some Ownership.Leased
    | "Owned" -> 
        Some Ownership.Owned
    | "OwnedOccupant" -> 
        Some Ownership.OwnedOccupant
    | "Tenant" -> 
        Some Ownership.Tenant
    | _ -> 
        //TODO log this as it should not happen
        None // unknown

/// Create an Location from a Location.Row and list of asset values
let rowToLocation (row:DazzleDb.dbo.Tables.Location.Row) dbAssetValues : Location =
    let assetValues : IDictionary<_,_> = dbAssetValues |> dict
    {
    Id = LocationId row.LocationId
    Name = row.Name
    Description = row.Description |> noneIfEmpty
    Address = rowToAddress row
    Geocode = rowToGeocode row
    BuildingValue = assetValues |> tryGetValue "Building" 
    MachineryValue = assetValues |> tryGetValue "Machinery" 
    StockValue = assetValues |> tryGetValue "Stock"
    BusinessInterruptionValue = assetValues |> tryGetValue "BusinessInterruption" 
    TotalArea = row.TotalAreaSqM |> Option.map toArea
    SiteCondition = row.SiteCondition |> Option.bind toQualitativeRating 
    PlantLayout = row.PlantLayout |> Option.bind toQualitativeRating 
    Ownership = row.Ownership |> Option.bind toOwnership 
    }

/// Query used by IReadModel
let getLocations (connection:SqlConnection) (filter:LocationFilter) (paging:ApiPaging) =
    use cmd = new GetLocations(connection) 
    let namePattern = filter.Name |> defaultArg <| ""
    let postCodePattern = filter.PostalCountry |> defaultArg <| ""
    let offset, fetchNext = int64 paging.Offset,int64 paging.FetchNext
    use locationTable = new DazzleDb.dbo.Tables.Location() 
    cmd.Execute(namePattern,postCodePattern,offset,fetchNext)  
    |> locationTable.Load 

    locationTable.Rows
    |> Seq.map rowToSummary
    |> Seq.toList

/// Query used by IReadModel
let getLocationById (connection:SqlConnection) (LocationId id) =
    use locationTable = new DazzleDb.dbo.Tables.Location() 
    use cmd = new GetLocationById(connection) 
    cmd.Execute(id)  
    |> locationTable.Load 

    use assetValueTable = new DazzleDb.dbo.Tables.LocationAssetValue() 
    use cmdAssets = new GetAssetValues(connection) 
    cmdAssets.Execute(id)  
    |> assetValueTable.Load 

    let assetRows = 
        assetValueTable.Rows
        |> Seq.map rowToAssetValue
        |> Seq.toList

    locationTable.Rows
    |> Seq.toList
    |> function
        | [row] -> 
            let location = rowToLocation row assetRows
            Ok location 
        | [] -> Error (ReadModelError.NotFound "")
        | _ -> Error (ReadModelError.MoreThanOneFound "")


// ================================================
// Update
// ================================================

/// Update Location.Row from an Address
let updateLocationRowAddress (row:DazzleDb.dbo.Tables.Location.Row) (address:Address) =
    row.Address_PostalCountry <- address.PostalCountry
    let getAddressLine n = 
        try
            address.AddressLines 
            |> List.toArray 
            |> fun arr -> arr.[n-1] |> Some
        with
        | ex -> None
    row.Address_AddressLine1 <- getAddressLine 1 |> ifNone ""
    row.Address_AddressLine2 <- getAddressLine 2 
    row.Address_AddressLine3 <- getAddressLine 3 
    row.Address_AddressLine4 <- getAddressLine 4 
    row.Address_AddressLine5 <- getAddressLine 5 
    row.Address_AdministrativeArea <- address.AdministrativeArea |> noneIfEmpty
    row.Address_Locality <- address.Locality |> noneIfEmpty
    row.Address_DependentLocality <- address.DependentLocality 
    row.Address_PostalCode <- address.PostalCode
    row.Address_SortingCode <- address.SortingCode
    row.Address_LanguageCode <- address.LanguageCode

/// Create a string from a QualitativeRating
let fromQualitativeRating rating = 
    match rating with 
    | QualitativeRating.Excellent -> "Excellent" 
    | QualitativeRating.Good -> "Good" 
    | QualitativeRating.Fair -> "Fair" 
    | QualitativeRating.Poor -> "Poor" 

/// Create a string from an Ownership
let fromOwnership ownership = 
    match ownership with 
    | Ownership.Leased -> "Leased" 
    | Ownership.Owned -> "Owned" 
    | Ownership.OwnedOccupant -> "OwnedOccupant" 
    | Ownership.Tenant -> "Tenant" 

/// Update Location.Row from a Location
let updateLocationRow (row:DazzleDb.dbo.Tables.Location.Row) (location:Location) =
    row.Name <- location.Name
    row.Description <- location.Description |> ifNone ""
    updateLocationRowAddress row location.Address
    row.Geocode_Latitude <- location.Geocode |> Option.map (fun geocode -> geocode.Latitude) 
    row.Geocode_Longitude <- location.Geocode |> Option.map (fun geocode -> geocode.Longitude) 
//    row.BuildingValue = see updateLocationAssetValueRows below
//    row.MachineryValue = 
//    row.StockValue = 
//    row.BusinessInterruptionValue = 
    row.TotalAreaSqM <- location.TotalArea |> Option.map fromArea
    row.SiteCondition <- location.SiteCondition |> Option.map fromQualitativeRating 
    row.PlantLayout <- location.PlantLayout |> Option.map fromQualitativeRating 
    row.Ownership <- location.Ownership |> Option.map fromOwnership 
    row.LastUpdated <- DateTime.UtcNow

/// Update LocationAssetValue.Row from an AssetValue
let updateLocationAssetValueRow (assetValue:AssetValue) (row:DazzleDb.dbo.Tables.LocationAssetValue.Row) =
    row.CurrencyCode <- assetValue.CurrencyCode.Value
    row.Value <- assetValue.Value
    row.LastUpdated <- DateTime.UtcNow
    
/// Create/Update/Delete LocationAssetValue rows from a Location
let updateLocationAssetValueRows (location:Location) (assetValueTable:DazzleDb.dbo.Tables.LocationAssetValue) =
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
            updateLocationAssetValueRow av row 
        | None, Some row ->
            row.Delete()
        | Some av, None ->
            let row = assetValueTable.NewRow()
            // set up the key
            row.LocationId <- location.Id.Value
            row.AssetName <- assetName 
            // update the body
            updateLocationAssetValueRow av row 
        | None, None ->
            ()
        
    matchAssets location.BuildingValue "Building"
    matchAssets location.BusinessInterruptionValue "BusinessInterruption"
    matchAssets location.MachineryValue "Machinery"
    matchAssets location.StockValue "Stock"

/// Delete used by IReadModel        
let deleteLocation (connection:SqlConnection) (locationId:LocationId) =
    use cmd = new DeleteLocationById(connection) 
    cmd.Execute(locationId.Value)  
    |> ignore

/// Update used by IReadModel        
let updateLocation (connection:SqlConnection) (location:Location) =

    // get the existing row (if any)
    use locationTable = new DazzleDb.dbo.Tables.Location() 
    use cmd = new GetLocationById(connection) 
    cmd.Execute(location.Id.Value)  
    |> locationTable.Load 

    // update the locationTable
    match locationTable.Rows |> Seq.toList with
    | [] ->
        // add new row
        let row = locationTable.NewRow()
        updateLocationRow row location
    | [row] ->
        // update existing row
        updateLocationRow row location
    | row::_ ->
        // Error? more than one row found
        updateLocationRow row location

    // save to DB
    locationTable.AcceptChanges()

    // ------------------------------------    
    // get the existing rows (if any)
    use assetValueTable = new DazzleDb.dbo.Tables.LocationAssetValue() 
    use cmdAssets = new GetAssetValues(connection) 
    cmdAssets.Execute(location.Id.Value)  
    |> assetValueTable.Load 

    // update the asset values
    updateLocationAssetValueRows location assetValueTable

    // save to DB
    assetValueTable.AcceptChanges()
