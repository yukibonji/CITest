namespace XLCatlin.DataLab.XCBRA.GraphQL

// Web/Suave support for GraphQL query processing 

open GraphQL.Types
open GraphQL
open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.ReadModelStore

// ===========================================
// Suave support
// ===========================================

module WebParts =
    open GraphQL.Http
    open System.Collections.Generic
    open Suave                 // always open suave
    open Suave.Operators       // for >=>

    /// The type used for JSON queries
    type GraphQLWebRequest = {
        Query : string
        OperationName : string
        Variables : System.Collections.Generic.IDictionary<string, obj>
        }

    let processWebRequest (readModelStore:IReadModelStore) (webRequest:GraphQLWebRequest) =
        let query = webRequest.Query
        let nullToEmpty = function null -> "" | s -> s
        let operationName = webRequest.OperationName |> nullToEmpty
        let variables = webRequest.Variables 
        let inputs = Inputs(variables)
        QueryService.executeQuery readModelStore operationName inputs query 

    let processWebRequestString readModelStore jsonString =
        let webRequest = ApiSerialization.deserialize<GraphQLWebRequest>(jsonString)
        processWebRequest readModelStore webRequest

    // example jsonString
    (*
    let jsonString =
        """{
         "query": "query DroidQuery($droidId: String) { droid(id: $droidId) { id name } }",
         "variables": {
           "droidId": "1",
           "droidId2": 1
         }
        }"""
    *)


    let graphQL readModelStore webRequest =
        async {
            let! result = processWebRequest readModelStore webRequest
            let json = DocumentWriter(indent=true).Write(result)
            return json
            } |> Async.RunSynchronously //TODO: Support async

    let get readModelStore :WebPart = 
        // for GET, expect a "query" param and a "variables" param
        // See http://graphql.org/learn/serving-over-http/
        request (fun r ->
            let queryStr = 
                match r.queryParam "query" with
                | Choice1Of2 query -> query 
                | Choice2Of2 _ -> ""
            let variables = 
                match r.queryParam "variables" with
                | Choice1Of2 variables -> Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,obj>>(variables)
                | Choice2Of2 _ -> Dictionary<string,obj>()

            let graphQLWebRequest : GraphQLWebRequest = {
                Query = queryStr
                OperationName = null
                Variables = variables
                }

            graphQLWebRequest 
            |> graphQL readModelStore 
            |> Successful.OK
            >=> Writers.setMimeType "application/json")

    let post readModelStore :WebPart = 
        // for POST, expect a body matching Savanna.GraphQL.Service.GraphQLWebRequest
        // See http://graphql.org/learn/serving-over-http/
        (*
    {
        "query": "...",
        "operationName": "...",
        "variables": { "myVariable": "someValue", ... }
    }
        *)
        request(fun r ->
            r.rawForm 
            |> System.Text.Encoding.UTF8.GetString
            |> (fun s -> ApiSerialization.deserialize<GraphQLWebRequest>(s))
            |> graphQL readModelStore
            |> Successful.OK
            >=> Writers.setMimeType "application/json")

