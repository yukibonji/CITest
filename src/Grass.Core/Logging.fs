namespace Savanna

open System

/// Simple implementation of logging - TODO replace with logary or similar
[<AutoOpen>]
module Logging =

    let rec asTuples list = 
        match list with 
        | [] -> []
        | k::v::tail -> (k,v)::asTuples tail
        | [k] -> [ (k,"") ]

    let logToConsole (level:string) (msg:string) args = 
        let sb = new Text.StringBuilder()
        let utcNow = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")
        sb.AppendFormat("{{\"Time\":\"{0}\"", utcNow) |> ignore
        sb.AppendFormat(", \"Level\":\"{0}\"", level) |> ignore
        sb.AppendFormat(", \"Msg\":\"{0}\"", msg) |> ignore
        for k,v in args |> Array.toList |> asTuples do
            sb.AppendFormat(", \"{0}\":\"{1}\"",k, v) |> ignore 
        sb.AppendLine("}") |> ignore
        let logMessage = sb.ToString()
        Console.Write(logMessage)
            


    type ILogger =
        abstract member Info: msg:string * [<ParamArray>] args:string[] -> unit
        abstract member Warn: msg:string * [<ParamArray>] args:string[] -> unit
        abstract member Error: msg:string * [<ParamArray>] args:string[] -> unit

    type ConsoleLogger() =
        interface ILogger with
            member this.Info(msg,[<ParamArray>] args) = 
                logToConsole "INFO" msg args
            member this.Warn(msg,[<ParamArray>] args) = 
                logToConsole "WARN" msg args
            member this.Error(msg,[<ParamArray>] args) = 
                logToConsole "ERROR" msg args
        static member Default = 
            ConsoleLogger() :> ILogger
            
        (*
        let logger = ConsoleLogger.Default 
        logger.Info("my message")
        logger.Info("my message", "a","1", "b","2")
        *)
            
