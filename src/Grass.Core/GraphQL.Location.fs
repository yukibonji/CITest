namespace XLCatlin.DataLab.XCBRA.GraphQL

// ==========================================
// GraphQL implementations for the Locations in the domain model
// ==========================================

open GraphQL.Types
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.ReadModelStore

type LocationType() as this = 
    inherit ObjectGraphType<Location>()

    // all items by default
    let noPaging = {ApiPaging.offset=0; fetchNext=10000}

    do
        this.Name <- "LocationType"
        this.Field("Id",expression=(fun x -> x.id.Value)).Description("Id") |> ignore 
        this.Field("Name",expression=(fun x -> x.name)).Description("Name") |> ignore

        // This is the correct way to do sub-objects
        this.Field<AddressType>("Address", description="Address", resolve=(fun context -> context.Source.address :> obj)) |> ignore
        
        // The normal way used for Id and Name above doesn't work for sub-objects.
        //this.Field("Address",expression=(fun x -> x.Address)).Description("Address") |> ignore

        // This way doesn't work for sub-objects either...
        //this.Field<ObjectGraphType<AddressType>>("Address", description="Address",  resolve = (fun context -> context.Source.Address :> obj)) |> ignore

        // ... but it does work for lists
        this.Field<ListGraphType<BuildingSummaryType>>(
           "Buildings", 
           description="Buildings",  
           resolve = (fun context -> 
            //let locationId = context.Source.id
            let readModelStore = context.UserContext :?> IReadModelStore
            let filter = 
                { locationId = None
                ; name=None
                ; occupancy = []
                }

            let result = readModelStore.GetBuildingsAtLocation(filter,noPaging) 
            match result with
            | Ok summaries -> summaries :> obj
            | Error err -> failwithf "%A" err
            )) |> ignore

        // An example of a synthetic field -- a list from a object
        this.Field<ListGraphType<AddressType>>("Addresses", description="Addresses",  resolve = (fun context -> [context.Source.address] :> obj)) |> ignore

