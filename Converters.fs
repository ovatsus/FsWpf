namespace FsWpf

open System.Windows

type BooleanInverterConverter() = 
    inherit ConverterMarkupExtension<bool, bool>()
        override x.Convert b = not b

type BooleanToVisibilityConverter() = 
    inherit ConverterWithParameterMarkupExtension<bool, string, Visibility>()
        override x.Convert b param = 
            let visibilityValues = param.Split ',' 
            Utilities.parseEnum (if b then visibilityValues.[0] else visibilityValues.[1])

type BooleanToStringConverter() = 
    inherit ConverterWithParameterMarkupExtension<bool, string, string>()
        override x.Convert b param = 
            let values = param.Split ',' 
            if b then values.[0] else values.[1]
