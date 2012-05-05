namespace FsWpf

open System.Windows.Markup

[<AbstractClass>]
type ConverterMarkupExtension<'OriginalType, 'ConvertedToType>() as this =    
    
    inherit MarkupExtension()
    
    override x.ProvideValue serviceProvider = IValueConverter.createConverter this.Convert :> obj
    
    abstract member Convert : 'OriginalType -> 'ConvertedToType
    default x.Convert value = Utilities.applyIfNotNull x.ConvertWhenNotNull value

    abstract member ConvertWhenNotNull : 'OriginalType -> 'ConvertedToType
    default x.ConvertWhenNotNull _ = Unchecked.defaultof<'ConvertedToType>

[<AbstractClass>]
type ConverterWithParameterMarkupExtension<'OriginalType, 'ParameterType, 'ConvertedToType>() as this =    
    
    inherit MarkupExtension()
    
    override x.ProvideValue serviceProvider = IValueConverter.createConverterWithParameter this.Convert :> obj
    
    abstract member Convert : 'OriginalType -> 'ParameterType -> 'ConvertedToType
    default x.Convert value parameter = Utilities.applyIfNotNull x.ConvertWhenNotNull value parameter

    abstract member ConvertWhenNotNull : 'OriginalType -> 'ParameterType -> 'ConvertedToType
    default x.ConvertWhenNotNull _ _ = Unchecked.defaultof<'ConvertedToType>

open System.Xaml
open System.Windows

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

        IValueConverter.createConverter (fun value -> this.Convert value (getRoot())) :> obj
    
    abstract member Convert : 'OriginalType -> 'RootObjectDataContextType -> 'ConvertedToType
    default x.Convert value root = Utilities.applyIfNotNull (fun value -> x.ConvertWhenNotNull value root) value

    abstract member ConvertWhenNotNull : 'OriginalType -> 'RootObjectDataContextType -> 'ConvertedToType
    default x.ConvertWhenNotNull _ _ = Unchecked.defaultof<'ConvertedToType>

[<AbstractClass>]
type ConverterWithParameterMarkupExtension<'OriginalType, 'ParameterType, 'ConvertedToType, 'RootObjectDataContextType>() as this =    
    
    inherit MarkupExtension()
    
    let mutable rootObjectDataContext = None
    
    override x.ProvideValue serviceProvider = 
    
        let getRoot() = 
            if rootObjectDataContext.IsNone then
                let rootObjectProvider = serviceProvider.GetService(typeof<IRootObjectProvider>) :?> IRootObjectProvider                
                let rootObject = rootObjectProvider.RootObject :?> FrameworkElement
                rootObjectDataContext <- rootObject.DataContext :?> 'RootObjectDataContextType |> Some
            rootObjectDataContext.Value

        IValueConverter.createConverter (fun value parameter -> this.Convert value parameter (getRoot())) :> obj
    
    abstract member Convert : 'OriginalType -> 'ParameterType -> 'RootObjectDataContextType -> 'ConvertedToType
    default x.Convert value parameter root = Utilities.applyIfNotNull (fun value parameter -> x.ConvertWhenNotNull value parameter root) value parameter

    abstract member ConvertWhenNotNull : 'OriginalType -> 'ParameterType -> 'RootObjectDataContextType -> 'ConvertedToType
    default x.ConvertWhenNotNull _ _ _ = Unchecked.defaultof<'ConvertedToType>
