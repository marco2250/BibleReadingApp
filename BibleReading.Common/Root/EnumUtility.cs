using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace BibleReading.Common45.Root
{
    public static class Enums
    {
        public static IEnumerable<int> GetToInt<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<int>();
        }

        public static IEnumerable<T> Get<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
