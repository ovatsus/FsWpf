namespace FsWpf

open System
open System.Windows
open System.Windows.Controls

type BindableWebBrowser() = 
    inherit Decorator()

    static let SourcePropertyChanged (o : DependencyObject) (e : DependencyPropertyChangedEventArgs) = 
        let bindableWebBrowser = o :?> BindableWebBrowser
        let uri = e.NewValue :?> string
        if String.IsNullOrEmpty uri then
            let webBrowser = bindableWebBrowser.Child :?> WebBrowser
            webBrowser.Dispose()
            bindableWebBrowser.Child <- null
        else
            let webBrowser = new WebBrowser()
            webBrowser.Source <- new Uri(uri)
            bindableWebBrowser.Child <- webBrowser

    static let SourceProperty : DependencyProperty = 
        DependencyProperty.RegisterAttached("Source", typeof<string>, typeof<BindableWebBrowser>, new UIPropertyMetadata(SourcePropertyChanged))

    member x.Source
        with get() = x.GetValue(SourceProperty) :?> string
        and set(v : string) = x.SetValue(SourceProperty, v)
