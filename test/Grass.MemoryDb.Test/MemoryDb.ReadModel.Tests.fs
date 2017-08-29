module MemoryDb.ReadModel.Tests

(*
Check that the in-memory implementation of the ReadModelStore works as expected

//TODO: Make this into a check of anything that implements IReadModelStore
//TODO: add property based tests
*)

open XLCatlin.DataLab.XCBRA.ReadModelStore 
open XLCatlin.DataLab.XCBRA.MemoryDb.SampleData
open UnitTestData

open Expecto

// ==========================================
// Test Location
// ==========================================

let expectNotFound result =
    result |> expectErrorWithValue (fun err -> 
        match err with
        | NotFound _ -> asExpected()
        | _ -> failwith "Expected NotFound"
        )  
    
[<Tests>]   
let location = 
    testList "MemoryDb.ReadModel.Tests/Location" [
    
        testCase "when location doesnt exist, expect GetLocation=None" <| fun () ->
            let store = newReadModelStore()
            let result = store.GetLocationById locationIdA
            result |> expectNotFound 
            
        testCase "when location does exist, expect GetLocation=Some" <| fun () ->
            let store = newReadModelStore()
            let _ = store.UpdateLocation locationA
            let result = store.GetLocationById locationIdA
            result |> expectOkWithValue (fun v -> Expect.equal v locationA "")

        testCase "when location updated, expect GetLocation=newest version" <| fun () ->
            let store = newReadModelStore()
            let _ = store.UpdateLocation locationA
            let updatedLocation = {locationB with id=locationIdA}
            let _ = store.UpdateLocation updatedLocation
            let result = store.GetLocationById locationIdA
            result |> expectOkWithValue (fun v -> Expect.equal v updatedLocation "expect locationB")
    ]


