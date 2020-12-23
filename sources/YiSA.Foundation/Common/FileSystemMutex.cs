using System;
using System.Threading;

namespace YiSA.Foundation.Common
{
    /// <summary>
    /// ファイルパスに対するミューテックスを作成し、同一ファイルへのアクセスを抑制します。
    /// Dispose時にMutexを開放します。
    /// example
    ///   using var mutex = new FileSystemMutex(filePath);
    ///   File.AppendAllLines(filePath,...);
    /// </summary>
    public class FileSystemMutex : IDisposable
    {
        private readonly Mutex _mutex;
        public FileSystemMutex(string absolutePath)
        {
            _mutex = new Mutex(false,$"YiSA:{absolutePath.GetHashCode()}");
            _mutex.WaitOne();
        }

        public void Dispose()
        {
            _mutex.ReleaseMutex();
            _mutex.Dispose();
        }
    }
}