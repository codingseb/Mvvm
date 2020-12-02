using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Xaml;

namespace CodingSeb.Mvvm
{
    internal class InternalExpressionEvaluatorWithXamlContext : ExpressionEvaluator.ExpressionEvaluator
    {
        private static readonly Regex elementNameRegex =
            new Regex(@"^((\#(?<ElementName>[\p{L}_][\p{L}_0-9]*))|([@](?<ResourceKey>[\p{L}_][\p{L}_0-9]*))|(?<Self>\$self)|(?<Parent>\$parent(\[\s*((?<AncestorLevel>\d+)|(?<AncestorType>[^;\] \t]+)(\s*;\s*(?<AncestorLevel>\d+))?)\s*\])?))");

        public DependencyObject TargetObject { get; set; }

        public Dictionary<string, object> BindingsSourcesDict = new Dictionary<string, object>();
        protected Dictionary<string, Type> XamlTypesDict { get; } = new Dictionary<string, Type>();

        public InternalExpressionEvaluatorWithXamlContext(object contextObject) : base(contextObject) { }

        public InternalExpressionEvaluatorWithXamlContext(object contextObject, IServiceProvider serviceProvider) : base(contextObject)
        {
            IXamlNamespaceResolver namespaceResolver = serviceProvider.GetService(typeof(IXamlNamespaceResolver)) as IXamlNamespaceResolver;
            XamlSchemaContext xamlSchemaContext = (serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider)?.SchemaContext;

            var namespaceDictionary = namespaceResolver?.GetNamespacePrefixes().ToDictionary(namespaceDefinition => namespaceDefinition.Prefix);

            xamlSchemaContext?.GetAllXamlNamespaces().ToList().ForEach(ns =>
                xamlSchemaContext?.GetAllXamlTypes(ns).Where(t => typeof(DependencyObject).IsAssignableFrom(t.UnderlyingType)).ToList()
                    .ForEach(xamlType => XamlTypesDict[xamlType.Name] = xamlType.UnderlyingType));
        }

        protected override void Init()
        {
            Namespaces.Add("System.Windows");
            Namespaces.Add("System.Windows.Controls");
            Namespaces.Add("System.Windows.Media");
            Namespaces.Add("System.Diagnostics");
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
                            Type type;

                            if (XamlTypesDict.ContainsKey(match.Groups["AncestorType"].Value))
                            {
                                type = XamlTypesDict[match.Groups["AncestorType"].Value];
                            }
                            else
                            {
                                object typeResult = Evaluate(match.Groups["AncestorType"].Value);
                                type = (typeResult as ExpressionEvaluator.ClassOrEnumType)?.Type ?? typeResult as Type ?? typeof(DependencyObject);
                            }

                            BindingsSourcesDict[name] = TargetObject
                                    .FindVisualParent(type, match.Groups["AncestorLevel"].Success ? int.Parse(match.Groups["AncestorLevel"].Value) : 1);
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
