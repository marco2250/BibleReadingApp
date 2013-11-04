using System;

using BibleReading.DAL;

namespace BibleReading.BS
{
    public class Reading
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
            return new ReadingDAL().InsertReading(startedAt, 
                finishedAt, 
                verseIdFrom, 
                verseIdTo, 
                totalSeconds, 
                wordsPerMinute, 
                totalVerses, 
                totalWods, userId);
        }
    }
}
