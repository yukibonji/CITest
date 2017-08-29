# Savanna web service

The Savanna web service is the web server component in the Dazzle project.
It consists of these projects:

* `DomainModel`. The definitions of all the domain types, commands, and events,
  plus the Query and Command APIs that are exposed in the web service (so that the
  front-end client can use the same API)
* `Savanna.Core` (this project). The core logic for processing commands, events, etc
* `Savanna.Web`. A web server (see README in that project)
* `Savanna.MemoryDb`. A memory backed implementation of the storage interfaces (IEventStore, IReadModel, etc).
* `Savanna.SqlDb`. A SQL Server backed implementation of the storage interfaces.
* A test project for each of these: `Savanna.Core.Test`, `Savanna.Web.Test`, etc.

## Contents

This project contains code for:

* The main Command service that implements commands (`Command.CommandService.fs`)
  and the command-related logic for each domain (eg `Command.Location.fs`).
  NOTE: Commands themselves are defined in the `DomainModel` project.
* The Query service that implements queries agains the read model (`Query.QueryService.fs`).
  NOTE: The query results (readmodel types) are defined in the `DomainModel` project.
* The GraphQL queries on the each Aggregate/Entity (e.g. `GraphQL.Warehouse.fs`) 
  and the GraphQL service that implements GraphQL queries (`GraphQL.QueryService.fs`).

## Commands

The command handler logic is in `Command.CommandService.fs`

The key function is `processCommand`, which:

* accepts a command
* rebuilds the precommand state
* applies the command to the precommand state to generate some events
* applies the events to the precommand state to get a postcommand state
* appends the events to the event store
* updates the read model with the postcommand state

For this to work, `processCommand` needs to have specific functions for:

* applying a domain event to a current entity state, resulting in a new state
* executing a command against a current state, resulting in a new events
* functions to convert a domain event to/from the event structures needed by the IEventStore
* functions to update the IReadModel

These are made available from a `EventSourcingFunctions` structure which is passed into to it.

The `executeCommand` function is the wrapper that looks at the kind of subdomain of the command
(Location command, Warehouse command, etc) and creates the `EventSourcingFunctions` needed from
the subdomain to pass on to `processCommand`.

The `IApiCommandExecutionService` is also implemented in `Command.CommandService.fs`.
It is basically a wrapper for `executeCommand` that keeps track of the command status.

### Domain specific functions

Each subdomain (`Location`, `Warehouse`, etc) is responsible for providing the functions
that `processCommand` needs. At the bottom of `Command.Location.fs` for example, there is a 
`eventSourcingFunctions` function that gathers the various functions and returns them.

### Storage: `IEventStore` and `IReadModel`

The two data stores needed by the system are `IEventStore` and `IReadModel`.

* `IEventStore` is responsible for reading events from an event stream, and
  appending events (never changing existing events!). 
  The implementation is hidden from Savanna.Core and Savanna.Web -- they just work from the interface.
  The definition of `IEventStore` is in `IEventStore.fs` 

* `IReadModelStore` is responsible for storing the current state of entities
  to make queries easier. The read model is *not* the system of record -- the event store is.
  If needed, the read model store could be deleted and rebuilt from the events.
  The implementation is hidden from Savanna.Core and Savanna.Web -- they just work from the interface.
  The definition of `IReadModelStore` is in `IReadModelStore.fs` 

* `ICommandStatusStore` is responsible for tracking the state of queued commands, E.g. Pending/Success/Failure.

### `DomainEvents` vs `WriteEvents` vs `RecordedEvents`.

* Domain events are the domain-specific events, written as unions and defined in the 
  `DomainModel` project.
* A `WriteEvent` is specific to the IEventStore storage implementation.
  It contains everything needed to write to the event stream.
  It is a flattened struture (the data field contains a json-serialized domain event)
* A `RecordedEvent` is what is actually stored in the event stream.
  It has all the fields of `WriteEvent`, plus some generated fields such as `EventNumber`, `Created` etc.

## Queries

The `ApiQueryService` is implemented in `Query.QueryService.fs`

Most of the functions are just passthroughs to the corresponding functions on the IReadModel.

## Dazzle

Dazzle calculations are transient and therefore event sourcing is not used to reconstruct state.

As a result, Dazzle calculations do not use the standard Command process.
Instead they have their own API -- see `ApiDazzleService` in `DomainModel` project.

The API is implemented in the file `Dazzle.Service.fs`. 
The actual Dazzle calculation is not used, so for testing there is a `MockDazzle` that just executes for a long time.



