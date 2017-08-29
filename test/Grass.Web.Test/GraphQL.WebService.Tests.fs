module GraphQL.WebService.Tests

open Expecto
open WebTestUtilities
open Suave
open XLCatlin.DataLab.XCBRA
open Newtonsoft.Json.Linq


// ==========================================
// webPost
// ==========================================

// Note: Must use "String!" instead of "String" 
let query = """
query ($locationId: String!) {
    location(id:$locationId) {
        id
    }
}
"""

[<Tests>]
let allTests = 
    let readModelStore = sampleReadModelStore() 

    ptestList "GraphQL.WebService.Tests" [

        testCase "webPost" <| fun () ->
            let jsonString = 
                sprintf """{
                 "query": "%s",
                 "variables": {
                   "locationId": "A",
                 }
                }""" query

(*
EXPECTED
{
    "data": {
    "location": {
        "id": "A"
    }
    }
}
*)

            HttpContext.empty
            |> withUri "http://example.com"
            |> withJsonContent jsonString 
            |> asPostRequest
            |> executeWebPart (GraphQL.WebParts.post readModelStore)
            |> function
                | None ->  
                    failwith "expected success"
                | Some result -> 
                    let jsonObject = 
                        result
                        |> contentAsString
                        |> JObject.Parse
                    let actual = string jsonObject.["data"].["location"].["id"]
                    Expect.equal actual "A" "location"  

// ==========================================
// webGet
// ==========================================

        testCase "webGet" <| fun () ->

(*
EXPECTED
{
    "data": {
    "location": {
        "id": "A"
    }
    }
}
*)
            let variablesParam = """{"locationId": "A"}"""
            let querySegment = sprintf """query=%s&variables=%s""" query variablesParam 
            HttpContext.empty
            |> withUri "http://example.com"
            |> withQuery querySegment 
            |> asGetRequest
            |> executeWebPart (GraphQL.WebParts.get readModelStore)
            |> function
                | None ->  
                    failwith "expected success"
                | Some result -> 
                    let jsonObject = 
                        result
                        |> contentAsString
                        |> JObject.Parse
                    let actual = string jsonObject.["data"].["location"].["id"]
                    Expect.equal actual "A" "location"  
    ]



