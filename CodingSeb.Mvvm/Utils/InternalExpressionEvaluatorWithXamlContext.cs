using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xaml;

namespace CodingSeb.Mvvm
{
    internal class InternalExpressionEvaluatorWithXamlContext : ExpressionEvaluator.ExpressionEvaluator
    {
        private static readonly Regex elementNameRegex =
            new Regex(@"^((\#(?<ElementName>[\p{L}_][\p{L}_0-9]*))|([@](?<ResourceKey>[\p{L}_][\p{L}_0-9]*))|(?<Self>\$self)|(?<Parent>\$(?<InVisualTree>\$)?parent(?![\p{L}_0-9])(?<Parameters>\[)?))");

        public DependencyObject TargetObject { get; set; }

        protected Dictionary<string, Type> XamlTypesDict { get; } = new Dictionary<string, Type>();

        public InternalExpressionEvaluatorWithXamlContext(object contextObject) : base(contextObject) { }

        public InternalExpressionEvaluatorWithXamlContext(object contextObject, IServiceProvider serviceProvider) : base(contextObject)
        {
            IXamlNamespaceResolver namespaceResolver = serviceProvider.GetService(typeof(IXamlNamespaceResolver)) as IXamlNamespaceResolver;
            XamlSchemaContext xamlSchemaContext = (serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider)?.SchemaContext;

            var namespaceDictionary = namespaceResolver?.GetNamespacePrefixes().ToDictionary(namespaceDefinition => namespaceDefinition.Prefix);

            xamlSchemaContext?.GetAllXamlNamespaces().ToList().ForEach(ns =>
                xamlSchemaContext?.GetAllXamlTypes(ns).Where(t => typeof(DependencyObject).IsAssignableFrom(t.UnderlyingType)).ToList()
                    .ForEach(xamlType => Types.Add(xamlType.UnderlyingType)));
        }

        protected override void Init()
        {
            Namespaces.Add("System.Windows");
            Namespaces.Add("System.Windows.Controls");
            Namespaces.Add("System.Windows.Media");
            Namespaces.Add("System.Diagnostics");
            StaticTypesForExtensionsMethods.Add(typeof(LogicalAndVisualTreeExtensions));
            ParsingMethods.Insert(0, EvaluateBindingVariables);
            operatorsDictionary.Add("and", ExpressionEvaluator.ExpressionOperator.ConditionalAnd);
            operatorsDictionary.Add("or", ExpressionEvaluator.ExpressionOperator.ConditionalOr);
            operatorsDictionary.Add("_and", ExpressionEvaluator.ExpressionOperator.ConditionalAnd);
            operatorsDictionary.Add("_or", ExpressionEvaluator.ExpressionOperator.ConditionalOr);
        }

        protected virtual bool EvaluateBindingVariables(string expression, Stack<object> stack, ref int i)
        {
            Match match = elementNameRegex.Match(expression.Substring(i));

            if (match.Success)
            {
                if (match.Groups["Parent"].Success)
                {
                    bool useVisualTreeForParents = match.Groups["InVisualTree"].Success;

                    if (match.Groups["Parameters"].Success)
                    {
                        i += match.Length;

                        List<object> parameters = GetExpressionsBetweenParenthesesOrOtherImbricableBrackets(expression, ref i, true, ",", "[", "]")
                            .ConvertAll(Evaluate);

                        Type type = (parameters[0] as ExpressionEvaluator.ClassOrEnumType)?.Type ?? parameters[0] as Type ?? typeof(DependencyObject);
                        int level = 1;

                        if(parameters[0] is int int0)
                        {
                            level = int0;
                        }
                        else if(parameters.Count > 1 && parameters[1] is int int1)
                        {
                            level = int1;
                        }

                        stack.Push(useVisualTreeForParents ? TargetObject.FindVisualParent(type, level) : TargetObject.FindLogicalParent(type, level));
                    }
                    else
                    {
                        stack.Push(useVisualTreeForParents ? VisualTreeHelper.GetParent(TargetObject) : LogicalTreeHelper.GetParent(TargetObject));
                        i += match.Length - 1;
                    }
                }
                else
                {
                    if (match.Groups["ElementName"].Success)
                    {
                        object elementNameObject = (TargetObject as FrameworkElement ?? TargetObject.FindLogicalParent<FrameworkElement>()).FindName(match.Groups["ElementName"].Value);

                        if (elementNameObject != null)
                            stack.Push(elementNameObject);
                        else
                            return false;
                    }
                    else if (match.Groups["Self"].Success)
                    {
                        stack.Push(TargetObject);
                    }
                    else if (match.Groups["ResourceKey"].Success)
                    {
                        stack.Push(TargetObject.FindNearestResource(match.Groups["ResourceKey"].Value));
                    }

                    i += match.Length - 1;
                }

                return true;
            }

            return false;
        }
    }
}
