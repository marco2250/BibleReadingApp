using System.Data;

using BibleReading.Model;

namespace BibleReading.DAL
{
    public class VerseDAL : BaseDAL
    {
        public DataTable GetVerseByBook(BookEnum book)
        {
            return GetDataTable("spVerseGetByBook", "BookId", book);
        }
    }
}
