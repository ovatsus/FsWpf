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

    let canExecuteChangedEvent = new Event<_, _>()

    let convert (param : obj) = 
        if param = null then 
            Unchecked.defaultof<'CommandParameterType>
        else 
            param :?> 'CommandParameterType

    member x.TriggerCanExecuteChanged() = 
        canExecuteChangedEvent.Trigger(x, EventArgs.Empty)

    override x.ProvideValue serviceProvider = 

        { new ICommand with   
            [<CLIEvent>] member x.CanExecuteChanged = canExecuteChangedEvent.Publish        
            member x.CanExecute param = this.CanExecute (convert param)
            member x.Execute param = this.Execute (convert param)
        } :> obj

    abstract member CanExecute : 'CommandParameterType -> bool
    default x.CanExecute param = not (obj.ReferenceEquals(param, null))

    abstract member Execute : 'CommandParameterType -> unit

[<AbstractClass>]
type CommandMarkupExtension<'CommandParameterType, 'RootObjectDataContextType>() as this =

    inherit MarkupExtension()

    let mutable rootObjectDataContext = None
    let canExecuteChangedEvent = new Event<_, _>()   

    let convert (param : obj) = 
        if param = null then 
            Unchecked.defaultof<'CommandParameterType>
        else 
            param :?> 'CommandParameterType

    member x.TriggerCanExecuteChanged() = 
        canExecuteChangedEvent.Trigger(x, EventArgs.Empty)

    override x.ProvideValue serviceProvider = 

        let getRoot() = 
            if rootObjectDataContext.IsNone then
                let rootObjectProvider = serviceProvider.GetService(typeof<IRootObjectProvider>) :?> IRootObjectProvider                
                let rootObject = rootObjectProvider.RootObject :?> FrameworkElement
                rootObjectDataContext <- rootObject.DataContext :?> 'RootObjectDataContextType |> Some
            rootObjectDataContext.Value
 
        { new ICommand with   
            [<CLIEvent>] member x.CanExecuteChanged = canExecuteChangedEvent.Publish        
            member x.CanExecute param = this.CanExecute (convert param) (getRoot())
            member x.Execute param = this.Execute (convert param) (getRoot())
        } :> obj

    abstract member CanExecute : 'CommandParameterType -> 'RootObjectDataContextType -> bool
    default x.CanExecute param _ = not (obj.ReferenceEquals(param, null))

    abstract member Execute : 'CommandParameterType -> 'RootObjectDataContextType -> unit
