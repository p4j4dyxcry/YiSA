using System;
using System.Threading;

namespace YiSA.Foundation.Common
{
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