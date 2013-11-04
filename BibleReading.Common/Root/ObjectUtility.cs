// Based on http://stackoverflow.com/questions/3531318/convert-changetype-fails-on-nullable-types

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BibleReading.Common45.Root.Reflection;

namespace BibleReading.Common45.Root
{
    public class ObjectUtility
    {
        public static T CastMe<T>(object value)
        {
            var type = typeof(T);

            if (/*type.IsNullable() && */(value == null || value == DBNull.Value))
                return default(T);

            Type underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? type;

            try
            {
                // Just one edge case you might want to handle.
                if (underlyingType == typeof(Guid))
                {
                    if (value is string)
                    {
                        value = new Guid(value as string);
                    }
                    if (value is byte[])
                    {
                        value = new Guid(value as byte[]);
                    }

                    return (T)Convert.ChangeType(value, underlyingType);
                }

                return (T)Convert.ChangeType(value, underlyingType);
            }
            catch (Exception ex)
            {
                throw new ArgumentException();
            }
        }
    }
}
