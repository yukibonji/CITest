namespace XLCatlin.DataLab.XCBRA.GraphQL

// Definition and implementation of GraphQL query processing, independent of any API

open Savanna.Utilities
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.ReadModelStore

module QueryService = 

    // ==========================================
    // Utilities
    // ==========================================

    let readModelErrorToString err = 
        match err with
        | NotFound msg -> sprintf "Not Found: %s" msg
        | MoreThanOneFound msg -> sprintf "More than one found: %s" msg

    // ==========================================
    // Main query
    // ==========================================

    open GraphQL.Types
    open GraphQL

    type SavannaQuery(readModelStore:IReadModelStore) as this = 
        inherit ObjectGraphType()
        do
            this.Name <- "SavannaQuery";

        // getWarehouseById
        do
            let arguments = QueryArguments(QueryArgument<NonNullGraphType<IntGraphType>>( Name = "id", Description = "id of the building" ))
            let resolver = FunAs.Func(fun (context:ResolveFieldContext<_>) -> 
                let id = context.GetArgument<string>("id")
                let onSuccess data = 
                    data :> obj // convert the data to a generic Obj
                let onFailure err = 
                    // convert error to a context.Error and fail
                    let msg = readModelErrorToString err
                    context.Errors.Add(ExecutionError(msg)) 
                    failwith msg
                
                readModelStore.GetWarehouseById(BuildingId id) 
                |> Result.bimap onSuccess onFailure

                )
            this.Field<WarehouseType>("Warehouse", arguments=arguments, resolve=resolver) |> ignore

        // getLocationById
        do
            let arguments = QueryArguments(QueryArgument<NonNullGraphType<StringGraphType>>( Name = "id", Description = "id of the location" ))
            let resolver = FunAs.Func(fun (context:ResolveFieldContext<_>) -> 
                let id = context.GetArgument<string>("id")
                let onSuccess data = 
                    data :> obj // convert the data to a generic Obj
                let onFailure err = 
                    // convert error to a context.Error and fail
                    let msg = readModelErrorToString err
                    context.Errors.Add(ExecutionError(msg)) 
                    failwith msg

                readModelStore.GetLocationById(LocationId id)
                |> Result.bimap onSuccess onFailure
                )
            this.Field<LocationType>("location", arguments=arguments, resolve=resolver) |> ignore

    // ==========================================
    // Schema
    // ==========================================

    let savannaSchema readModelStore = 
        let schema = new Schema(Query = SavannaQuery(readModelStore))
        // register subtypes to avoid runtime exceptions
        schema.RegisterType<AddressType>()
        schema 

    let executeQuery readModelStore operationName inputs query =
        let executer = DocumentExecuter()
        executer.ExecuteAsync( 
            fun options -> 
                options.Schema <- savannaSchema readModelStore
                options.Inputs <- inputs  // parameters for the query
                options.Query <- query
                // pass the readModelStore to the per-domain functions
                options.UserContext <- readModelStore
                options.OperationName <- operationName
            )
        |> Async.AwaitTask

