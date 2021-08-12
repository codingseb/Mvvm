using System;
using System.Windows.Markup;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// An Markup to easily provide a inline string as binding source
    /// </summary>
    public class StringSource : MarkupExtension
    {
        public StringSource()
        {}

        public StringSource(string stringValue)
        {
            StringValue = stringValue;
        }

        public string StringValue { get; set; } = string.Empty;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return StringValue;
        }
    }
}
