using System.Data;
using BibleReading.DAL;

namespace BibleReading.BS
{
    public class Book
    {
        public DataTable GetBook()
        {
            return new BookDAL().GetBook();
        }

    }
}
