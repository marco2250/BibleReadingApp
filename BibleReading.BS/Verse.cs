using System.Data;
using BibleReading.DAL;
using BibleReading.Model;

namespace BibleReading.BS
{
    public class Verse
    {
        public DataTable GetVerseByBook(BookEnum book)
        {
            return new VerseDAL().GetVerseByBook(book);
        }
    }
}
