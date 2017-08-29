module Command.WebService.Tests

open System
open Expecto
open Suave
open WebTestUtilities
open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.Command
open XLCatlin.DataLab.XCBRA.Query
open XLCatlin.DataLab.XCBRA.MemoryDb.SampleData


[<Tests>]
let allTests = 
    testList "Command.WebService.Tests" [
    
        testCase "when createLocation succeeds, expect 200" <| fun () ->
            let service = emptyCommandService() 
            let postRequest = 
                cmdCreate() |> ApiSerialization.serialize |> createPostRequest "/"
            
            postRequest
            |> executeWebPart (Command.WebParts.executeCommand service)
            |> checkExpectation "createLocation" expect200 
    
        testCase "when createLocation is malformed, expect 400" <| fun () ->
            let service = emptyCommandService() 
            let jsonRequest = cmdCreate() |> ApiSerialization.serialize 
            // make it malformed
            let malformedJsonRequest = jsonRequest.Replace("Any","XXX") 
            let postRequest = 
                malformedJsonRequest |> createPostRequest "/"
            
            postRequest 
            |> executeWebPart (Command.WebParts.executeCommand service)
            |> checkExpectation "createLocation malformed" expect400 

        testCase "when createLocation fails, expect 400" <| fun () ->
            let service = emptyCommandService() 
            let postRequest = 
                cmdCreate() |> ApiSerialization.serialize |> createPostRequest "/"
            
            postRequest 
            |> executeWebPart (Command.WebParts.executeCommand service)
            |> checkExpectation "createLocation first time" expect200 

            // do it again - expect an error now because the record already exists
            postRequest 
            |> executeWebPart (Command.WebParts.executeCommand service)
            |> checkExpectation "createLocation second time" expect400 
    
        testCase "when createLocation followed by changeProperty, expect 200" <| fun () ->
            let service = emptyCommandService() 

            let postRequest1 = 
                cmdCreate() |> ApiSerialization.serialize |> createPostRequest "/"
            postRequest1
            |> executeWebPart (Command.WebParts.executeCommand service)
            |> checkExpectation "createLocation" expect200 

            let postRequest2 = 
                cmdChangeProperty "NewName" |> ApiSerialization.serialize |> createPostRequest "/"
            postRequest2
            |> executeWebPart (Command.WebParts.executeCommand service)
            |> checkExpectation "changeProperty" expect200 
    
    
    ]
