namespace XLCatlin.DataLab.XCBRA.MemoryDb

open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.ReadModelStore
open System.Collections.Generic
open Savanna.Utilities

/// An in-memory implementation of an read model store
type ReadModelStore() =
        
    // each of these will be a table in a database            
    let locations = Dictionary<LocationId,Location>()

    let warehouses = Dictionary<BuildingId,Warehouse>()

    let dazzleCalculations = Dictionary<DazzleCalculationId,DazzleCalculation>()

    /// object to act as lock monitor. Could use agents instead.
    let lockMonitor = obj()

    /// Convert the result of a TryGet to a Result
    let tryGetToResult id tryGet = 
        match tryGet with
        | true,v -> Ok v
        | false,_ -> Error <| ReadModelError.NotFound (sprintf "%A" id)

    let toLocationSummary isUserFavourite (l:Location)  : LocationSummary =
        { id = l.id
        ; name = l.name
        ; description = l.description |> ifNone "No description."
        ; address = l.address
        ; isFavourite = isUserFavourite
        }

    let toBuildingSummary (w:Warehouse) :BuildingSummary =
        { id = w.id
        ; locationId = w.locationId
        ; name = w.name
        ; occupancy = w.occupancy
        ; userInput = ()
        }

    let lockedUpdate (dict:Dictionary<_,_>) key value = 
        lock lockMonitor (fun () -> 
            dict.[key] <-value
            )

    let lockedReplace (dict:Dictionary<_,_>) key replaceFn = 
        lock lockMonitor (fun () -> 
            match dict.TryGetValue(key) with
            | true,value ->
                dict.[key] <- replaceFn value
            | false,_ ->
                () // ignored
            )

    let lockedDelete (dict:Dictionary<_,_>) key = 
        lock lockMonitor (fun () -> 
            dict.Remove(key) |> ignore
            )

    interface IReadModelStore with

        member __.UpdateLocation location =
            lockedUpdate locations location.id location |> Ok

        member __.DeleteLocation locationId =
            lockedDelete locations locationId |> Ok

        member __.GetLocationById locationId =
            let tryGet = locations.TryGetValue(locationId) 
            tryGet |> tryGetToResult locationId 

        member __.GetLocations (_filter,_paging) =
            //TODO: implement filter and paging
            locations.Values 
            //TODO: implement user favourites
            |> Seq.map (toLocationSummary false)
            |> Seq.toList
            |> Ok

        member __.UpdateWarehouse warehouse =
            lockedUpdate warehouses warehouse.id warehouse 
            |> Ok

        member __.DeleteWarehouse warehouseId =
            lockedDelete warehouses warehouseId 
            |> Ok

        member __.GetWarehouseById buildingId =
            let tryGet = warehouses.TryGetValue(buildingId) 
            tryGet 
            |> tryGetToResult buildingId 

        member __.GetBuildingsAtLocation(buildingFilter,_paging) =
            let filter = 
                match buildingFilter.locationId with
                | Some id -> Seq.filter ( fun (b:Building<_>) -> b.locationId = id )
                | _ -> id

            warehouses.Values 
            |> filter 
            |> Seq.map toBuildingSummary
            |> Seq.toList 
            |> Ok
                
        member __.CreateDazzleCalculation (value:DazzleCalculation) =
            lockedUpdate dazzleCalculations value.Id value 
            |> Ok

        member __.UpdateDazzleCalculationStatus id status =
            let replaceFn value = {value with Status=status}
            lockedReplace dazzleCalculations id replaceFn 
            |> Ok

        member __.DeleteDazzleCalculation id =
            lockedDelete dazzleCalculations id 
            |> Ok
