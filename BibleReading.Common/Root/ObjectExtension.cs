using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibleReading.Common45.Root
{
    public static class ObjectExtension
    {
        #region Conversion

        public static int ToInt32(this object o)
        {
            return System.Convert.ToInt32(o);
        }

        public static T CastMe<T>(this object o)
        {
            return ObjectUtility.CastMe<T>(o);
        }

        #endregion
    }
}
