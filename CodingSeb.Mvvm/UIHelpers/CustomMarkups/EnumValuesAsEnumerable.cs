using System;
using System.Windows.Markup;

namespace CodingSeb.Mvvm.UIHelpers
{
    /// <summary>
    /// A MarkupExtention to easily get an enumerable of all values of the specified enum type.
    /// Useful to populate (By the DependencyProperty "ItemsSource") an ItemsControl
    /// </summary>
    public class EnumValuesAsEnumerable : MarkupExtension
    {
        private Type enumType;

        /// <summary>
        /// The type of the enum to enumerate
        /// </summary>
        public Type EnumType
        {
            get { return enumType; }
            set
            {
                if (!value.IsEnum)
                    throw new Exception($"The type \"{value.Name}\" is not an enum");

                enumType = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
