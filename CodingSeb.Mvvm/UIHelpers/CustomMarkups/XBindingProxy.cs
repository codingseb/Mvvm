using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// To easily add binding in xMultiBinding Hierarchy and not be block by Xaml
    /// </summary>
    public class XBindingProxy : MarkupExtension
    {
        public BindingBase Binding { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider, false);
        }

        public object ProvideValue(IServiceProvider serviceProvider, bool hierarchyBuilding)
        {
            if(!hierarchyBuilding)
            {
                if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service)
                || !(service.TargetObject is DependencyObject targetObject)
                || !(service.TargetProperty is DependencyProperty targetProperty))
                {
                    return this;
                }

                BindingOperations.SetBinding(targetObject, targetProperty, Binding);
            }

            return Binding;
        }
    }
}
