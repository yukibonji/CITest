# Domain Model

This project contains the domain model that is shared between the front-end and the back-end.

For each Entity/Aggregate (such as Location), there are:

* Command types for each command that can be executed against the Aggregate.
  Note that each command can only target *one* aggregate.
* ReadModel(s) for query results. These can include data from multiple aggregates.

## Interfaces

Interfaces are used define behavior that is common across multiple types.
The definition and implementation of these interface are included in this project.

## Design notes

* Only records and unions should be used to define the public API.
  Tuples and Aliases should not be used.
* For convenience, the full definition of each Aggregate (not exposed in the API) is also in this project
* For convenience, the various events triggered as a result of the commands are also in this project


## API and breaking changes

The Commands and ReadModels are used by the front end, and so these form the "API"
that the front-end uses to talk to the back-end.  Therefore, any changes to these structures could potentially
break the API. 

## Serialization

The front-end and back-end must use the same serialization format.
This will be [FSharpLu.Json](https://github.com/Microsoft/fsharplu/wiki/fsharplu.json) because it
works much better with F# union types.





