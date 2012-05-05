namespace FsWpf

open System

[<AbstractClass>]
type ConverterToStringMarkupExtension<'OriginalType>() =        
    inherit ConverterMarkupExtension<'OriginalType, string>()

[<AbstractClass>]
type ConverterToStringMarkupExtension<'OriginalType, 'RootObjectDataContextType>() =        
    inherit ConverterMarkupExtension<'OriginalType, string, 'RootObjectDataContextType>()
