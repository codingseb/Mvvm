using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace CodingSeb.Mvvm
{
    internal static class MarkupStandardTypeConverter
    {
        private static readonly Dictionary<Type, Dictionary<Type, Func<object, object>>> extendedConvertions = new Dictionary<Type, Dictionary<Type, Func<object, object>>>()
        {
            {
                typeof(bool), new Dictionary<Type, Func<object, object>>
                {
                    {typeof(int), source => (bool)source ? 1 : 0 },
                    {typeof(int?), source => (bool)source ? 1 : 0 },
                    {typeof(Visibility), source => (bool)source ? Visibility.Visible : Visibility.Collapsed },
                }
            },
            {
                typeof(int), new Dictionary<Type, Func<object, object>>
                {
                    {typeof(bool), source => (int)source > 0 },
                    {typeof(bool?), source => (int)source > 0 },
                }
            },
        };

        public static object ConvertValueForDependencyProperty(object value, DependencyProperty dependencyProperty, bool standardConvertOnly = false)
        {
            Type sourceType = value?.GetType();
            Type targetType = dependencyProperty.PropertyType;

            if (sourceType == null)
                return value;

            if (!standardConvertOnly)
            {
                if (extendedConvertions.ContainsKey(sourceType) && extendedConvertions[sourceType].ContainsKey(targetType))
                {
                    return extendedConvertions[sourceType][targetType](value);
                }
            }

            if (!targetType.IsAssignableFrom(sourceType))
            {
                try
                {
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(targetType);

                    if (typeConverter?.CanConvertFrom(sourceType) ?? false)
                    {
                        return typeConverter.ConvertFrom(value);
                    }
                }
                catch { }

                try
                {
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(sourceType);

                    if (typeConverter?.CanConvertTo(targetType) ?? false)
                    {
                        return typeConverter.ConvertTo(value, targetType);
                    }
                }
                catch { }

                if(targetType == typeof(string))
                {
                    return value.ToString();
                }
                else if(sourceType != typeof(string))
                {
                    try
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(targetType);

                        if (typeConverter?.CanConvertFrom(typeof(string)) ?? false)
                        {
                            return typeConverter.ConvertFrom(value.ToString());
                        }
                    }
                    catch { }

                    try
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(string));

                        if (typeConverter?.CanConvertTo(targetType) ?? false)
                        {
                            return typeConverter.ConvertTo(value.ToString(), targetType);
                        }
                    }
                    catch { }
                }
            }

            return value;
        }
    }
}
