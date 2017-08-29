module Utilities

open System
open System.Collections.Generic  // for Dictionary
open XLCatlin.DataLab.XCBRA.DomainModel

/// Set the default value for an Option
let ifNone defaultValue opt = 
    defaultArg opt defaultValue 

/// If a string is null or Empty, return None
let noneIfEmpty str = 
    if String.IsNullOrWhiteSpace(str) then
        None
    else
        Some str            

let tryGetValue key (dict:IDictionary<_,_>) = 
    match dict.TryGetValue(key) with
    | true,v -> Some v
    | false, _ -> None

let toDistance (d:decimal) : Distance =
    d * 1M<Metre>

let fromDistance (d:Distance) :decimal =
    d * 1M<_>

let toArea (a:decimal) :Area =
    a * 1M<Metre^2>

let fromArea (a:Area) : decimal =
    a * 1M<_>