using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BibleReading.Common45.Root.IO
{
    public class StreamWriterUtility
    {
        public static void WriteToFile(string pathFile, string message)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(pathFile, true))
                {
                    sw.WriteLine(message);
                    sw.Close();
                }
            }
            catch (Exception ex) { }
        }
    }
}
