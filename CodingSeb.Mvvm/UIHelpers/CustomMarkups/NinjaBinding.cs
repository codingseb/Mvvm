﻿using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// Binds to the datacontext of the current root object or elementname
    /// Use this for things that are not in the visual tree such as DataGridColumn
    /// </summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class NinjaBinding : MarkupExtension
    {
        private static readonly DependencyObject DependencyObject = new DependencyObject();
        private static readonly string[] DoNotCopy = { "Path", "Source", "ElementName", "RelativeSource", "ValidationRules" };
        private static readonly PropertyInfo[] CopyProperties = typeof(Binding).GetProperties().Where(x => !DoNotCopy.Contains(x.Name)).ToArray();

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjaBinding"/> class.
        /// </summary>
        public NinjaBinding()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjaBinding"/> class.
        /// </summary>
        /// <param name="binding">The binding to apply.</param>
        public NinjaBinding(Binding binding)
        {
            Binding = binding;
        }

        /// <summary>
        /// The binding to apply.
        /// </summary>
        [ConstructorArgument("binding")]
        public Binding Binding { get; set; }

        private bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(DependencyObject);

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Binding == null)
            {
                throw new ArgumentException("Binding == null");
            }

            if (IsInDesignMode)
            {
                if (Binding.RelativeSource != null)
                {
                    throw new NotSupportedException("NinjaBinding does not support Binding with RelativeSource, try using ElementName instead.");
                }

                return DefaultValue(serviceProvider);
            }

            Binding binding;
            if (Binding.ElementName != null)
            {
                var reference = new Reference(Binding.ElementName);
                if (!(reference.ProvideValue(serviceProvider) is FrameworkElement source))
                {
                    var rootObjectProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
                    if (rootObjectProvider == null)
                    {
                        throw new ArgumentException($"Could not resolve element: {Binding.ElementName}");
                    }

                    if (rootObjectProvider.RootObject is FrameworkElement root && root.Name == Binding.ElementName)
                    {
                        source = root;
                    }
                    else
                    {
                        throw new ArgumentException($"Could not resolve element: {Binding.ElementName}");
                    }
                }

                binding = CreateElementNameBinding(Binding, source);
            }
            else if (Binding.RelativeSource != null)
            {
                return null;
            }
            else
            {
                var rootObjectProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
                if (rootObjectProvider == null)
                {
                    throw new ArgumentException("rootObjectProvider == null");
                }

                binding = CreateDataContextBinding((FrameworkElement)rootObjectProvider.RootObject, Binding);
            }

            return binding.ProvideValue(serviceProvider);
        }

        private static Binding CreateElementNameBinding(Binding original, object source)
        {
            var binding = new Binding()
            {
                Path = original.Path,
                Source = source,
            };
            SyncProperties(original, binding);
            return binding;
        }

        private static Binding CreateDataContextBinding(FrameworkElement rootObject, Binding original)
        {
            string path = $"{FrameworkElement.DataContextProperty.Name}.{original.Path.Path}";
            var binding = new Binding(path)
            {
                Source = rootObject,
            };
            SyncProperties(original, binding);
            return binding;
        }

        private static void SyncProperties(Binding source, Binding target)
        {
            foreach (var copyProperty in CopyProperties)
            {
                var value = copyProperty.GetValue(source);
                copyProperty.SetValue(target, value);
            }

            foreach (var rule in source.ValidationRules)
            {
                target.ValidationRules.Add(rule);
            }
        }

        private static object DefaultValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (provideValueTarget == null)
            {
                throw new ArgumentException("provideValueTarget == null");
            }

            var dependencyProperty = (DependencyProperty)provideValueTarget.TargetProperty;
            return dependencyProperty.DefaultMetadata.DefaultValue;
        }
    }
}
