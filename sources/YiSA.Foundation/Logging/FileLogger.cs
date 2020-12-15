using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YiSA.Foundation.Common;
using YiSA.Foundation.Common.Extensions;
using YiSA.Foundation.Internal;

namespace YiSA.Foundation.Logging
{
    public class FileLogger : DisposableHolder , ILogger
    {
        private readonly CancellationTokenSource _cancellationToken;
        private readonly string _absoluteFilePath;
        private readonly int _writeTimeSpanMs;
        private int _startedWriteQueueThread;
        private ConcurrentQueue<FileLoggerLogInfo> _queue = new ConcurrentQueue<FileLoggerLogInfo>();
        private Func<string, LogLevel, DateTime,string> _loggingFormatFunction;
        
        public FileLogger(string absoluteFilePath , int writeSpanMs = 2000)
        {
            Debug.Assert(string.IsNullOrWhiteSpace(absoluteFilePath));
            _absoluteFilePath = absoluteFilePath;
            _writeTimeSpanMs = writeSpanMs;
            _loggingFormatFunction = (msg, lv, dt) => $"{dt}:{lv}:{msg}";
            _cancellationToken = new CancellationTokenSource()
                .DisposeBy(Disposables);
        }

        public void OverrideLoggingFormat(Func<string, LogLevel, DateTime,string> formatter)
        {
            _loggingFormatFunction = formatter;
        }
        
        public void WriteLine(string message, LogLevel logLevel)
        {
            if (_startedWriteQueueThread is 0)
            {
                Interlocked.Increment(ref _startedWriteQueueThread);

                if (_startedWriteQueueThread is 1)
                {
                    _ = _startWriteToFileThread(_cancellationToken.Token);                    
                }
            }
            _queue.Enqueue(new FileLoggerLogInfo(message,logLevel,DateTime.Now));
        }

        private async Task _startWriteToFileThread(CancellationToken cancellationToken)
        {
            FileSystemUtility.TryCreateParentDirectory(_absoluteFilePath);

            IScheduler a;
            
            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(_writeTimeSpanMs, cancellationToken);

                    if (_cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    WriteQueueToFile();

                }
            }, cancellationToken);
        }

        private void WriteQueueToFile()
        {
            if(_queue.IsEmpty)
                return;

            var list = new List<string>();
            while (_queue.Any())
            {
                if(_queue.TryDequeue(out var info))
                    list.Add(_loggingFormatFunction(info.Message,info.LogLevel,info.DateTime));
            }
            
            using var mutex = new FileSystemMutex(_absoluteFilePath);
            File.AppendAllLines(_absoluteFilePath,list);
        }
    }

    internal class FileLoggerLogInfo
    {
        public FileLoggerLogInfo(string message, LogLevel logLevel,DateTime dateTime)
        {
            Message = message;
            LogLevel = logLevel;
            DateTime = dateTime;
        }

        public string Message { get; } 
        public DateTime DateTime { get; }
        public LogLevel LogLevel { get; }
    }
}