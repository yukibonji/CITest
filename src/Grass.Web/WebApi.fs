module XLCatlin.DataLab.XCBRA.WebApi

// The top-level web API

open Suave                 // always open suave
open Suave.Operators       // for >=>
open Suave.Filters         // for path 
open XLCatlin.DataLab.XCBRA
open XLCatlin.DataLab.XCBRA.Command
open XLCatlin.DataLab.XCBRA.Query
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA.EventStore
open XLCatlin.DataLab.XCBRA.ReadModelStore
open XLCatlin.DataLab.XCBRA.CommandStatusStore

let setCORSHeaders =
    Writers.addHeader  "Access-Control-Allow-Origin" "*" 
    >=> Writers.setHeader "Access-Control-Allow-Headers" "authorization" 
    >=> Writers.addHeader "Access-Control-Allow-Headers" "content-type" 
    >=> Writers.addHeader "Access-Control-Allow-Methods" "GET,POST,PUT" 

let allow_cors : WebPart =
    choose 
        [ Filters.OPTIONS >=>
            fun context ->
                context |> (
                    setCORSHeaders
                        >=> Successful.OK "CORS approved" 
                        )
    ]


let corsConf = 
    { CORS.defaultCORSConfig 
        with 
            allowedUris = CORS.InclusiveOption.All
    }


let routes (_eventStore:IEventStore,readModelStore:IReadModelStore,commandService:IApiCommandExecutionService,queryService:IApiQueryService,commandStatusStore:ICommandStatusStore,dazzleService:IApiDazzleService) : WebPart = 

    let authenticate = ApiAuth.WebParts.authenticate 

    choose [
        allow_cors

        GET >=> choose [
            path "/" >=> (Successful.OK "")

            // these routes require authentication
            authenticate <| CORS.cors corsConf >=> choose [
                path "/graphql" >=> GraphQL.WebParts.get readModelStore
               
                // more restrictive paths first
                pathScan "/location/%s/building" (fun locationId -> 
                    Query.WebParts.queryBuildingsAtLocation queryService (LocationId locationId))

                pathScan "/location/%s" (fun locationId -> 
                    Query.WebParts.queryLocationById queryService locationId)

                path "/location" 
                    >=> authenticate (Query.WebParts.queryAllLocations queryService )

                pathScan "/warehouse/%s" (fun warehouseId -> 
                    Query.WebParts.queryWarehouseById queryService warehouseId)

                pathScan "/commandstatus/%s" (fun commandId -> 
                    CommandStatus.WebParts.getCommandStatus commandStatusStore commandId )

                pathScan "/dazzle/%s" (fun calcId -> 
                    Dazzle.WebParts.getStatus dazzleService calcId )
                ]

            RequestErrors.NOT_FOUND ""
        ]

        POST >=> choose [
            // these routes require authentication
            authenticate <| CORS.cors corsConf >=> choose [
                path "/graphql" 
                    >=> GraphQL.WebParts.post readModelStore

                path "/command" 
                    >=> Command.WebParts.executeCommand commandService 

                path "/commandqueue" 
                    >=> Command.WebParts.executeCommand commandService //TODO add queue
                    >=> Suave.Writers.setStatus HTTP_202
                
                path "/dazzle" 
                    >=> Dazzle.WebParts.start dazzleService 
                ]
            RequestErrors.NOT_FOUND ""
        ]
    ]




