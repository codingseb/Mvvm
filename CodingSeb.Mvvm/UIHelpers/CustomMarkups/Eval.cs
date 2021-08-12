using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// Allow to execute small C# expressions in Xaml as Binding
    /// </summary>
    public class Eval : MarkupExtension
    {
        private BindingBase evaluateBinding;
        private string evaluate;
        #region Constructor and ManageArgs

        public Eval()
        { }

        public Eval(object evaluate)
        {
            if(evaluate is BindingBase evaluateBinding)
                EvaluateBinding = evaluateBinding;
            else
                Evaluate = evaluate.ToString();
        }

        #endregion

        /// <summary>
        /// The Expression to Evaluate
        /// </summary>
        public string Evaluate
        {
            get { return evaluate; }
            set
            { evaluate = value.EscapeForXaml(); }
        }

        public BindingBase EvaluateBinding
        {
            get { return evaluateBinding; }
            set
            {
                evaluateBinding = value;
                AutoBinding = EvalAutoBinding.AutoBindingAtEachEvaluation;
            }
        }

        /// <summary>
        /// The value to return if something go wrong in bindings or evaluation.
        /// </summary>
        public object FallbackValue { get; set; }

        /// <summary>
        /// To StringFormat the result.
        /// </summary>
        public string StringFormat { get; set; }

        /// <summary>
        /// To specify the way evaluation auto create bindings to update the result on changes of used dependencyProperties and INotifyPropertyChanged
        /// Default : <c>AutoBindingAtFirstEvaluation</c>
        /// </summary>
        public EvalAutoBinding AutoBinding { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider, false);
        }

        public object ProvideValue(IServiceProvider serviceProvider, bool hierarchyBuilding)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service)
                || !(service.TargetObject is DependencyObject targetObject)
                || !(service.TargetProperty is DependencyProperty targetProperty))
            {
                return this;
            }

            FrameworkElement frameworkElement = targetObject as FrameworkElement;
            object dataContext = frameworkElement?.DataContext;

            var evaluator = new InternalExpressionEvaluatorWithXamlContext(dataContext, serviceProvider)
            {
                TargetObject = targetObject,
                OptionsSyntaxRules = new ExpressionEvaluator.ExpressionEvaluator.SyntaxRules()
                {
                    MandatoryLastStatementTerminalPunctuator = false
                }
            };

            var internalConverter = new EvalInternalConverter()
            {
                Evaluator = evaluator,
                Evaluate = Evaluate,
                EvaluateBinding = EvaluateBinding,
                DataContext = dataContext,
                FallbackValue = FallbackValue,
                StringFormat = StringFormat,
                TargetObject = targetObject,
                TargetProperty = targetProperty,
                ResetAutoBindings = AutoBinding == EvalAutoBinding.AutoBindingAtEachEvaluation,
                IsInHierarchy = hierarchyBuilding
            };

            MultiBinding multiBinding = new MultiBinding()
            {
                Converter = internalConverter
            };

            if (EvaluateBinding is Binding binding)
                multiBinding.Bindings.Add(binding);
            else if (EvaluateBinding is MultiBinding evalMultiBinding)
                evalMultiBinding.Bindings.ToList().ForEach(multiBinding.Bindings.Add);
            else
                multiBinding.Bindings.Add(new Binding() { Source = Evaluate, Mode=BindingMode.OneWay });

            if (AutoBinding != EvalAutoBinding.DoNotAutoBinding)
            {
                evaluator.PreEvaluateVariable += internalConverter.PreEvaluateVariables;
            }

            try
            {
                if (string.IsNullOrEmpty(Evaluate))
                {
                    internalConverter.LastValue = evaluator.ScriptEvaluate(Evaluate);

                    if (StringFormat != null)
                        internalConverter.LastValue = string.Format(StringFormat, internalConverter.LastValue);

                    if (!hierarchyBuilding)
                    {
                        internalConverter.LastValue = MarkupStandardTypeConverter.ConvertValueForDependencyProperty(internalConverter.LastValue, targetProperty);
                    }
                }
            }
            catch
            {
                if (FallbackValue != null)
                    internalConverter.LastValue = FallbackValue;
            }

            if(AutoBinding == EvalAutoBinding.AutoBindingAtFirstEvaluation)
            {
                evaluator.PreEvaluateVariable -= internalConverter.PreEvaluateVariables;
            }

            if (AutoBinding != EvalAutoBinding.DoNotAutoBinding)
            {
                if (hierarchyBuilding)
                    return multiBinding;

                BindingOperations.SetBinding(targetObject, targetProperty, multiBinding);

                return multiBinding.ProvideValue(serviceProvider);
            }

            return internalConverter.LastValue;
        }

        protected class EvalInternalConverter : IMultiValueConverter
        {
            public ExpressionEvaluator.ExpressionEvaluator Evaluator { get; set; }
            private WeakDictionary<INotifyPropertyChanged, List<string>> PropertiesToBindDict { get; } = new WeakDictionary<INotifyPropertyChanged, List<string>>();
            private List<DependencyPropertyListener> DependencyPropertyListeners { get; } = new List<DependencyPropertyListener>();

            public bool IsInHierarchy { get; set; }
            public string Evaluate { get; set; }
            public BindingBase EvaluateBinding { get; set; }
            public object DataContext { get; set; }
            public object LastValue { get; set; }
            public object FallbackValue { get; set; }
            public string StringFormat { get; set; }
            public bool ResetAutoBindings { get; set; }
            public DependencyObject TargetObject { get; set; }
            public DependencyProperty TargetProperty { get; set; }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if(ResetAutoBindings)
                {
                    PropertiesToBindDict.Keys.ToList()
                        .ForEach(notifyPropertyChanged => WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(notifyPropertyChanged, nameof(INotifyPropertyChanged.PropertyChanged), NotifyPropertyChanged_PropertyChanged));
                    DependencyPropertyListeners.ForEach(listener => listener.Dispose());
                    PropertiesToBindDict.Clear();
                    DependencyPropertyListeners.Clear();
                }

                try
                {
                    if (EvaluateBinding is MultiBinding evalMultiBinding)
                        Evaluate = evalMultiBinding.Converter.Convert(values.Take(evalMultiBinding.Bindings.Count).ToArray(), null, evalMultiBinding.ConverterParameter, evalMultiBinding.ConverterCulture).ToString();
                    else if(EvaluateBinding != null)
                        Evaluate = values[0].ToString();

                    LastValue = Evaluator.ScriptEvaluate(Evaluate);

                    if (StringFormat != null)
                        LastValue = string.Format(StringFormat, LastValue);
                }
                catch
                {
                    if(FallbackValue != null)
                    {
                        LastValue = FallbackValue;
                    }
                }

                return IsInHierarchy ? LastValue : MarkupStandardTypeConverter.ConvertValueForDependencyProperty(LastValue, TargetProperty);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            internal void PreEvaluateVariables(object sender, ExpressionEvaluator.VariablePreEvaluationEventArg args)
            {
                if (args.This != TargetObject || !args.Name.Equals(TargetProperty.Name))
                {
                    if (args.This is INotifyPropertyChanged notifyPropertyChanged)
                    {
                        WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(notifyPropertyChanged, nameof(INotifyPropertyChanged.PropertyChanged), NotifyPropertyChanged_PropertyChanged);
                        if (!PropertiesToBindDict.ContainsKey(notifyPropertyChanged))
                            PropertiesToBindDict[notifyPropertyChanged] = new List<string>();

                        PropertiesToBindDict[notifyPropertyChanged].Add(args.Name);
                    }
                    else if(args.This is DependencyObject dependencyObject)
                    {
                        var dependencyPropertyListener = new DependencyPropertyListener(dependencyObject, new PropertyPath(args.Name));
                        dependencyPropertyListener.Changed += DependencyPropertyListener_Changed;
                        DependencyPropertyListeners.Add(dependencyPropertyListener);
                    }
                }
            }

            private void DependencyPropertyListener_Changed(object sender, DependencyPropertyChangedEventArgs e)
            {
                BindingOperations.GetBindingExpression(TargetObject, TargetProperty)?.UpdateTarget();
                BindingOperations.GetMultiBindingExpression(TargetObject, TargetProperty)?.UpdateTarget();
            }

            private void NotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (sender is INotifyPropertyChanged notifyPropertyChanged && PropertiesToBindDict.ContainsKey(notifyPropertyChanged) && PropertiesToBindDict[notifyPropertyChanged].Contains(e.PropertyName))
                {
                    BindingOperations.GetBindingExpression(TargetObject, TargetProperty)?.UpdateTarget();
                    BindingOperations.GetMultiBindingExpression(TargetObject, TargetProperty)?.UpdateTarget();
                }
            }
        }
    }
}
