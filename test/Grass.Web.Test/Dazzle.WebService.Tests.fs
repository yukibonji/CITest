module Dazzle.WebService.Tests

open System
open Expecto
open Suave
open WebTestUtilities
open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.MemoryDb
open XLCatlin.DataLab.XCBRA.Dazzle


// set very short for testing
let defaultDuration = 100<Ms>

let newService() =
    MockDazzle.setDuration defaultDuration 
    let readModelStore = newReadModelStore() 
    ApiDazzleService(readModelStore) :> IApiDazzleService    

let newCmd() = {
    // make the id unique
    DazzleStartCommand.Id = DazzleCalculationId <| Guid.NewGuid().ToString()
    Input = ""
    }

let isInProcess status  = 
    match status with
    | InProcess _ -> true
    | Completed _ -> false

let isCompleted status  = 
    status |> isInProcess |> not

let queryRequest = 
    "" |> createGetRequest // path not used when testing the web part directly


[<Tests>]
let allTests = 
    testList "Dazzle.WebService.Tests" [

        testCase "when Start calculation, expect status is InProcess" <| fun () ->
            let service = newService() 
            let cmd = newCmd()

            let postRequest = 
                cmd |> ApiSerialization.serialize |> createPostRequest "/"

            postRequest
            |> executeWebPart (Dazzle.WebParts.start service)
            |> checkExpectation "start" expect200 

        //testCase "when Start calculation is malformed, expect 400" <| fun () ->            
        //    let service = newService() 

        //    let jsonRequest = cmd |> ApiSerialization.serialize 

        //    // make it malformed
        //    let malformedJsonRequest = jsonRequest.Replace("Input","InpXXX") 
        //    let postRequest = 
        //        malformedJsonRequest |> createPostRequest "/"
            
        //    postRequest 
        //    |> executeWebPart (Dazzle.WebParts.start service)
        //    |> checkExpectation "Start malformed" expect400 

        testCase "when Start, expect status is InProcess" <| fun () ->
            let service = newService() 
            let cmd = newCmd()

            let postRequest = 
                cmd |> ApiSerialization.serialize |> createPostRequest "/"

            let result1 = 
                postRequest
                |> executeWebPart (Dazzle.WebParts.start service)

            result1 |> checkExpectation "start" expect200 
            result1 |> checkExpectation "status returned from Start" (expectContentContains "InProcess")

            
        testCase "when check Status after creation, expect status is InProcess" <| fun () ->
            let service = newService() 
            let cmd = newCmd()

            let postRequest = 
                cmd |> ApiSerialization.serialize |> createPostRequest "/"

            let result1 = 
                postRequest
                |> executeWebPart (Dazzle.WebParts.start service)
            result1 |> checkExpectation "start" expect200 
            result1 |> checkExpectation "status returned from Start" (expectContentContains "InProcess")

            queryRequest
            |> executeWebPart (Dazzle.WebParts.getStatus service cmd.Id.Value)
            |> checkExpectation "status returned from GetStatus" (expectContentContains "InProcess")

        testCase "when check Status of missing id, expect Error" <| fun () ->
            let service = newService() 
            let badId = DazzleCalculationId "bad"

            queryRequest
            |> executeWebPart (Dazzle.WebParts.getStatus service badId.Value)
            |> checkExpectation "status for missing calculation" expect400

        //TODO works in serial but not in parallel
        testCase "when check Status after duration, expect status is Completed" <| fun () ->
            let service = newService() 
            let cmd = newCmd()

            let postRequest = 
                cmd |> ApiSerialization.serialize |> createPostRequest "/"

            let result1 = 
                postRequest
                |> executeWebPart (Dazzle.WebParts.start service)
            result1 |> checkExpectation "start" expect200 
            result1 |> checkExpectation "status returned from Start" (expectContentContains "InProcess")

            // wait 3x as long
            System.Threading.Thread.Sleep (defaultDuration * 3<_>) 

            queryRequest
            |> executeWebPart (Dazzle.WebParts.getStatus service cmd.Id.Value)
            |> checkExpectation "status after completed" (expectContentContains "Completed")


    // END testlist
    ]
