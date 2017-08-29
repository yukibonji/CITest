# Savannah web service

The Savannah web service is the web server component in the Dazzle project.
It uses the [Suave](https://suave.io/) web framework to serve content.

It implements the following APIs from the DomainModel project in a HTTP-friendly way:

* `IApiCommandExecutionService`
* `IApiCommandStatusService`
* `IApiQueryService`
* `IApiDazzleService`

## Contents

This project contains:

* Command-related
  * The Web API endpoint for commands
* Query-related
  * The Web API endpoints for the queries 
* GraphQL-related
  * The Web API endpoint for GraphQL 
* Dazzle-related
  * The Web API endpoint for Dazzle requests

## To Compile and Run

* Run `build` from the root directory to build all the projects
* Start the web service with `src\Savanna.Web\bin\Release\Savanna.Web.exe`
* The default port is localhost:8081. This can be changed in the `program.fs` file.
src\Savanna.Web\bin\Release\Savanna.Web.exe

## Endpoints overview

Commands and queries have separate endpoints

* Standand queries each have their own endpoint `/[queryName]?param1=xx&param2=yy`.
  GET must be used.
* Queries using the GraphQL language have endpoint is `/graphql`.
  GET must be used.
* There is one endpoint for all Commands. The endpoint is `/command`. POST must be used.
* To check the status of long running commands, use the endpoint `/commandstatus`. GET must be used.
* Calls to the Dazzle risk model are at `/dazzle`. See Dazzle section below.


## Serialization

It's important that the client and server agree on serialization.
The current library used is `Microsoft.FSharpLu.Json.Default`.

This can be changed in the `ApiSerialization` module in this project.

## Commands

To post a command, use the `ApiCommand` structure documented in the DomainModel project,
and serialize it.

For synchronous results, POST it to the `/command` endpoint.
The http statuses are:

* `200`. The command succeeded
* `400`. The command was malformed or could not be executed (e.g. updating an unknown record)
* `401`. Authentication issue.
* `500`. A server error. (e.g. database connection failed)

For asynchronous results, POST it to the `/commandqueue` endpoint. This will always return 202.

The command will be queued. You will have to check the status explicitly later.

## Command Status

To check the status of a command, GET the endpoint `/commandstatus/{id}`.
The http statuses are:

* `200`. The status was successfully returned
* `400`. The command id was malformed
* `401`. Authentication issue.
* `500`. A server error. (e.g. database connection failed)

The body of the result will contain a serialized `ApiCommandStatus` (from the DomainModel project).

* Pending
* Success
* Failure (of message)
* NotFound -- the CommandId was not found, 
  generally because the original command request was malformed and never even passed the validation stage
  to go into the command pipeline.


## Queries

To make a query, GET one of the endpoints below. 
The http statuses are:

* `200`. The results were successfully returned.
* `400`. The query was malformed
* `401`. Authentication issue.
* `500`. A server error. (e.g. database connection failed)

The available query endpoints are:

* `/locations?filterKey=filterValue&filterKey=filterValue&` where `filterKey` is one of the
  properties in `LocationFilter`. The results are a list of `LocationSummary`
* `/location/{id}` where `id` is the LocationId
  The result is a single `Location`
* `/location/{id}/buildings?filterKey=filterValue&...` where `id` is the LocationId
  and `filterKey` is one of the properties of `BuildingFilter`.
  The results are a list of `BuildingSummary`
* etc (TODO)

For all routes, see the code in the `WebApi` module in this project.
  
## Dazzle

Calls to the Dazzle risk model are at `/dazzle`. 

* POST a `DazzleStartCommand` (from `DomainModel/ApiDazzleService.fs`) to start a long-running calculation.
* GET `/dazzle/{id}` to get the status.

In both cases the HTTP response body will be a `DazzleCalculationStatus` (from `DomainModel/DazzleCalculations.fs`)
if successful.

The http statuses are:

* `200`. The request was success
* `400`. The command or query was malformed
* `401`. Authentication issue.
* `500`. A server error. (e.g. database connection failed)


## Authentication

TO DO

