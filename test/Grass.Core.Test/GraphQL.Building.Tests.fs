module GraphQL.Building.Tests

open Expecto

// =====================
// Tests to check the GraphQL Building queries work.
//
// These tests talk directly to the service, bypassing Suave. 
// The tests use mocks and dummy data, so there is no I/O.
//
// For tests that go through Suave, see Integration.GraphQL
// =====================


let allTests = 
    testList "GraphQL.Building" []



