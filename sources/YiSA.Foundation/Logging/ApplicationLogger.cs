using System;
using System.IO;
using YiSA.Foundation.Common;

namespace YiSA.Foundation.Logging
{
    public class ApplicationLogger : CompositeLogger
    {
        private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        
        public ApplicationLogger(string applicationName, FileLoggerOption? option = null)
        {
            var loggingDirectory = Path.Combine(AppData,applicationName,"Logs");
            var logFileName = $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.log";

            Directory.CreateDirectory(loggingDirectory);
            this.Add(new FileLogger(Path.Combine(loggingDirectory, logFileName), option));
            if(Debugging.True)
                this.Add(new ConsoleLogger());
        }
    }
}