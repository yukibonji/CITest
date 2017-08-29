module GraphQL.Location.Tests

open Expecto
open GraphQL.TestingUtilities

// =====================
// Tests to check the GraphQL Location queries work.
//
// These tests talk directly to the service, bypassing Suave.
// The tests use mocks and dummy data, so there is no I/O.
//
// For tests that go through Suave, see GraphQL.WebService.Tests in a different project
// =====================

// ==========================================
// locationAFound
// ==========================================

[<Tests>]
let locationTests =
    ptestList "GraphQL.Location.Tests" [
    
        testCase "locationAFound" <| fun () ->
            let query = """
query {
    location(id:"A") {
        id
        name
        buildings {
            id
            name
        }
    }
}
"""

(*
EXPECTED
{
    "data": {
    "location": {
        "id": "A",
        "name": "LocationA",
        "buildings": [
        {
            "id": 101,
            "name": "BuildingA101"
        },
        {
            "id": 102,
            "name": "BuildingA102"
        }
        ]
    }
    }
}
*)

            let result = queryAsJsonObject query
            let location  = 
                result.["data"].["location"] 

            let actual1 = 
                string location.["id"],string location.["name"]
            
            Expect.equal actual1 ("A","LocationA") "idName"  

            let actual2 = 
                location.["buildings"]
                |> Seq.map (fun b -> string b.["id"], string b.["name"] )
                |> Seq.toList

            let expected2 = 
                [
                ("101","BuildingA101")
                ("102","BuildingA102")
                ]
            Expect.equal actual2 expected2 "buildings"              

// ==========================================
// locationAWithAddress
// ==========================================
        testCase "locationAWithAddress" <| fun () ->
            let query = """
query {
    location(id:"A") {
        id
        name
        address {
            postalCountry
            postalCode
        }
    }
}"""

(*
EXPECTED
{
    "data": {
    "location": {
        "id": "A",
        "name": "LocationA",
        "address": {
        "postalCountry": "GB",
        "postalCode": "NW1"
        }
    }
    }
}
*)
 
            let result = queryAsJsonObject query
            let location  = 
                result.["data"].["location"] 

            let actual1 = 
                string location.["id"],string location.["name"]
            
            Expect.equal actual1 ("A","LocationA") "idName"  

            let actual2 = 
                let address = location.["address"]
                string address.["postalCountry"], string address.["postalCode"]

            Expect.equal actual2 ("GB","NW1") "address"              

// ==========================================
// locationAWithSyntheticAddresses
// ==========================================

        testCase "locationAWithSyntheticAddresses " <| fun () ->
            let query = """
query {
    location(id:"A") {
        id
        name
        addresses {
            postalCountry
            postalCode
        }
    }
}
"""
(*
EXPECTED
{
    "data": {
    "location": {
        "id": "A",
        "name": "LocationA",
        "addresses": [
        {
            "postalCountry": "GB",
            "postalCode": "NW1"
        }
        ]
    }
    }
}
*)

            let result = queryAsJsonObject query
            let location  = 
                result.["data"].["location"] 

            let actual1 = 
                string location.["id"],string location.["name"]
            
            Expect.equal actual1 ("A","LocationA") "idName"  

            let actual2 = 
                location.["addresses"]
                |> Seq.map (fun a -> string a.["postalCountry"], string a.["postalCode"])
                |> Seq.toList

            Expect.equal actual2 [("GB","NW1")] "addresses"              

// ==========================================
// locationNotFound
// ==========================================

        testCase "locationNotFound " <| fun () ->
            let query = """
query {
    location(id:"C") {
        id
        name
    }
}
"""
            let json = queryAsJsonObject query
            json |> expectHasErrorMessages
            json |> expectErrorContaining "Not Found"

// ==========================================
// aliases
// ==========================================

        testCase "aliases " <| fun () ->
            let query = """
query {
    locationA:location(id:"A") {
        id
    }
    locationB:location(id:"B") {
        id
    }
}
"""

(*
EXPECTED
{
    "data": {
    "locationA": {
        "id": "A"
    },
    "locationB": {
        "id": "B"
    }
    }
}
*)

            let result = queryAsJsonObject query
            let locationA  = 
                result.["data"].["locationA"].["id"] |> string
            let locationB  = 
                result.["data"].["locationB"].["id"] |> string

            
            Expect.equal locationA "A" "locationA"  
            Expect.equal locationB "B" "locationB"  

// ==========================================
// withParams
// ==========================================

        testCase "withParams" <| fun () ->
            // Note: Must used "String!" instead of "String" for some reason
            let query = """
query ($locationId: String!) {
    location(id:$locationId) {
        id
    }
}
"""

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

            let inputs = """{ "locationId":"A" }"""

            let result = queryAsJsonObjectWithInputs inputs query
            let locationA  = 
                result.["data"].["location"].["id"] |> string

            
            Expect.equal locationA "A" "locationA"  

// ==========================================
// webQuery - test the full webquery
// ==========================================

(*
        testCase "webQuery " <| fun () ->
            // Note: Must use "String!" instead of "String" for some reason
            let query = """
query ($locationId: String!) {
    location(id:$locationId) {
        id
    }
}
"""
            let jsonString = 
                sprintf """{
                 "query": "%s",
                 "variables": {
                   "locationId": "B",
                 }
                }""" query

            let expected = """{
    "data": {
    "location": {
        "id": "B"
    }
    }
}"""

            let expected = expected |> toPlatformNewLine
            let actual = webQuery jsonString
            Expect.equal expected actual ""
*)



// ==========================================
// metadata
// ==========================================

        // note use GraphQL type: "LocationType" not domain type "Location"
        testCase "metadata" <| fun () ->
            let query = """
{
    __type(name: "LocationType") {
    name
    kind
    fields {
        name
        type {
        name
        kind
        }
    }
    }
}
    """

(*
EXPECTED
{
    "data": {
    "__type": {
        "name": "LocationType",
        "kind": "OBJECT",
        "fields": [
        {
            "name": "id",
            "type": {
            "name": null,
            "kind": "NON_NULL"
            }
        },
        {
            "name": "name",
            "type": {
            "name": null,
            "kind": "NON_NULL"
            }
        },
        {
            "name": "address",
            "type": {
            "name": "AddressType",
            "kind": "OBJECT"
            }
        },
        {
            "name": "buildings",
            "type": {
            "name": null,
            "kind": "LIST"
            }
        },
        {
            "name": "addresses",
            "type": {
            "name": null,
            "kind": "LIST"
            }
        }
        ]
    }
    }
}
*)

            let result = queryAsJsonObject query
            let mtype = result.["data"].["__type"]

            let actual1 = 
                string mtype.["name"],string mtype.["kind"]
            
            Expect.equal actual1 ("LocationType","OBJECT") "nameAndKind"  

            let actual2 = 
                mtype.["fields"]
                |> Seq.map (fun f -> string f.["name"],string f.["type"].["name"],string f.["type"].["kind"])
                |> Seq.toList

            let expected2 = 
                [
                ("id","","NON_NULL")
                ("name","","NON_NULL")
                ("address","AddressType","OBJECT")
                ("buildings","","LIST")
                ("addresses","","LIST")
                ]

            Expect.equal actual2 expected2 "fields"  

    // END test list
    ]