using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace BibleReading.Common45.Root.Diagnostics
{
    public class ProcessUtility
    {
        public static Process GetProcess(string processName)
        {
            return GetProcess(processName, true);
        }

        public static Process GetProcess(string processName, bool removeExt)
        {
            if (removeExt)
            {
                processName = Path.GetFileNameWithoutExtension(processName);
            }

            Process[] aProc = Process.GetProcessesByName(processName);

            if (aProc.Length > 0)
                return aProc[0];
            else
                return null;
        }

        public static void KillProcess(string processName)
        {
            KillProcess(processName, false, null);
        }

        public static void KillProcess(string processName, bool waitForExit)
        {
            KillProcess(processName, waitForExit, null);
        }

        public static void KillProcess(string processName, bool waitForExit, Nullable<int> waitForExitMilliseconds)
        {
            Process process = GetProcess(processName);

            if (process != null)
            {
                process.Kill();

                if (waitForExit)
                {
                    if (waitForExitMilliseconds == null)
                        process.WaitForExit();
                    else
                        process.WaitForExit(waitForExitMilliseconds.Value);
                }
            }
        }
    }
}
