namespace FsWpf

open System.Windows.Markup

[<AbstractClass>]
type CommandMarkupExtension<'CommandParameterType>() as this =

    inherit MarkupExtension()

    let mutable triggerCanExecuteChanged = (fun () -> ())
    member x.TriggerCanExecuteChanged = triggerCanExecuteChanged

    override x.ProvideValue serviceProvider = 
        let cmd, trigger = ICommand.createCommand this.CanExecute this.Execute
        triggerCanExecuteChanged <- trigger
        cmd :> obj

    abstract member CanExecute : 'CommandParameterType -> bool
    default x.CanExecute param = not (obj.ReferenceEquals(param, null))

    abstract member Execute : 'CommandParameterType -> unit

open System.Xaml
open System.Windows

[<AbstractClass>]
type CommandMarkupExtension<'CommandParameterType, 'RootObjectDataContextType>() as this =

    inherit MarkupExtension()    

    let mutable rootObjectDataContext = None

    let mutable triggerCanExecuteChanged = (fun () -> ())
    member x.TriggerCanExecuteChanged = triggerCanExecuteChanged

    override x.ProvideValue serviceProvider = 

        let getRoot() = 
            if rootObjectDataContext.IsNone then
                let rootObjectProvider = serviceProvider.GetService(typeof<IRootObjectProvider>) :?> IRootObjectProvider                
                let rootObject = rootObjectProvider.RootObject :?> FrameworkElement
                rootObjectDataContext <- rootObject.DataContext :?> 'RootObjectDataContextType |> Some
            rootObjectDataContext.Value

        let cmd, trigger = ICommand.createCommand (fun param -> this.CanExecute param (getRoot())) (fun param -> this.Execute param (getRoot()))
        triggerCanExecuteChanged <- trigger
        cmd :> obj

    abstract member CanExecute : 'CommandParameterType -> 'RootObjectDataContextType -> bool
    default x.CanExecute param _ = not (obj.ReferenceEquals(param, null))

    abstract member Execute : 'CommandParameterType -> 'RootObjectDataContextType -> unit
