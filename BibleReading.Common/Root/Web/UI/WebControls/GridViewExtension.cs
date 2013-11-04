using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web.UI.WebControls;

namespace BibleReading.Common45.Root.Web.UI.WebControls
{
    public static class GridViewExtension
    {
        public static GridViewRow GetRowByDataKeyValue(this GridView gv, string key)
        {
            return gv.Rows.Cast<GridViewRow>().ToList().Where(x => gv.DataKeys[x.RowIndex].Value.ToString() == key).FirstOrDefault();
        }
    }
}
