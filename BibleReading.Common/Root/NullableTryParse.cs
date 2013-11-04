using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace BibleReading.Common45.Root
{
    public static class NullableTryParse
    {
        public static bool TryParseStruct<T>(this string value, out Nullable<T> result)
            where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                result = new Nullable<T>();

                return true;
            }

            result = default(T);
            try
            {
                IConvertible convertibleString = (IConvertible)value;
                result = new Nullable<T>((T)convertibleString.ToType(typeof(T), System.Globalization.CultureInfo.CurrentCulture));
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }

        public static Nullable<T> TryParseStruct<T>(string input)
                where T : struct
        {
            Nullable<T> result = new Nullable<T>();

            if (input.IsNullOrEmpty())
                return result;
            
            try
            {
                if (!typeof(T).IsEnum)
                {
                    IConvertible convertibleString = (IConvertible)input;
                    result = new Nullable<T>((T)convertibleString.ToType(typeof(T), CultureInfo.CurrentCulture));
                }
                else
                {
                    if (Enums.GetToInt<T>().Contains(int.Parse(input)))
                    {
                        result = input.ToEnum<T>();
                    }
                    else
                        return result;
                }
            }
            catch (InvalidCastException)
            {

            }
            catch (FormatException)
            {

            }

            return result;
        }

        public static string TryParseString(object input)
        {
            if (input == null || input is DBNull)
                return string.Empty;
            else
                return input.ToString();
        }

        public static Nullable<T> TryParseDBNullValue<T>(this object obj)
            where T : struct
        {
            if (DBNull.Value.Equals(obj))
            {
                return null;
            }

            return (T)obj;
        }

        public static T TryParseDBNullReference<T>(this object obj)
            where T : class
        {
            if (DBNull.Value.Equals(obj))
            {
                return default(T);
            }
            else
            {
                return (T)obj;
            }
        }
    }
}
