module FsWpf.ICommand

open System
open System.Windows.Input

let create canExec exec =    
    
    let cecEvent = new Event<_, _>()    

    let convert (param : obj) = 
        if param = null then 
            Unchecked.defaultof<'CommandParameterType>
        else 
            param :?> 'CommandParameterType

    let command = 
        { new ICommand with   
            [<CLIEvent>] member x.CanExecuteChanged = cecEvent.Publish        
            member x.CanExecute param = canExec (convert param)
            member x.Execute param = exec (convert param)
        }

    let triggerCanExecuteChanged() = cecEvent.Trigger(command, EventArgs.Empty)

    command, triggerCanExecuteChanged
