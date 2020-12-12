using System.IO;
using YiSA.Foundation.Common.Extensions;
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

            Directory.EnumerateFiles(sourceAbsolutePath, "*", SearchOption.TopDirectoryOnly)
                .ForEach(x=>File.Copy(x,Path.Combine(destAbsolutePath,Path.GetFileName(x)),overwrite));
            
            Directory.EnumerateDirectories(sourceAbsolutePath, "*", SearchOption.TopDirectoryOnly)
                .ForEach(x=>CopyDirectory(x,Path.Combine(destAbsolutePath,Path.GetFileName(x)),overwrite));
            return true;
        }
    }
}