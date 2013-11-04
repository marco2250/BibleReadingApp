using System;
using System.IO;

namespace BibleReading.Common45.Root.IO
{
    public static class FileInfoExtension
    {
        public static void RenameWithNewExtension(this FileInfo fInfo, string newExt)
        {
            RenameWithNewExtensionAndSuffix(fInfo, null, newExt);
        }

        public static void RenameWithNewExtensionAndSuffix(this FileInfo fInfo, string newExt, string suffix)
        {
            string newName = GetFileNameWithoutExtension(fInfo);

            if (!suffix.IsNullOrEmpty())
                newName += suffix + newExt;
            else
                newName += newExt;

            string pathDest = fInfo.FullName.Replace(fInfo.Name, string.Empty);

            if (!pathDest.EndsWith("\\"))
                pathDest += "\\";
            
            fInfo.MoveTo(pathDest + newName);
        }

        public static void MoveWithNewExtension(this FileInfo fInfo, string pathDest, string newExt)
        {
            string newName = GetFileNameWithoutExtension(fInfo);
            newName += newExt;

            if (!pathDest.EndsWith("\\"))
                pathDest += "\\";

            fInfo.MoveTo(pathDest + newName);
        }

        public static void MoveWithNewName(this FileInfo fInfo, string pathDest, string newName)
        {
            if (!pathDest.EndsWith("\\"))
                pathDest += "\\";

            fInfo.MoveTo(pathDest + newName + fInfo.Extension);
        }

        public static void MoveWithSameExtension(this FileInfo fInfo, string pathDest)
        {
            string newName = fInfo.Name;

            if (!pathDest.EndsWith("\\"))
                pathDest += "\\";

            fInfo.MoveTo(pathDest + newName);
        }

        public static string GetFileNameWithoutExtension(this FileInfo fInfo)
        {
            string fileName = fInfo.Name;
            fileName = fileName.Substring(0, fileName.LastIndexOf(fInfo.Extension, StringComparison.Ordinal)); // Retira a extensão

            return fileName;
        }

        public static string GetNewFileNameIfExists(this FileInfo fInfo, string path)
        {
            var fileName = fInfo.Name;
            var originalFileName = fileName;
            var i = 0;
            while (new FileInfo(path + "\\" + fileName).Exists)
            {
                i++;
                fileName = GetFileNameWithoutExtension(new FileInfo(originalFileName)) + "_" + i + new FileInfo(originalFileName).Extension;
            }
            return fileName; 
        }
    }
}
