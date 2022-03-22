using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// To easily add binding in xMultiBinding Hierarchy and not be blocked by Xaml
    /// </summary>
    public class XBindingProxy : MarkupExtension
    {
        /// <summary>
        /// The Binding to proxy
        /// </summary>
        public BindingBase Binding { get; set; }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider, false);
        }

        /// <summary>
        /// Special ProvideValue that can indicate that this MarkupExtension is used in a XMultibinding for example and is not the root binding
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension</param>
        /// <param name="hierarchyBuilding">Should be given as <c>true</c> when this markup extension is used in an other XMultibinding. So it do not set the binding directly on the dependencyProperty</param>
        /// <returns>The given binding</returns>
        /// <exception cref="ArgumentNullException"></exception>
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
