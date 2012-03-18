namespace FsWpf

open System.Diagnostics
open System.Reflection
open System.Windows
open System.Windows.Markup 

// from http://v2matveev.blogspot.com/2010/03/f-and-wpf-or-how-to-make-life-bit.html

[<AbstractClass>]
/// Usage: FsUiObject<FrameworkElement>(__SOURCE_FILE__.Replace(".fs", ".xaml"))
/// The FsUiObject will be set as the data context of the FrameworkElement
/// All the [<DefaultValue>] mutable val's will be set to the elements with the same name in XAML
type FsUiObject<'T when 'T :> FrameworkElement> (xamlPath) as this = 
    
    let loadXaml () = 
        let currentAssembly = Assembly.GetExecutingAssembly()
        let stackTrace = new StackTrace()
        let assembly =
            stackTrace.GetFrames() 
            |> Seq.map (fun frame -> frame.GetMethod().DeclaringType.Assembly)
            |> Seq.find (fun asm -> asm <> currentAssembly)
        use resourceStream = assembly.GetManifestResourceStream(xamlPath)
        XamlReader.Load(resourceStream)

    let uiObj = loadXaml() :?> 'T
     
    let flags = BindingFlags.Instance ||| BindingFlags.NonPublic ||| BindingFlags.Public
     
    do 
        let fields = 
            this.GetType().GetFields(flags) 
            |> Seq.choose (fun f -> 
                let attrs =  f.GetCustomAttributes(typeof<Microsoft.FSharp.Core.DefaultValueAttribute>, false)
                if attrs.Length = 0 then 
                    None                    
                else
                    let attr = attrs.[0] :?> Microsoft.FSharp.Core.DefaultValueAttribute
                    Some(f, f.Name))
        for field, name in fields do
            let value = uiObj.FindName(name)
            if value <> null then
                field.SetValue(this, value)
            else
                failwithf "Ui element %s not found" name
 
        uiObj.DataContext <- this

    member x.UiObject = uiObj
