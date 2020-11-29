using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// Allow to execute small C# expressions in Xaml
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

        //public Eval(BindingBase expressionBinding)
        //{
        //    ExpressionBinding = expressionBinding;
        //}

        #endregion

        /// <summary>
        /// The Expression to Evaluate
        /// </summary>
        [ConstructorArgument("expression")]
        public string Expression { get; set; }

        //[ConstructorArgument("expressionBinding")]
        //public BindingBase ExpressionBinding { get; set; }

        /// <summary>
        /// Si mis à <c>true</c> on récupère le résultat mais on ne rend pas l'Eval dynamic en créant des Binding sur les éléments qui les supportent.
        /// Par défaut : <c>false</c>
        /// </summary>
        public bool DoNotBind { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider, false);
        }

        public object ProvideValue(IServiceProvider serviceProvider, bool hierarchyBuilding)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service))
                return this;

            if (!(service.TargetObject is DependencyObject targetObject)
                || !(service.TargetProperty is DependencyProperty targetProperty))
            {
                return this;
            }

            FrameworkElement frameworkElement = targetObject as FrameworkElement;
            object dataContext = frameworkElement?.DataContext;

            var evaluator = new InternalExpressionEvaluator(frameworkElement?.DataContext)
            {
                ServiceProvider = serviceProvider,
                //TargetObject = targetObject
            };

            var internalConverter = new EvalInternalConverter()
            {
                Evaluator = evaluator,
                Expression = Expression,
                DataContext = dataContext,
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

            if (!DoNotBind)
            {
                evaluator.PreEvaluateVariable += PreEvaluateVariables;
            }

            try
            {
                internalConverter.LastValue = evaluator.Evaluate(Expression);
            }
            catch { }

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

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    LastValue = Evaluator.Evaluate(Expression);
                }
                catch { }

                return LastValue;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        protected class InternalExpressionEvaluator : ExpressionEvaluator.ExpressionEvaluator
        {
            private static readonly Regex elementNameRegex = new Regex(@"^#(?<name>[\p{L}_](?>[\p{L}_0-9]*))");

            public IServiceProvider ServiceProvider { get; set; }
            //public DependencyObject TargetObject { get; set; }

            public Dictionary<string, object> elementNameDict = new Dictionary<string, object>();

            public InternalExpressionEvaluator(object contextObject) : base(contextObject)
            {}

            protected override void Init()
            {
                ParsingMethods.Insert(0, EvaluateBindingVariables);
            }

            protected virtual bool EvaluateBindingVariables(string expression, Stack<object> stack, ref int i)
            {
                Match match = elementNameRegex.Match(expression.Substring(i));

                if (match.Success)
                {
                    string name = match.Groups["name"].Value;

                    if (!elementNameDict.ContainsKey(name))
                    {
                        elementNameDict[name] = new Reference(name).ProvideValue(ServiceProvider);
                    }

                    stack.Push(elementNameDict[name]);

                    i += match.Length - 1;

                    return true;
                }

                return false;
            }
        }
    }
}
