module UnitTestData

/// Simple, known record for use in unit tests

open System
open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.ReadModelStore
open XLCatlin.DataLab.XCBRA.Command
open XLCatlin.DataLab.XCBRA.Query


// ===================================
// Ids
// ===================================

let locationIdA = LocationId "A"
let locationIdB = LocationId "B"
let buildingId101 = BuildingId "101"
let buildingId102 = BuildingId "102"
let buildingId201 = BuildingId "201"
let buildingId202 = BuildingId "202"

// ===================================
// Locations
// ===================================

let createLocation id name address :Location = {
    id=id
    name=name
    description = None
    address = address
    geocode = None
    siteCondition = None
    plantLayout = None
    ownership = None
    lastSurveyDate = None
    }


let addressA : Address = {
    postalCountry = "GB"
    addressLines = ["123 High St"; "London"; "NW1"]
    administrativeArea = ""
    locality = ""
    dependentLocality = None
    postalCode = "NW1"
    sortingCode = None
    languageCode = Some "en-GB"
    }

let addressB : Address = {
    postalCountry = "US"
    addressLines = ["123 Main St"; "New York"; "10001"]
    administrativeArea = ""
    locality = ""
    dependentLocality = None
    postalCode = "10001"
    sortingCode = None
    languageCode = Some "en-US"
    }

let locationA = 
    createLocation locationIdA "LocationA" addressA 

let locationB = 
    createLocation locationIdB "LocationB" addressB

// ===================================
// Warehouses
// ===================================

let createWarehouse id locationId name = 
    { id = id
    ; locationId = locationId
    ; name = name
    ; occupancy = Warehouse
    ; userInput = WarehouseUserInput.empty
    }


let buildingA101 = createWarehouse buildingId101 locationIdA "BuildingA101"
let buildingA102 = createWarehouse buildingId102 locationIdA "BuildingA102"

let buildingB201 = createWarehouse buildingId201 locationIdB "BuildingB201"
let buildingB202 = createWarehouse buildingId202 locationIdB "BuildingB202"



// ===================================
// Events
// ===================================

let locationCreated id name address = 
    LocationEvent.Created (id,name,address,None)

let warehouseCreated id locationId name = 
    WarehouseEvent.Created(id, locationId, name)


// ===================================
// EventStore/Readmodel set up with this data
// ===================================

let appendLocationEvents (eventStore:IEventStore) locationId (events:LocationEvent list) = 
    let streamId = Command.Location.idToEventStream locationId 
    let writeEvents = events |> List.map Command.Location.toWriteEvent
    let expectedVersion = ExpectedVersion.Any 
    let result = eventStore.AppendEventsToStream streamId expectedVersion writeEvents  
    match result with
    | Ok _ -> 
        ()  //ignore
    | Error data -> 
        let msg = sprintf "Error appendLocationEvents '%A'" data
        failwith msg // should never happen

let appendWarehouseEvents (eventStore:IEventStore) warehouseId (events:WarehouseEvent list) = 
    let streamId = Command.Warehouse.idToEventStream warehouseId  
    let writeEvents = events |> List.map Command.Warehouse.toWriteEvent
    let expectedVersion = ExpectedVersion.Any 
    let result = eventStore.AppendEventsToStream streamId expectedVersion writeEvents  
    match result with
    | Ok _ -> 
        ()  //ignore
    | Error data -> 
        let msg = sprintf "Error appendWarehouseEvents '%A'" data
        failwith msg // should never happen


/// set up an event store initialized with the above events
let unitTestEventStore() =
    let utcNow() = DateTime.UtcNow
    let eventStore = MemoryDb.EventStore(utcNow) :> IEventStore
        
    // LocationA
    let events = [
        locationCreated locationA.id locationA.name locationA.address
        ]
    appendLocationEvents eventStore locationA.id events

    // return
    eventStore

/// set up an event store initialized with the above events
let unitTestReadModelStore() =
    let readModelStore = 
        MemoryDb.ReadModelStore() :> IReadModelStore
    let eventStore = unitTestEventStore()
    let result = 
        DomainCommandHandler.replayEventsForAllStreams eventStore readModelStore 
    
    match result with
    | Ok _ -> 
        ()  //ignore

    | Error data -> 
        let msg = sprintf "Error replayEventsForAllStreams '%A'" data
        failwith msg // should never happen

    readModelStore 
    
/// Return a query service, with existing sample data
let sampleQueryService() = 
    let eventStore = unitTestEventStore()
    let readModelStore = unitTestReadModelStore()
    ApiQueryService(readModelStore) :> IApiQueryService
