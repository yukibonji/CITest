module WebTestUtilities

open System
open Expecto
open Suave
open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.Command
open XLCatlin.DataLab.XCBRA.Query
open XLCatlin.DataLab.XCBRA.MemoryDb.SampleData
open UnitTestData

// ========================================
// Services
// ========================================


/// Return both a command and a query service, with no existing data
let emptyCommandAndQueryService() = 
    let eventStore = MemoryDbDataStore.eventStore()
    let readModelStore = MemoryDbDataStore.readModelStore()
    let commandStatusStore = MemoryDbDataStore.commandStatusStore()
    let commandService = ApiCommandService(eventStore,readModelStore,commandStatusStore) :> IApiCommandExecutionService 
    let queryService = ApiQueryService(readModelStore) :> IApiQueryService
    commandService,queryService 

/// A command service with no existing data
let emptyCommandService() = 
    let commandService,_ = emptyCommandAndQueryService()
    commandService

/// Return a ReadModelStore, with existing sample data
let sampleReadModelStore() = 
    let eventStore = SampleDbDataStore.eventStore()
    SampleDbDataStore.readModelStore(eventStore)
    
/// Return a query service, with existing sample data
let sampleQueryService() = 
    let eventStore = SampleDbDataStore.eventStore()
    let readModelStore = SampleDbDataStore.readModelStore(eventStore)
    ApiQueryService(readModelStore) :> IApiQueryService

// ========================================
// Command helpers
// ========================================

let defaultId = locationA.id

let defaultCreateLocation = 
    LocationCommand.Create (defaultId,locationA.name,locationA.address,None)

let updateName newName =
    LocationCommand.UpdateName (defaultId,newName)


/// Transform a domain command into an ApiCommand
let toApiCommand domainCommand = 
    let cmdId = Guid.NewGuid()
    {
    ApiCommand.commandId = cmdId
    expectedVersion = ExpectedVersion.Any 
    domainCommand = domainCommand 
    }

/// The defaultCreateLocation as a ApiCommand
let cmdCreate() = 
    defaultCreateLocation
    |> DomainCommand.Location 
    |> toApiCommand 

/// The defaultChangeProperty as a ApiCommand
let cmdChangeProperty newName = 
    updateName newName
    |> DomainCommand.Location 
    |> toApiCommand 

// ========================================
// Query helpers
// ========================================


// ========================================
// Http.Request helpers
// ========================================

/// Set the URI of HttpContext
let withUri uri (hc:HttpContext) =
    let req = { hc.request with url = System.Uri(uri)}
    { hc with request = req }

/// Set the Path of HttpContext including leading slash (hardcoded to example.com)
let withPath path (hc:HttpContext) =
    let uri = sprintf "http://example.com%s" path 
    hc |> withUri uri 

/// Add a query to a HttpContext
let withQuery queryStr (hc:HttpContext) =
    let req = { hc.request with rawQuery = queryStr }
    { hc with request = req }

/// Add context to a HttpContext
let withJsonContent (jsonStr:string) (hc:HttpContext) =
    let req = { hc.request with rawForm = System.Text.Encoding.UTF8.GetBytes(jsonStr) }
    { hc with request = req }

/// Transform a HttpContext into a GET
let asGetRequest hc =
    let req = { hc.request with ``method`` = HttpMethod.GET }
    { hc with request = req }

/// Transform a HttpContext into a POST
let asPostRequest hc =
    let req = { hc.request with ``method`` = HttpMethod.POST }
    { hc with request = req }

/// Given a JSON string and path, create a POST request from it
let createPostRequest uriPath jsonContent =
    HttpContext.empty
    |> withPath uriPath 
    |> withJsonContent jsonContent
    |> asPostRequest

/// Given a URI Path, create a GET request from it
let createGetRequest uriPath =
    HttpContext.empty
    |> withPath uriPath 
    |> asGetRequest

// ========================================
// Http.Response helpers
// ========================================

/// Get the content of the response
let contentAsString (hc:HttpContext) = 
    match hc.response.content with
    | Bytes b -> b |> System.Text.Encoding.UTF8.GetString
    | NullContent -> ""
    | SocketTask _ -> failwith "not expecting socket"

// ========================================
// WebPart helpers
// ========================================

/// Execute a part with the given HttpContext
let executeWebPart (wp:WebPart) (hc:HttpContext) = 
    wp hc
    |> Async.RunSynchronously

// ========================================
// Expectations
// ========================================

let expectHttpStatus status label (httpContext:HttpContext) =
    Expect.equal httpContext.response.status status (sprintf "%i %s" status.code label)

let expect200 =
    expectHttpStatus HTTP_200.status 

let expect202 =
    expectHttpStatus HTTP_202.status 

let expect400 =
    expectHttpStatus HTTP_400.status 

let expect403 =
    expectHttpStatus HTTP_403.status 

let expect404 =
    expectHttpStatus HTTP_404.status 

let expect412 =
    expectHttpStatus HTTP_412.status 

let expect500 =
    expectHttpStatus HTTP_500.status 

let expectContentLike anObj label (httpContext:HttpContext) =
    let actual = contentAsString httpContext 
    let expected = ApiSerialization.serialize anObj 
    Expect.equal actual expected label

let expectContentContains keyword label (httpContext:HttpContext) =
    let actual = contentAsString httpContext 
    Expect.stringContains actual keyword label

/// Given a HttpContext option, check that it meets expectations
let checkExpectation label expect httpContextOpt = 
    match httpContextOpt with
    | None ->  
        failwith "Expected httpContext to be present"
    | Some httpContext -> 
        expect label httpContext 
