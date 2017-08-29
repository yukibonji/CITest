(*
This is an example of using the C# GraphQL.NET library
Taken from https://github.com/graphql-dotnet/graphql-dotnet and ported to F#
*)

System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
#r "../../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "../../packages/GraphQL/lib/net45/GraphQL.dll"
#r "../../packages/GraphQL-Parser/lib/net45/GraphQL-Parser.dll"

open System

module EventSourcing = 
    type Event<'id,'data> = {
        Id : 'id
        Data : 'data
        User : string
        Timestamp : DateTime
        // etc
        }

    type LocationId = LocationId of string
    type BuildingId = BuildingId of string

    type LocationBuildingAdded = {
        LocationId : LocationId
        BuildingId : BuildingId
        }

    /// A building was removed from a Location 
    type LocationBuildingRemoved = {
        LocationId : LocationId
        BuildingId: BuildingId
        }

    type LocationEvent =
        | BuildingAdded of LocationBuildingAdded
        | BuildingRemoved of LocationBuildingRemoved

    let event =  BuildingAdded {LocationId=LocationId "A"; BuildingId=BuildingId "B"}
    let dto = {Id=1; Data=event; User="me"; Timestamp=DateTime.Now}

    open Newtonsoft.Json
    let jsonString = JsonConvert.SerializeObject(dto)
    printfn "%s" jsonString 
    let dto2 = JsonConvert.DeserializeObject<Event<int,LocationEvent>>(jsonString)
    let dto3 = JsonConvert.DeserializeObject<Event<int,LocationEvent>>("aa")
    printfn "%A" dto2 


    let locId : obj = LocationId "A" :> obj
    let buildId : obj = BuildingId "A" :> obj
    locId = buildId 
    locId = locId

    let dict = System.Collections.Generic.Dictionary<obj,obj>()
    dict.[locId] <- locId
    dict.[buildId] <- buildId
    let get<'t> k = dict.[k] :?> 't
    let locId2 = get<LocationId> locId
    let locId3 = get<LocationId> buildId

open System
open System.Threading.Tasks
open GraphQL
open GraphQL.Http
open GraphQL.Types
open Newtonsoft.Json


/// Helpers to convert F# functions to Func and Linq.Expressions
/// see https://stackoverflow.com/a/42858641/1136133
[<AutoOpen>]
module FuncHelper =
    open System.Linq.Expressions
    type FunAs() =
        static member LinqExpression<'T, 'TResult>(e: Expression<Func<'T, 'TResult>>) = e
        static member Func<'T, 'TResult>(e: Func<'T, 'TResult>) = e

// ==========================================
// Domain types
// ==========================================
type Droid = {
    Id : string
    Name : string
    }

// ==========================================
// GraphQL types
// ==========================================

type DroidType() as this = 
    inherit ObjectGraphType<Droid>()
    do
        this.Field(fun x -> x.Id).Description("The Id of the Droid.") |> ignore 
        this.Field(fun x -> x.Name, true).Description("The name of the Droid.") |> ignore
    

type StarWarsQuery() as this = 
    inherit ObjectGraphType()
    do
        let f = FunAs.Func (fun context -> { Droid.Id = "1"; Name = "R2-D2" } :> obj )
        this.Field<DroidType>("hero", resolve=f) |> ignore

// ==========================================
// GraphQL server
// ==========================================

let starWarsSchema = new Schema(Query = StarWarsQuery())
//starWarsSchema.RegisterTypes(typeof<DroidType>) 
starWarsSchema.ResolveType <- (
    fun typ -> 
        match typ.Name with
        | "DroidType" -> DroidType() 
        | name -> 
            let msg = sprintf "Type '%s' not known" name
            printfn "%s" msg
            failwith msg
        :> IGraphType
    )

let processQuery schema query =
    let executer = DocumentExecuter()
    executer.ExecuteAsync( 
        fun options -> 
            options.Schema <- schema
            options.Query <- query
        )
    |> Async.AwaitTask




// ==========================================
// GraphQL client
// ==========================================

let query = """
query {
  hero {
    id
    name
    }
  }
"""

let result = processQuery starWarsSchema query |> Async.RunSynchronously
result.Errors

async {
    let! result = processQuery starWarsSchema query
    let json = DocumentWriter(indent=true).Write(result)
    return json
    }
|> Async.RunSynchronously





//module Test =
//    let mutable remainingMs = 10000
//    let loopDuration = 1000 //  lasting 1 second 
//    let doCalculation = 
//        async {
//            // in process
//            while remainingMs > 0<_> do
//                do! Async.Sleep (loopDuration * 1<_>)
//                System.Console.WriteLine("{0} remaining {1}",id,remainingMs)
//                remainingMs <- (remainingMs - loopDuration)
//
//            // completed
//            System.Console.WriteLine("Completed")
//            remainingMs <- 0<_>
//            } 
//        
//
//    // start now
//    doCalculation |> Async.Start
//
//
//    let mutable i = 10
//    let sleepWorkflow  = async{
//        printfn "Starting sleep workflow at %O" DateTime.Now.TimeOfDay
//        while i> 0 do
//            do! Async.Sleep 1000
//            printfn "Count" 
//            i <- i - 1
//        printfn "Finished sleep workflow at %O" DateTime.Now.TimeOfDay
//        }
//
//    Async.RunSynchronously sleepWorkflow  
//    sleepWorkflow  |> Async.Start