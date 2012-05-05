namespace FsWpf

open System
open System.Windows
open System.Windows.Data
open System.Windows.Input
open System.Windows.Markup
open System.Xaml

[<AbstractClass>]
type ConverterMarkupExtension<'OriginalType, 'ConvertedToType>() as this =    
    
    inherit MarkupExtension()
    
    override x.ProvideValue serviceProvider = 
    
        { new IValueConverter with
               
            override x.Convert(value, targetType, parameter, culture) =
                let castedValue = 
                    if value = null then 
                        Unchecked.defaultof<'OriginalType>
                    else 
                        value :?> 'OriginalType
                this.Convert castedValue :> obj

            override x.ConvertBack(value, targetType, parameter, culture) =
                failwith "ConvertBack not supported"

        } :> obj
    
    abstract member Convert : 'OriginalType -> 'ConvertedToType
    default x.Convert param = 
        if obj.ReferenceEquals(param, null) then
            Unchecked.defaultof<'ConvertedToType>
        else
            x.ConvertWhenNotNull param

    abstract member ConvertWhenNotNull : 'OriginalType -> 'ConvertedToType
    default x.ConvertWhenNotNull _ = Unchecked.defaultof<'ConvertedToType>

[<AbstractClass>]
type ConverterMarkupExtension<'OriginalType, 'ConvertedToType, 'RootObjectDataContextType>() as this =    
    
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
                let castedValue = 
                    if value = null then 
                        Unchecked.defaultof<'OriginalType>
                    else 
                        value :?> 'OriginalType
                this.Convert castedValue (getRoot()) :> obj

            override x.ConvertBack(value, targetType, parameter, culture) =
                failwith "ConvertBack not supported"

        } :> obj
    
    abstract member Convert : 'OriginalType -> 'RootObjectDataContextType -> 'ConvertedToType
    default x.Convert param root = 
        if obj.ReferenceEquals(param, null) then
            Unchecked.defaultof<'ConvertedToType>
        else
            x.ConvertWhenNotNull param root

    abstract member ConvertWhenNotNull : 'OriginalType -> 'RootObjectDataContextType -> 'ConvertedToType
    default x.ConvertWhenNotNull _ _ = Unchecked.defaultof<'ConvertedToType>
