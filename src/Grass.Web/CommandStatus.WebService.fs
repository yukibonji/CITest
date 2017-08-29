namespace XLCatlin.DataLab.XCBRA.CommandStatus

open System
open Suave                 // always open suave
open Suave.Operators       // for >=>
open XLCatlin.DataLab.XCBRA.CommandStatusStore
open XLCatlin.DataLab.XCBRA.DomainModel
open XLCatlin.DataLab.XCBRA

// ===========================================
// Suave support
// ===========================================

module WebParts =

    let resultToWebPart result =
        result 
        |> function
            | Ok commandStatus -> 
                commandStatus |> ApiSerialization.serialize |>  Successful.OK 
            | Error err ->
                match err with
                | CommandStatusStoreError.NotFound ->
                    RequestErrors.NOT_FOUND ""
                | CommandStatusStoreError.OtherError msg ->
                    ServerErrors.INTERNAL_ERROR msg 

    let getCommandStatus (statusStore:ICommandStatusStore) (cmdId:string) :WebPart = 
        warbler (fun _httpContext ->
            let result = statusStore.GetCommandStatus(ApiCommandId cmdId)

            // choose a webpart based on the result
            result |> resultToWebPart 
            )



