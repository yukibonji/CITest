namespace XLCatlin.DataLab.XCBRA.SqlDb

open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.ReadModelStore
open System.Collections.Generic
open Savanna.Utilities
open System.Data.SqlClient

/// A SQL implementation of an read model store
type ReadModelStore(connection:SqlConnection) =
        
    interface IReadModelStore with

        member __.UpdateLocation location =
            Location.updateLocation connection location |> Ok
        member __.DeleteLocation locationId =
            Location.deleteLocation connection locationId |> Ok
        member __.GetLocationById locationId =
            Location.getLocationById connection locationId 
        member __.GetLocations (filter,paging) =
            Location.getLocations connection filter paging |> Ok
        member __.UpdateWarehouse warehouse =
            Warehouse.updateWarehouse connection warehouse |> Ok
        member __.DeleteWarehouse warehouseId =
            Warehouse.deleteWarehouse connection warehouseId |> Ok
        member __.GetWarehouseById buildingId =
            Warehouse.getWarehouseById connection buildingId 
        member __.GetBuildingsAtLocation(buildingFilter,paging) =
            Building.getBuildingsAtLocation connection buildingFilter paging |> Ok
        member __.CreateDazzleCalculation (value:DazzleCalculation) =
            DazzleCalculation.createDazzleCalculation connection value |> Ok
        member __.UpdateDazzleCalculationStatus id status =
            DazzleCalculation.updateDazzleCalculationStatus connection id status |> Ok
        member __.DeleteDazzleCalculation id =
            DazzleCalculation.deleteDazzleCalculation connection id |> Ok
                