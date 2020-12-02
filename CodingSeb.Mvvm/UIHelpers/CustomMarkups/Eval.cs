using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;

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

        public Eval(string expression)
        {
            Expression = expression;
        }

        #endregion

        /// <summary>
        /// The Expression to Evaluate
        /// </summary>
        [ConstructorArgument("expression")]
        public string Expression { get; set; }

        /// <summary>
        /// The value to return if something go wrong in bindings or evaluation.
        /// </summary>
        public object FallbackValue { get; set; }

        /// <summary>
        /// Si mis à <c>true</c> on récupère le résultat mais on ne rend pas l'Eval dynamic en créant des Binding sur les éléments qui les supportent.
        /// Par défaut : <c>false</c>
        /// </summary>
        public bool DoNotAutoBinding { get; set; }

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
            };

            evaluator.StaticTypesForExtensionsMethods.Add(typeof(LogicalAndVisualTreeExtensions));

            var internalConverter = new EvalInternalConverter()
            {
                Evaluator = evaluator,
                Expression = Expression,
                DataContext = dataContext,
                FallbackValue = FallbackValue,
            };

            MultiBinding multiBinding = new MultiBinding()
            {
                Converter = internalConverter
            };

            void PreEvaluateVariables(object sender, ExpressionEvaluator.VariablePreEvaluationEventArg args)
            {
                if (args.This != targetObject || !args.Name.Equals(targetProperty.Name))
                {
                    if (args.This is INotifyPropertyChanged || args.This is DependencyObject)
                    {
                        multiBinding.Bindings.Add(new Binding(args.Name)
                        {
                            Source = args.This,
                            Mode = BindingMode.OneWay
                        });
                    }
                }
            }

            if (!DoNotAutoBinding)
            {
                evaluator.PreEvaluateVariable += PreEvaluateVariables;
            }

            try
            {
                internalConverter.LastValue = evaluator.Evaluate(Expression);
            }
            catch
            {
                if (FallbackValue != null)
                    internalConverter.LastValue = FallbackValue;
            }

            evaluator.PreEvaluateVariable -= PreEvaluateVariables;

            if (multiBinding.Bindings.Count > 0)
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

            public string Expression { get; set; }
            public object DataContext { get; set; }
            public object LastValue { get; set; }
            public object FallbackValue { get; set; }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    LastValue = Evaluator.Evaluate(Expression);
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
        }
    }
}
