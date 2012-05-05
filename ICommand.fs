module FsWpf.ICommand

open System
open System.Windows.Input

let createCommand canExec exec =    
    
    let canExecuteChangedEvent = new Event<_, _>()    

    let command = 
        { new ICommand with   
            [<CLIEvent>] member x.CanExecuteChanged = canExecuteChangedEvent.Publish        
            member x.CanExecute param = canExec (Utilities.castIfNotNull param)
            member x.Execute param = exec (Utilities.castIfNotNull param)
        }

    let triggerCanExecuteChanged() = 
        canExecuteChangedEvent.Trigger(command, EventArgs.Empty)

    command, triggerCanExecuteChanged
