namespace FsWpf

open System.Windows

[<AbstractClass>]
/// Usage: FsWindow(__SOURCE_FILE__.Replace(".fs", ".xaml"))
/// The FsWindow will be set as the data context of the Window
/// All the [<DefaultValue>] mutable val's will be set to the elements with the same name in XAML
type FsWindow(xamlPath) = 
    inherit FsUiObject<Window>(xamlPath)