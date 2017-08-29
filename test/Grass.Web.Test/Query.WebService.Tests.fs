module Query.WebService.Tests

open System
open Expecto
open Suave
open WebTestUtilities
open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.Command
open XLCatlin.DataLab.XCBRA.Query
open XLCatlin.DataLab.XCBRA.MemoryDb.SampleData
open UnitTestData

let queryRequest = 
    "" |> createGetRequest // path not used when testing the web part directly


[<Tests>]
let locationById = 

    testList "Query.WebService.Tests/LocationById" [
    
        testCase "when location exists, expect QueryLocationById to return same value" <| fun () ->
            let queryService = UnitTestData.sampleQueryService() 

            // query for the result
            let result =
                queryRequest
                |> executeWebPart (Query.WebParts.queryLocationById queryService locationA.id.Value)

            result 
            |> checkExpectation "queryLocationById status" expect200 

            result 
            |> checkExpectation "queryLocationById context" (expectContentLike locationA) 

        testCase "when location doesnt exist, expect 400" <| fun () ->
            let queryService = sampleQueryService() 

            // query for a Id that is not known
            let missingId = "ZZ"
            let result =
                queryRequest
                |> executeWebPart (Query.WebParts.queryLocationById queryService missingId )

            result 
            |> checkExpectation "queryLocationById status" expect400 
    
    ]
