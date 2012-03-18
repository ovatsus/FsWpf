namespace FsWpf

open System
open System.Windows
open System.Windows.Data
open System.Windows.Input
open System.Windows.Markup
open System.Xaml

[<AbstractClass>]
type CommandMarkupExtension<'CommandParameterType>() as this =

    inherit MarkupExtension()

    let canExecuteChangedEvent = new DelegateEvent<EventHandler>()   

    member x.TriggerCanExecuteChanged() = 
        canExecuteChangedEvent.Trigger([| x; EventArgs.Empty |])

    override x.ProvideValue serviceProvider = 

        { new ICommand with   
            [<CLIEvent>] member x.CanExecuteChanged = canExecuteChangedEvent.Publish        
            member x.CanExecute param = this.CanExecute (if param = null then None else Some (param :?> 'CommandParameterType))
            member x.Execute param = this.Execute (if param = null then None else Some (param :?> 'CommandParameterType))
        } :> obj

    abstract member CanExecute : 'CommandParameterType option -> bool
    default x.CanExecute param = param.IsSome

    abstract member Execute : 'CommandParameterType option -> unit

[<AbstractClass>]
type CommandMarkupExtension<'CommandParameterType, 'RootObjectDataContextType>() as this =

    inherit MarkupExtension()

    let mutable rootObjectDataContext = None
    let canExecuteChangedEvent = new DelegateEvent<EventHandler>()   

    member x.TriggerCanExecuteChanged() = 
        canExecuteChangedEvent.Trigger([| x; EventArgs.Empty |])

    override x.ProvideValue serviceProvider = 

        let getRoot() = 
            if rootObjectDataContext.IsNone then
                let rootObjectProvider = serviceProvider.GetService(typeof<IRootObjectProvider>) :?> IRootObjectProvider                
                let rootObject = rootObjectProvider.RootObject :?> FrameworkElement
                rootObjectDataContext <- rootObject.DataContext :?> 'RootObjectDataContextType |> Some
            rootObjectDataContext.Value
 
        { new ICommand with   
            [<CLIEvent>] member x.CanExecuteChanged = canExecuteChangedEvent.Publish        
            member x.CanExecute param = this.CanExecute (if param = null then None else Some (param :?> 'CommandParameterType)) (getRoot())
            member x.Execute param = this.Execute (if param = null then None else Some (param :?> 'CommandParameterType)) (getRoot())
        } :> obj

    abstract member CanExecute : 'CommandParameterType option -> 'RootObjectDataContextType -> bool
    default x.CanExecute param _ = param.IsSome

    abstract member Execute : 'CommandParameterType option -> 'RootObjectDataContextType -> unit
