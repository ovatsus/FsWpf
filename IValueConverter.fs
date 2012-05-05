module FsWpf.IValueConverter

open System.Windows.Data

let createConverter converter =    
    
    { new IValueConverter with
               
        override x.Convert(value, targetType, parameter, culture) =
            converter (Utilities.castIfNotNull value) :> obj

        override x.ConvertBack(value, targetType, parameter, culture) =
            failwith "ConvertBack not supported"
    }

let createConverterWithParameter converter =    
    
    { new IValueConverter with
               
        override x.Convert(value, targetType, parameter, culture) =
            converter (Utilities.castIfNotNull value) (Utilities.castIfNotNull parameter) :> obj

        override x.ConvertBack(value, targetType, parameter, culture) =
            failwith "ConvertBack not supported"
    }
