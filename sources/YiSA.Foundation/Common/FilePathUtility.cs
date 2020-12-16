using System.IO;

namespace YiSA.Foundation.Common
{
    public static class FilePathUtility
    {
        /// <summary>
        /// 拡張子を除いたファイル名を変更します。
        /// example
        ///  x = ChangeFileNameWithOutExtension( "C:\\test.txt" , "test2" )
        ///  x is "C:\\test2.txt"
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newNameWithOutExtension"></param>
        /// <returns></returns>
        public static string RenameWithOutExtension(string path,string newNameWithOutExtension)
        {
            var lastSeparatorIndex = path.Replace('\\', '/').LastIndexOf('/');

            if (lastSeparatorIndex < 0)
            {
                return $"{path}{Path.GetExtension(path)}";
            }

            var parentDirectory = path.Substring(0, lastSeparatorIndex);
            return $"{parentDirectory}{newNameWithOutExtension}{Path.GetExtension(path)}";
        }
        
    }
}