module FsWpf.Utilities

open System

let castIfNotNull (value : obj) = 
    if obj.ReferenceEquals(value, null) then
        Unchecked.defaultof<'Type>
    else 
        value :?> 'Type

let applyIfNotNull func value = 
    if obj.ReferenceEquals(value, null) then 
        Unchecked.defaultof<'Type>
    else 
        func value

let parseEnum str =
    Enum.Parse(typeof<'EnumType>, str) :?> 'EnumType