namespace FsWpf

open System.ComponentModel
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

[<AbstractClass>]
type ViewModelBase() =
    
    let propertyChanged = new Event<_, _>()

    let toPropName expr =
        match expr with
        | PropertyGet(a, b, list) -> b.Name
        | _ -> failwith "Unsupported: " + expr.ToString()

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member x.PropertyChanged = propertyChanged.Publish

    member x.OnPropertyChanged expr =
        propertyChanged.Trigger(x, new PropertyChangedEventArgs(toPropName expr))
