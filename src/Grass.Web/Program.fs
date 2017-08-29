open Suave                 // always open suave
open System.Net

open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.Command
open XLCatlin.DataLab.XCBRA.Query
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.Dazzle



//TODO choose data store based on config
let eventStore = 
    XLCatlin.DataLab.XCBRA.SampleDbDataStore.eventStore() 
    // MemoryDbDataStore.SampleData.eventStore()

let readModelStore = 
    SampleDbDataStore.readModelStore(eventStore)
    //MemoryDbDataStore.readModelStore(eventStore) 

let commandStatusStore = 
    MemoryDbDataStore.commandStatusStore() 

let commandService = 
    ApiCommandService(eventStore,readModelStore,commandStatusStore) :> 
        IApiCommandExecutionService 

let queryService = 
    ApiQueryService(readModelStore) :> 
        IApiQueryService

let dazzleService = 
    ApiDazzleService(readModelStore) :> 
        IApiDazzleService

let routes = 
    XLCatlin.DataLab.XCBRA.WebApi.routes
        ( eventStore
        , readModelStore
        , commandService
        , queryService
        , commandStatusStore
        , dazzleService
        )

let binding = 
    HttpBinding.create HTTP IPAddress.Loopback 8081us // us=unsigned short!

let config = 
    { defaultConfig with bindings = [binding] }

startWebServer config routes 
