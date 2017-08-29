/// Miscellaneous utility functions
module Savanna.Utilities

open System

/// Helpers to convert F# functions to Func and Linq.Expressions
/// see https://stackoverflow.com/a/42858641/1136133
[<AutoOpen>]
module FuncHelper =
    open System.Linq.Expressions
    type FunAs() =
        static member LinqExpression<'T, 'TResult>(e: Expression<Func<'T, 'TResult>>) = e
        static member Func<'T, 'TResult>(e: Func<'T, 'TResult>) = e

/// Set the default value for an Option
let ifNone defaultValue opt = 
    defaultArg opt defaultValue 
