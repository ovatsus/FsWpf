module FsWpf.ICommand

open System
open System.Windows.Input

let create canExec exec =    
    let cecEvent = new DelegateEvent<EventHandler>()    
    let command = 
        { new ICommand with   
            [<CLIEvent>] member x.CanExecuteChanged = cecEvent.Publish        
            member x.CanExecute param = canExec (if param = null then None else Some (param :?> 'CommandParameterType))
            member x.Execute param = exec (if param = null then None else Some (param :?> 'CommandParameterType))
        }
    let triggerCanExecuteChanged() = cecEvent.Trigger ([| command; EventArgs.Empty |])
    command, triggerCanExecuteChanged
