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
        private readonly string _absoluteFilePath;
        private readonly FileLoggerOption _option;
        private readonly ConcurrentQueue<(string,LogLevel,DateTime)> _queue = new ConcurrentQueue<(string,LogLevel,DateTime)>();
        private Task? _currentWriteFileTask;
        
        public FileLogger(string absoluteFilePath , FileLoggerOption? option)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(absoluteFilePath));
            _absoluteFilePath = absoluteFilePath;
            _option = option ?? FileLoggerOption.Default;
            
            Disposables.Add(InternalDisposable.Make(WaitForWriteAsync));
        }
        
        public void WriteLine(string message, LogLevel logLevel)
        {
            if(logLevel < _option.LoggingLevel)
                return;
            
            if (_option.IsAsync)
                _ = WriteLineToQueue(message,logLevel,DateTime.Now);
            else
                WriteLineToFile(message,logLevel,DateTime.Now);
        }
        
        private void WriteLineToFile(string message, LogLevel logLevel,DateTime dateTime)
        {
            WriteLinesToFile(new []{(message,logLevel,dateTime)});
        }
        
        private async Task WriteLineToQueue(string message , LogLevel logLevel,DateTime dateTime)
        {
            _queue.Enqueue((message,logLevel,dateTime));

            if (_currentWriteFileTask is {})
                return;

            _currentWriteFileTask = Task.Run(async () =>
            {
                await Task.Delay(_option.AsyncLoggingInterval);

                var list = new List<(string,LogLevel,DateTime)>();
                while (_queue.Any())
                {
                    if(_queue.TryDequeue(out var info))
                        list.Add((info.Item1,info.Item2,info.Item3));
                }
                WriteLinesToFile(list);
            });
            await _currentWriteFileTask;
            _currentWriteFileTask = null;
        }

        private void WaitForWriteAsync()
        {
            if (_option.IsAsync && _queue.Any())
            {
                while (_queue.Any())
                {
                    Thread.Sleep(10);
                }
            }
        }

        private void WriteLinesToFile(IEnumerable<(string message, LogLevel logLevel, DateTime dateTime)>lines)
        {
            var stringLines = lines.Select(x=>_option.LoggingFormatter(x.message,x.logLevel,x.dateTime));

            var filePath = _absoluteFilePath;
            var fileName = Path.GetFileName(filePath);
            int index = 2;
            while (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length < _option.MaximumFileSizeByte)
                    break;
                filePath = FilePathUtility.RenameWithOutExtension(_absoluteFilePath,$"{fileName}{index++}");
            }
            
            using var mutex = new FileSystemMutex(filePath);
            File.AppendAllLines(filePath,stringLines);
        }
    }
    
    public class FileLoggerOption
    {
        public bool IsAsync { get;private set; }
        public TimeSpan AsyncLoggingInterval { get;private set; }
        public long MaximumFileSizeByte { get;private set; }
        public Func<string, LogLevel, DateTime,string> LoggingFormatter { get; set; }
        public LogLevel LoggingLevel { get; private set; }

        private FileLoggerOption(FileLoggerOption? option)
        {
            if (option != null)
            {
                IsAsync = option.IsAsync;
                AsyncLoggingInterval = option.AsyncLoggingInterval;
                MaximumFileSizeByte = option.MaximumFileSizeByte;
                LoggingFormatter = option.LoggingFormatter;
                LoggingLevel = option.LoggingLevel;
            }
            else
            {
                LoggingFormatter = DefaultLoggingFormat;
            }
        }
        
        public static FileLoggerOption Default { get; } 
            = new FileLoggerOption(default)
            {
                IsAsync = true,
                AsyncLoggingInterval = TimeSpan.FromMilliseconds(2000),
                MaximumFileSizeByte = 100 * 1024 * 1024 , // 100MB
            };
        private static string DefaultLoggingFormat(string msg, LogLevel lv,DateTime dt)  => $"{dt:yyyy/MM/dd/HH:mm:ss}:{lv}:{msg}";

        public FileLoggerOption WithLoggingFormatter(Func<string, LogLevel, DateTime, string> formatter)
        {
            return new FileLoggerOption(this)
            {
                LoggingFormatter = formatter
            };
        }
        
        public FileLoggerOption WithSync()
        {
            return new FileLoggerOption(this)
            {
                IsAsync = false,
            };
        }   
        
        public FileLoggerOption WithLoggingAsyncLoggingInterval(TimeSpan interval)
        {
            return new FileLoggerOption(this)
            {
                IsAsync = true,
                AsyncLoggingInterval = interval,
            };
        }    
        
        public FileLoggerOption WithLoggingMaximumFileSizeByte(long szByte)
        {
            return new FileLoggerOption(this)
            {
                MaximumFileSizeByte = szByte,
            };
        }        
        
        public FileLoggerOption WithLoggingLevel(LogLevel level)
        {
            return new FileLoggerOption(this)
            {
                LoggingLevel = level,
            };
        }
        
    }
}