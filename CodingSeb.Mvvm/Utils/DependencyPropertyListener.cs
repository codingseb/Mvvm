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
            Binding = new Binding
            {
                Source = source,
                Path = property,
                Mode = BindingMode.OneWay,
            };
            BindingOperations.SetBinding(this, ProxyProperty, Binding);
        }

        public event EventHandler<DependencyPropertyChangedEventArgs> Changed;

        private Binding Binding { get; }

        public DependencyObject Source => (DependencyObject)Binding.Source;

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
}
