using System.Data;

using BibleReading.DAL;

namespace BibleReading.BS
{
    public class User
    {
        public DataTable GetUserById(int userId)
        {
            return new UserDAL().GetUserById(userId);
        }

        public void UpdateNextVerseId(int userId, int nextVerseId)
        {
            new UserDAL().UpdateNextVerseId(userId, nextVerseId);
        }
    }
}
