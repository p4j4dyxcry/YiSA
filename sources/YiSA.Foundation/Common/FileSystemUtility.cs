using System.IO;
using YiSA.Foundation.Internal;

namespace YiSA.Foundation.Common
{
    public static class FileSystemUtility
    {
        public static bool TryFileExists(string absoluteFilePath)
        {
            return TryCatchUtility.TryInvoke(
                () => File.Exists(absoluteFilePath),false);
        }
        
        public static bool TryCreateParentDirectory(string absoluteFilePath)
        {
            return TryCatchUtility.TryInvoke(
                () =>
                {
                    Directory.CreateDirectory(Directory.GetParent(absoluteFilePath).FullName);
                    return true;
                },false);
        }

        public static bool CopyDirectory(string sourceAbsolutePath, string destAbsolutePath, bool overwrite)
        {
            if (TryCreateParentDirectory(destAbsolutePath) is false)
                return false;

            foreach (var file in Directory.EnumerateFiles(sourceAbsolutePath, "*", SearchOption.TopDirectoryOnly))
            {
                File.Copy(file, Path.Combine(destAbsolutePath, Path.GetFileName(file)), overwrite);
            }

            foreach (var dir in Directory.EnumerateFiles(sourceAbsolutePath, "*", SearchOption.TopDirectoryOnly))
            {
                CopyDirectory(dir, Path.Combine(destAbsolutePath, Path.GetFileName(dir)), overwrite);
            }
            
            return true;
        }
    }
}