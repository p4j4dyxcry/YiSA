using System.Diagnostics;
using YiSA.Foundation.Logging;

namespace YiSA.Foundation.Common
{
    /// <summary>
    /// 外部プロセスに対する操作のUtilityです。
    /// </summary>
    public class ProcessUtility
    {
        /// <summary>
        /// 標準出力をリダイレクトしつつプロセスを開始します。
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
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

        /// <summary>
        /// cmd経由でプロセスを開始します。
        /// これによって開始したプロセスは子プロセスではなくなるのでプロセス間の親子関係が切れます。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="args"></param>
        /// <param name="workingDirectory"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// 標準出力をLoggerに関連付けて作成します。
        /// この関数で生成されたプロセスはLoggerに出力がリダイレクトされます。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="args"></param>
        /// <param name="workingDirectory"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
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

        /// <summary>
        /// プロセスの出力にロガーを関連付けます。
        /// プロセスは開始前である必要があります。
        /// </summary>
        /// <param name="process"></param>
        /// <param name="logger"></param>
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