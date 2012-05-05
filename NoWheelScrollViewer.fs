namespace FsWpf

open System.Windows.Controls

type NoWheelScrollViewer()  =    
    inherit ScrollViewer()
    override x.OnMouseWheel(e) = ()
