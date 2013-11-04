using System.Web.UI;

namespace BibleReading.Common45.Root.Web.UI
{
    public static class ControlExtension
    {
        public static T FindControl<T>(this Control ctl, string id) where T : Control
        {
            if (ctl.FindControl(id) == null)
                return null;
            
            return (T)ctl.FindControl(id);
        }
    }
}
