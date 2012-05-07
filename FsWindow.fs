namespace FsWpf

open System.Windows

[<AbstractClass>]
/// Assumes there's a TypeName.xaml file in the project with build action set to EmbeddedResource
/// All the [<DefaultValue>] mutable val's will be set to the elements with the same name in XAML
/// The FsWindow will be set as the data context of the Window
type FsWindow() = 
    inherit FsUiObject<Window>()