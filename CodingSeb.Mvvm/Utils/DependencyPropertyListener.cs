using System;
using System.Windows;
using System.Windows.Data;

namespace CodingSeb.Mvvm
{
    public sealed class DependencyPropertyListener : DependencyObject, IDisposable
    {
        private static readonly DependencyProperty ProxyProperty = DependencyProperty.Register(
            "Proxy",
            typeof(object),
            typeof(DependencyPropertyListener),
            new PropertyMetadata(null, OnSourceChanged));

        private bool disposed;

        public DependencyPropertyListener(
            DependencyObject source,
            PropertyPath property)
        {
            BindingOperations.SetBinding(this, ProxyProperty, new Binding
            {
                Source = source,
                Path = property,
                Mode = BindingMode.OneWay,
            });
        }

        public DependencyPropertyListener(BindingBase bindingBase, DependencyObject dependencyObject)
        {
            if(bindingBase is Binding binding)
            {
                bindingBase = TransformBindingToSourceMode(binding, dependencyObject);
            }
            else if (bindingBase is MultiBinding multiBinding)
            {
                for(int i = 0; i < multiBinding.Bindings.Count; i++)
                {
                    if(multiBinding.Bindings[i] is Binding binding2)
                    {
                        multiBinding.Bindings[i] = TransformBindingToSourceMode(binding2, dependencyObject);
                    }
                }
            }

            BindingOperations.SetBinding(this, ProxyProperty, bindingBase);
        }

        private BindingBase TransformBindingToSourceMode(Binding binding, DependencyObject dependencyObject)
        {
            if (binding.ElementName != null && dependencyObject is FrameworkElement frameworkElement)
            {
                binding = new Binding()
                    { Source = frameworkElement.FindName(binding.ElementName) }
                    .PartialCopyFrom(binding);
            }
            else if (binding.RelativeSource != null)
            {
                if (binding.RelativeSource.Mode == RelativeSourceMode.Self)
                {
                    binding = new Binding()
                        { Source = dependencyObject }
                        .PartialCopyFrom(binding);
                }
                else if (binding.RelativeSource.Mode == RelativeSourceMode.FindAncestor)
                {
                    binding = new Binding()
                        { Source = dependencyObject.FindLogicalParent(binding.RelativeSource.AncestorType, Math.Max(binding.RelativeSource.AncestorLevel, 1))}
                        .PartialCopyFrom(binding);
                }
            }
            else if(binding.Source == null && dependencyObject is FrameworkElement frameworkElement2)
            {
                binding = new Binding()
                    { Source = frameworkElement2.DataContext }
                    .PartialCopyFrom(binding);
            }

            return binding;
        }

        public event EventHandler<DependencyPropertyChangedEventArgs> Changed;

        public object Value => GetValue(ProxyProperty);

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            BindingOperations.ClearBinding(this, ProxyProperty);
            Changed = null;
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listener = (DependencyPropertyListener)d;
            if (listener.disposed)
            {
                return;
            }

            listener.OnChanged(e);
        }

        private void OnChanged(DependencyPropertyChangedEventArgs e)
        {
            Changed?.Invoke(this, e);
        }
    }

    internal static class BindingExtensions
    {
        internal static Binding PartialCopyFrom(this Binding newBinding, Binding binding)
        {
            newBinding.Delay = binding.Delay;

            if(binding.XPath != null)
                newBinding.XPath = binding.XPath;
            if(binding.BindingGroupName != null)
                newBinding.BindingGroupName = binding.BindingGroupName;
            if(binding.FallbackValue != null)
                newBinding.FallbackValue = binding.FallbackValue;
            if(binding.StringFormat != null)
                newBinding.StringFormat = binding.StringFormat;
            if (binding.Converter != null)
                newBinding.Converter = binding.Converter;
            if (binding.ConverterCulture != null)
                newBinding.ConverterCulture = binding.ConverterCulture;
            if (binding.ConverterParameter != null)
                newBinding.ConverterParameter = binding.ConverterParameter;
            if (binding.Path != null)
                newBinding.Path = binding.Path;

            newBinding.Mode = BindingMode.OneWay;
            newBinding.UpdateSourceTrigger = binding.UpdateSourceTrigger;

            return newBinding;
        }
    }
}
