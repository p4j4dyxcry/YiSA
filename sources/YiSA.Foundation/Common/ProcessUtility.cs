using System.Diagnostics;
using YiSA.Foundation.Logging;

namespace YiSA.Foundation.Common
{
    public class ProcessUtility
    {
        public bool StartAndReadCommandLine(Process process)
        {
            if (process.Start() is false)
                return false;
            
            if (process.StartInfo.UseShellExecute)
            {
                return true;
            }
            
            if (process.StartInfo.RedirectStandardOutput)
            {
                process.BeginOutputReadLine();
            }
            if (process.StartInfo.RedirectStandardError)
            {
                process.BeginErrorReadLine();
            }

            return true;
        }

        public Process CreateWithCmd(
            string filePath,
            string args,
            string workingDirectory,
            ILogger? logger = null)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    WorkingDirectory = workingDirectory,
                    Arguments = $"/c {filePath} {args}",
                    UseShellExecute = false
                }
            };

            if (logger != null)
            {
                BindLogger(process,logger);
            }

            return process;
        }
        
        public Process CreateForLogging(
            string filePath,
            string args,
            string workingDirectory,
            ILogger? logger = null)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = filePath,
                    WorkingDirectory = workingDirectory,
                    Arguments = args,
                    UseShellExecute = false
                }
            };

            if (logger != null)
            {
                BindLogger(process,logger);
            }

            return process;
        }

        private void BindLogger(Process process, ILogger logger)
        {
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(e.Data) is false)
                {
                    logger.WriteLine(e.Data,LogLevel.Default);
                }
            };
                
            process.ErrorDataReceived += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(e.Data) is false)
                {
                    logger.WriteLine(e.Data,LogLevel.Error);
                }
            };
        }
    }
}