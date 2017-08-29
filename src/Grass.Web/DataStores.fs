namespace XLCatlin.DataLab.XCBRA

open System
open XLCatlin.DataLab.XCBRA.ReadModelStore
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.CommandStatusStore

/// Globals for the data store
module MemoryDbDataStore =

    let eventStore() = 
        let utcNow() = DateTime.UtcNow
        MemoryDb.EventStore(utcNow) :> IEventStore 

    let readModelStore() = 
        MemoryDb.ReadModelStore() :> IReadModelStore 

    let commandStatusStore() = 
        MemoryDb.CommandStatusStore() :> ICommandStatusStore

/// Globals for the sample data store
module SampleDbDataStore =

    let eventStore() = 
        MemoryDb.SampleData.eventStore()

    let readModelStore(eventStore) = 
        MemoryDb.SampleData.readModelStore(eventStore)

/// Globals for the SQL data stores
module SqlDbDataStore =
    // todo
    ()            