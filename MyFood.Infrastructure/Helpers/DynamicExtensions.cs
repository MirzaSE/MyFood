using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace MyFood.Infrastructure.Models
{
    public static class DynamicExtensions
    {
        public static ExpandoObject ToDynamic(this object value)
        {
            ArgumentNullException.ThrowIfNull(value);

            ExpandoObject expando = new();
            var dict = (IDictionary<string, object?>)expando;

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
            {
                dict[property.Name] = property.GetValue(value);
            }

            return expando;
        }
    }
}