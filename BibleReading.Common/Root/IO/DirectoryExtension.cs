using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BibleReading.Common45.Root.IO
{
    public static class DirectoryExtension
    {
        #region GetFilesRecursive

        public static List<string> GetFilesRecursive(this DirectoryInfo directory, bool exactExt)
        {
            return GetFilesRecursive(directory, null, exactExt);
        }

        public static List<string> GetFilesRecursive(this DirectoryInfo directory, string match, bool exactExt)
        {
            // 1.
            // Store results in the file results list.
            var result = new List<string>();

            // 2.
            // Store a stack of our directories.
            var stack = new Stack<string>();

            // 3.
            // Add initial directory.
            stack.Push(directory.FullName);

            // 4.
            // Continue while there are directories to process
            while (stack.Count > 0)
            {
                // A.
                // Get top directory
                string dir = stack.Pop();

                try
                {
                    // B
                    // Add all files at this directory to the result List.
                    if (string.IsNullOrEmpty(match))
                        result.AddRange(Directory.GetFiles(dir, "*.*"));
                    else
                    {
                        var aMatch = match.Split(';');

                        if (exactExt)
                            foreach (var s in aMatch)
                                result.AddRange(new DirectoryInfo(dir).GetFiles(s).Where(x => x.Extension.ToUpper() == s.Replace("*", string.Empty).ToUpper()).Select(y => y.FullName));
                        else
                            foreach (var s in aMatch)
                                result.AddRange(Directory.GetFiles(dir, s));
                    }

                    // C
                    // Add all directories at this directory.
                    foreach (string dn in Directory.GetDirectories(dir))
                    {
                        stack.Push(dn);
                    }
                }
                catch
                {
                    // D
                    // Could not open the directory
                }
            }

            return result;
        }

        #endregion

        public static void DeleteFilesByExtension(this DirectoryInfo directory, string extensions)
        {
            var files = directory.GetFilesRecursive(extensions, true);

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                }
            }
        }
    }
}
