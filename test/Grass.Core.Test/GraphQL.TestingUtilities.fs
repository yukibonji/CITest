/// Helper functions for testing GraphQL
module GraphQL.TestingUtilities

open Expecto
open GraphQL
open GraphQL.Http
open Newtonsoft.Json.Linq

open System
open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.GraphQL

let eventStore = MemoryDb.SampleData.eventStore() 
let readModelStore = MemoryDb.SampleData.readModelStore(eventStore) 

/// Does a query and returns the result as a string
let queryAsStringWithInputs (inputsJson:string) query = 
    async {
        let inputs = inputsJson.ToInputs()
        let operationName = ""
        let! result = GraphQL.QueryService.executeQuery readModelStore operationName inputs query
        let json = DocumentWriter(indent=true).Write(result)
        return json
        }
    |> Async.RunSynchronously

/// Does a query and returns the result as a string
let queryAsString query = 
    let inputsJson = ""
    queryAsStringWithInputs inputsJson query 

/// Does a query and returns the result as a Newtonsoft JObject
let queryAsJsonObject query = 
    let str = queryAsString query 
    JObject.Parse(str)

let queryAsJsonObjectWithInputs inputsJson query = 
    let str = queryAsStringWithInputs inputsJson query 
    JObject.Parse(str)

(*
let webQuery jsonString =
    async {
        let! result = GraphQL.WebService.processWebRequestString jsonString 
        let json = DocumentWriter(indent=true).Write(result)
        return json
        }
    |> Async.RunSynchronously
*)

let expectHasErrorMessages (jsonObject:JObject) = 
    Expect.isFalse (jsonObject.["errors"] |> Seq.isEmpty) "has errors"

let errorMessages (jsonObject:JObject) = 
    jsonObject.["errors"] |> Seq.map (fun o -> o.["message"].ToString())

let expectErrorContaining phrase jsonObject = 
    let messages = errorMessages jsonObject
    let context = sprintf "Error message contains '%s'" phrase
    Expect.exists messages (fun msg -> msg.Contains(phrase)) context 
