using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BibleReading.DAL
{
    public class UserDAL : BaseDAL
    {
        public DataTable GetUserById(int userId)
        {
            return GetDataTable("spUserGetById", "UserId", userId);
        }

        public void UpdateNextVerseId(int userId, int nextVerseId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetSqlParameter("UserId", userId));
            parameters.Add(GetSqlParameter("NextVerseId", nextVerseId));

            ExecuteNonQuery("spUserUpdateNextVerseId", parameters);
        }
    }
}
