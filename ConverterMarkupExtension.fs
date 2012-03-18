namespace FsWpf

open System
open System.Windows
open System.Windows.Data
open System.Windows.Input
open System.Windows.Markup
open System.Xaml

[<AbstractClass>]
type ConverterMarkupExtension<'ConvertedValueType, 'RootObjectDataContextType>() as this =    
    
    inherit MarkupExtension()
    
    let mutable rootObjectDataContext = None
    
    override x.ProvideValue serviceProvider = 
    
        let getRoot() = 
            if rootObjectDataContext.IsNone then
                let rootObjectProvider = serviceProvider.GetService(typeof<IRootObjectProvider>) :?> IRootObjectProvider                
                let rootObject = rootObjectProvider.RootObject :?> FrameworkElement
                rootObjectDataContext <- rootObject.DataContext :?> 'RootObjectDataContextType |> Some
            rootObjectDataContext.Value

        { new IValueConverter with            
            override x.Convert(value, targetType, parameter, culture) =
                if value = null then null else this.Convert (value :?> 'ConvertedValueType) (getRoot())
            override x.ConvertBack(value, targetType, parameter, culture) =
                failwith "ConvertBack not supported"
        } :> obj
    
    abstract member Convert : 'ConvertedValueType -> 'RootObjectDataContextType -> obj
