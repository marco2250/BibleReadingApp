using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using BibleReading.Common45.Root.IO;

using BibleReading.Common45.Properties;

namespace BibleReading.Common45.Root.Diagnostics.Logging
{
    public class LoggerUtility
    {
        public static void WriteToEventViewer(string source
            , string log
            , string evtMsg
            , EventLogEntryType type
            , bool eventLogEnabled
            , bool ignoreDisabled)
        {
            try
            {
                if (eventLogEnabled || ignoreDisabled)
                {
                    if (!EventLog.SourceExists(source))
                        EventLog.CreateEventSource(source, log);

                    EventLog.WriteEntry(source, evtMsg, type);
                }
            }
            catch (Exception ex) { }
        }


        public static void WriteToFile(string filePath, bool eventLogEnabled, string source, string message, EventLogEntryType type)
        {
            try
            {
                if (eventLogEnabled)
                {
                    string fullMessage = string.Format("{0:dd/MM/yyyy HH:mm:ss} - {1} => {2} - {3}", DateTime.Now, type, source, message);

                    StreamWriterUtility.WriteToFile(filePath, fullMessage);
                }
            }
            catch (Exception ex) { }
        }
    }
}
