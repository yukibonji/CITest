/// Helpers for web and Suave
module WebUtilities

open Suave

// get a parameter, or None if not found
let tryGetParam (r:HttpRequest) key = 
    match r.queryParam key with
    | Choice1Of2 value -> Some value
    | _ -> None


// get a header, or None if not found
let tryGetHeader (r:HttpRequest) key = 
    match r.header key with
    | Choice1Of2 value -> Some value
    | _ -> None

/// Set the default value for an Option
let ifNone defaultValue opt = 
    defaultArg opt defaultValue 

