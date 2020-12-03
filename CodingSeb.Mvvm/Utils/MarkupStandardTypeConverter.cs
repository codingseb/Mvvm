using System.ComponentModel;
using System.Windows;

namespace CodingSeb.Mvvm
{
    internal static class MarkupStandardTypeConverter
    {
        public static object ConvertValueForDependencyProperty(object value, DependencyProperty dependencyProperty)
        {
            if(!dependencyProperty.PropertyType.IsAssignableFrom(value.GetType()))
            {
                try
                {
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(dependencyProperty.PropertyType);

                    if (typeConverter?.CanConvertFrom(value.GetType()) ?? false)
                    {
                        return typeConverter.ConvertFrom(value);
                    }
                }
                catch { }

                try
                {
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(value.GetType());

                    if (typeConverter?.CanConvertTo(dependencyProperty.PropertyType) ?? false)
                    {
                        return typeConverter.ConvertTo(value, dependencyProperty.PropertyType);
                    }
                }
                catch { }

                if(dependencyProperty.PropertyType == typeof(string))
                {
                    return value.ToString();
                }
                else if(value.GetType() != typeof(string))
                {
                    try
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(dependencyProperty.PropertyType);

                        if (typeConverter?.CanConvertFrom(typeof(string)) ?? false)
                        {
                            return typeConverter.ConvertFrom(value.ToString());
                        }
                    }
                    catch { }

                    try
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(string));

                        if (typeConverter?.CanConvertTo(dependencyProperty.PropertyType) ?? false)
                        {
                            return typeConverter.ConvertTo(value.ToString(), dependencyProperty.PropertyType);
                        }
                    }
                    catch { }
                }
            }

            return value;
        }
    }
}
