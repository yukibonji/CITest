module XLCatlin.DataLab.XCBRA.SqlDb.Building

open System
open Utilities
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.Query
open System.Data
open System.Data.SqlClient
open System.Collections.Generic  // for Dictionary
open FSharp.Data
open FSharp.Data.SqlClient

/// Query used by IReadModel
let getBuildingsAtLocation (connection:SqlConnection) (buildingFilter:BuildingFilter) (paging:ApiPaging) :BuildingSummary list =
    [    
    yield! Warehouse.getWarehousesAtLocation connection buildingFilter.LocationId paging
    // yield! other building types
    ]
    
