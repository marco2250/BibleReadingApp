using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BibleReading.DAL
{
    public class ReadingDAL : BaseDAL
    {
        public int InsertReading(DateTime startedAt, 
            DateTime finishedAt, 
            int verseIdFrom, 
            int verseIdTo, 
            int totalSeconds, 
            int wordsPerMinute, 
            int totalVerses, 
            int totalWods, 
            int userId)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(GetSqlParameter("StartedAt", startedAt));
            parameters.Add(GetSqlParameter("FinishedAt", finishedAt));
            parameters.Add(GetSqlParameter("VerseIdFrom", verseIdFrom));
            parameters.Add(GetSqlParameter("VerseIdTo", verseIdTo));
            parameters.Add(GetSqlParameter("TotalSeconds", totalSeconds));
            parameters.Add(GetSqlParameter("WordsPerMinute", wordsPerMinute));
            parameters.Add(GetSqlParameter("TotalVerses", totalVerses));
            parameters.Add(GetSqlParameter("TotalWords", totalWods));
            parameters.Add(GetSqlParameter("UserId", userId));

            return ExecuteScalar<int>("spReadingInsert", CommandType.StoredProcedure, parameters);
        }
    }
}
