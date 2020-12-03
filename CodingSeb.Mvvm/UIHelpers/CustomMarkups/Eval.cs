using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
        #region Constructor and ManageArgs

        public Eval()
        { }

        public Eval(string evaluate)
        {
            Evaluate = evaluate;
        }

        #endregion

        /// <summary>
        /// The Expression to Evaluate
        /// </summary>
        [ConstructorArgument("evaluate")]
        public string Evaluate { get; set; }

        /// <summary>
        /// The value to return if something go wrong in bindings or evaluation.
        /// </summary>
        public object FallbackValue { get; set; }

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
                OptionScriptNeedSemicolonAtTheEndOfLastExpression = false
            };

            var internalConverter = new EvalInternalConverter()
            {
                Evaluator = evaluator,
                Evaluate = Evaluate,
                DataContext = dataContext,
                FallbackValue = FallbackValue,
                TargetObject = targetObject,
                TargetProperty = targetProperty,
                ResetAutoBindings = AutoBinding == EvalAutoBinding.AutoBindingAtEachEvaluation
            };

            MultiBinding multiBinding = new MultiBinding()
            {
                Converter = internalConverter
            };

            if (AutoBinding != EvalAutoBinding.DoNotAutoBinding)
            {
                evaluator.PreEvaluateVariable += internalConverter.PreEvaluateVariables;
            }

            try
            {
                internalConverter.LastValue = evaluator.ScriptEvaluate(Evaluate);
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
                multiBinding.Bindings.Add(new Binding(nameof(EvalInternalConverter.EvaluationCounter))
                {
                    Source = internalConverter
                });

                if (hierarchyBuilding)
                    return multiBinding;

                BindingOperations.SetBinding(targetObject, targetProperty, multiBinding);

                return multiBinding.ProvideValue(serviceProvider);
            }

            return internalConverter.LastValue;
        }

        protected class EvalInternalConverter : IMultiValueConverter, INotifyPropertyChanged
        {
            private int evaluationCounter;
            public ExpressionEvaluator.ExpressionEvaluator Evaluator { get; set; }
            public List<INotifyPropertyChanged> NotifyProperyChangedList { get; } = new List<INotifyPropertyChanged>();
            public List<DependencyPropertyListener> DependencyPropertyListeners { get; } = new List<DependencyPropertyListener>();

            public string Evaluate { get; set; }
            public object DataContext { get; set; }
            public object LastValue { get; set; }
            public object FallbackValue { get; set; }
            public bool ResetAutoBindings { get; set; }
            public DependencyObject TargetObject { get; set; }
            public DependencyProperty TargetProperty { get; set; }

            public int EvaluationCounter
            {
                get { return evaluationCounter; }
                set
                {
                    evaluationCounter = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(EvaluationCounter)));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if(ResetAutoBindings)
                {
                    NotifyProperyChangedList.ForEach(notifyPropertyChanged => notifyPropertyChanged.PropertyChanged -= NotifyPropertyChanged_PropertyChanged);
                    DependencyPropertyListeners.ForEach(listener => listener.Dispose());
                    NotifyProperyChangedList.Clear();
                    DependencyPropertyListeners.Clear();
                }

                try
                {
                    LastValue = Evaluator.ScriptEvaluate(Evaluate);
                }
                catch
                {
                    if(FallbackValue != null)
                    {
                        LastValue = FallbackValue;
                    }
                }

                return LastValue;
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
                        notifyPropertyChanged.PropertyChanged += NotifyPropertyChanged_PropertyChanged;
                        NotifyProperyChangedList.Add(notifyPropertyChanged);
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
                BindingOperations.GetBindingExpression(TargetObject, TargetProperty)?.UpdateTarget();
                BindingOperations.GetMultiBindingExpression(TargetObject, TargetProperty)?.UpdateTarget();
            }
        }
    }
}
