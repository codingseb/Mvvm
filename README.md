# Mvvm
[![NuGet Status](http://img.shields.io/nuget/v/CodingSeb.Mvvm.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/CodingSeb.Mvvm/)  
Some useful stuff for better Mvvm in WPF

## Features
* `XCommand` (To bind between Events, Commands, Methods and Property for CanExecute) with smart refresh of the CanExecute (Have some code evaluation features similar to Eval).
* `Eval` Binding with Code Evaluation (Avalonia UI Style and more)
  * On DataContext (Properties and methods directly available)  
  * ElementName `#Name.Something`  
  * RelativeSource `$self`, `$parent` (LogicalTree), `$$parent` (VisualTree), `$parent[TypeName]`, `$parent[3]`, `$parent[TypeName,2]` 
  * StaticResource `@ResourceKey`  
* `XMultiBinding` for nesting `MultiBinding`, `Binding`, `Eval` and other markups. You can use at each level `IMultiValueConverter`, `IValueConveter` ([Work very well with CodingSeb.Converters](https://github.com/codingseb/Converters)) 
* `IRelayCommand` (That inherit from ICommand) to call RaiseCanExecuteChanged() from anywhere
  * `RelayCommand`
  * `RelayCommand<T>`
* Some extensions methods to navigate in both VisualTree and LogicalTree easily from C#
* `NotifyPropertyChangedBaseClass` the class to inherit that implementing `INotifyPropertyChanged` and a `public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")`
  
[Code Evaluation stuff use CodingSeb.ExpressionEvaluator](https://github.com/codingseb/ExpressionEvaluator)

*You can also use [PropertyChanged.Fody](https://github.com/Fody/PropertyChanged) and/or [ReactiveUI.Fody](https://www.reactiveui.net/docs/handbook/view-models/boilerplate-code) to simplify more your Mvvm code*
