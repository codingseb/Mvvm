using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// Extended MultiBinding (Allow to make nested hierarchy of bindings and markupextensions)
    /// </summary>
    [ContentProperty(nameof(Children))]
    [ContentWrapper(typeof(MarkupExtension))]
    public class XMultiBinding
        : MarkupExtension
    {
        #region Constructor and ManageArgs

        public XMultiBinding()
        { }

        public XMultiBinding(MarkupExtension markup1) => Children.Add(markup1);

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2)
            : this(markup1) => Children.Add(markup2);

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3)
            : this(markup1, markup2) => Children.Add(markup3);

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4)
            : this(markup1, markup2, markup3) => Children.Add(markup4);

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5)
            : this(markup1, markup2, markup3, markup4) => Children.Add(markup5);

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6)
            : this(markup1, markup2, markup3, markup4, markup5) => Children.Add(markup6);

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7)
            : this(markup1, markup2, markup3, markup4, markup5, markup6) => Children.Add(markup7);

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7) => Children.Add(markup8);

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8) => Children.Add(markup9);
        
        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8, MarkupExtension markup9, MarkupExtension markup10)
            : this(markup1, markup2, markup3, markup4, markup5, markup6, markup7, markup8, markup9) => Children.Add(markup10);

        #endregion

        public Collection<MarkupExtension> Children { get; } = new Collection<MarkupExtension>();

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

            return "Hye";
        }
    }
}
