using System;
using System.Collections.ObjectModel;
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

        public XMultiBinding(MarkupExtension markup1)
        {
            Children.Add(markup1);
        }

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2)
        {
            Children.Add(markup1);
            Children.Add(markup2);
        }

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3)
        {
            Children.Add(markup1);
            Children.Add(markup2);
            Children.Add(markup3);
        }

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4)
        {
            Children.Add(markup1);
            Children.Add(markup2);
            Children.Add(markup3);
            Children.Add(markup4);
        }

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5)
        {
            Children.Add(markup1);
            Children.Add(markup2);
            Children.Add(markup3);
            Children.Add(markup4);
            Children.Add(markup5);
        }

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6)
        {
            Children.Add(markup1);
            Children.Add(markup2);
            Children.Add(markup3);
            Children.Add(markup4);
            Children.Add(markup5);
            Children.Add(markup6);
        }

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7)
        {
            Children.Add(markup1);
            Children.Add(markup2);
            Children.Add(markup3);
            Children.Add(markup4);
            Children.Add(markup5);
            Children.Add(markup6);
            Children.Add(markup7);
        }

        public XMultiBinding(MarkupExtension markup1, MarkupExtension markup2, MarkupExtension markup3, MarkupExtension markup4, MarkupExtension markup5, MarkupExtension markup6, MarkupExtension markup7, MarkupExtension markup8)
        {
            Children.Add(markup1);
            Children.Add(markup2);
            Children.Add(markup3);
            Children.Add(markup4);
            Children.Add(markup5);
            Children.Add(markup6);
            Children.Add(markup7);
            Children.Add(markup8);
        }

        #endregion

        public Collection<MarkupExtension> Children { get; } = new Collection<MarkupExtension>();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return "Hye";
        }
    }
}
