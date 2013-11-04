using System.Data;

namespace BibleReading.DAL
{
    public class BookDAL : BaseDAL
    {
        public DataTable GetBook()
        {
            return GetDataTable("spBookGet");
        }
    }
}
