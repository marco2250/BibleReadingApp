using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BibleReading.Common45.Root.Reflection
{
    public static class ReflectionExtension
    {
        public static bool IsNullable(this PropertyInfo p)
        {
            return p.PropertyType.IsNullable();
        }

        public static bool IsNullable(this Type t)
        {
            return !t.IsValueType || Nullable.GetUnderlyingType(t) != null;
        }
    }
}
