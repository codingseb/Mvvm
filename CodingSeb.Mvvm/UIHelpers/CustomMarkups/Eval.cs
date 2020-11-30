using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xaml;

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
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service))
                return this;

            if (!(service.TargetObject is DependencyObject targetObject)
                || !(service.TargetProperty is DependencyProperty targetProperty))
            {
                return this;
            }

            FrameworkElement frameworkElement = targetObject as FrameworkElement;
            object dataContext = frameworkElement?.DataContext;

            IXamlNamespaceResolver namespaceResolver = serviceProvider.GetService(typeof(IXamlNamespaceResolver)) as IXamlNamespaceResolver;
            XamlSchemaContext xamlSchemaContext = (serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider)?.SchemaContext;

            var namespaceDictionary = namespaceResolver?.GetNamespacePrefixes().ToDictionary(namespaceDefinition => namespaceDefinition.Prefix);

            Dictionary<string, Type> xamlTypeNameToTypeDict = new Dictionary<string, Type>();

            xamlSchemaContext?.GetAllXamlNamespaces().ToList().ForEach(ns =>
                xamlSchemaContext?.GetAllXamlTypes(ns).Where(t => typeof(DependencyObject).IsAssignableFrom(t.UnderlyingType)).ToList()
                    .ForEach(xamlType => xamlTypeNameToTypeDict[xamlType.Name] = xamlType.UnderlyingType));

            var evaluator = new InternalExpressionEvaluator(frameworkElement?.DataContext)
            {
                TargetObject = targetObject,
                TargetProperty = targetProperty,
                XamlTypesDict = xamlTypeNameToTypeDict
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

        protected class InternalExpressionEvaluator : ExpressionEvaluator.ExpressionEvaluator
        {
            private static readonly Regex elementNameRegex =
                new Regex(@"^((\#(?<ElementName>[\p{L}_][\p{L}_0-9]*))|([@](?<ResourceKey>[\p{L}_][\p{L}_0-9]*))|(?<Self>\$self)|(?<Parent>\$parent(\[\s*((?<AncestorLevel>\d+)|(?<AncestorType>[^;\] \t]+)(\s*;\s*(?<AncestorLevel>\d+))?)\s*\])?))");

            public DependencyObject TargetObject { get; set; }
            public DependencyProperty TargetProperty { get; set; }

            public Dictionary<string, object> BindingsSourcesDict = new Dictionary<string, object>();
            public Dictionary<string,Type> XamlTypesDict { get; set; }

            public MultiBinding LinkedMultiBinding { get; set; }

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
                    string name = match.Value;

                    if (!BindingsSourcesDict.ContainsKey(name))
                    {
                        if (match.Groups["ElementName"].Success)
                        {
                            BindingsSourcesDict[name] = (TargetObject as FrameworkElement ?? TargetObject.FindLogicalParent<FrameworkElement>()).FindName(match.Groups["ElementName"].Value);
                        }
                        else if (match.Groups["Self"].Success)
                        {
                            BindingsSourcesDict[name] = TargetObject;
                        }
                        else if (match.Groups["ResourceKey"].Success)
                        {
                            BindingsSourcesDict[name] = TargetObject.FindNearestResource(match.Groups["ResourceKey"].Value);
                        }
                        else if (match.Groups["Parent"].Success)
                        {
                            if (match.Groups["AncestorType"].Success)
                            {
                                if (XamlTypesDict.ContainsKey(match.Groups["AncestorType"].Value))
                                {
                                    BindingsSourcesDict[name] = TargetObject
                                        .FindVisualParent(XamlTypesDict[match.Groups["AncestorType"].Value], match.Groups["AncestorLevel"].Success ? int.Parse(match.Groups["AncestorLevel"].Value) : 1);
                                }
                            }
                            else if (match.Groups["AncestorLevel"].Success)
                            {
                                BindingsSourcesDict[name] = TargetObject.FindVisualParent(typeof(DependencyObject), int.Parse(match.Groups["AncestorLevel"].Value));
                            }
                            else
                            {
                                BindingsSourcesDict[name] = VisualTreeHelper.GetParent(TargetObject);
                            }
                        }
                    }

                    stack.Push(BindingsSourcesDict[name]);

                    i += match.Length - 1;

                    return true;
                }

                return false;
            }
        }
    }
}
