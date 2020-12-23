using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using Xunit.Abstractions;
using YiSA.Foundation.Logging;

namespace YiSA.Test
{
    /// <summary>
    /// テスト用のベースクラスです。
    /// ロギングの整形や、出力レベル、時間計測等のUtilityがあります。
    /// 利用は必須ではありません。
    /// </summary>
    public class TestBase
    {
        /// <summary>
        /// 出力用のロガーです。
        /// ITestOutputHelperにリダイレクトされます。
        /// </summary>
        protected ILogger Logger { get; }
        
        private readonly ITestOutputHelper _helper;
        private int _loggingIndent = 0;
        private bool _isBuffering = false;
        private ConcurrentQueue<string> _buffer = new ConcurrentQueue<string>();

        /// <summary>
        /// コンストラクト
        /// </summary>
        /// <param name="helper"></param>
        protected TestBase(ITestOutputHelper helper)
        {
            _helper = helper;
            Logger = new DelegateLogger((s, e) =>
            {
                if(e == LogLevel.Debug && IsDebugging)
                    return;
                
                var sb = new StringBuilder();
                for (int i = 0; i < _loggingIndent * 2; ++i)
                    sb.Append(' ');

                // 警告以上の場合
                if (e >= LogLevel.Warning)
                    sb.Append($"[{e}]");

                sb.Append(s);

                if (_isBuffering)
                    _buffer.Enqueue(sb.ToString());
                else
                    helper.WriteLine(sb.ToString());
            });
        }

        /// <summary>
        /// 以降の出力ログをインデントします。
        /// </summary>
        /// <returns>Disposeするとインデントが戻ります。</returns>
        protected IDisposable IncrementLogIndent()
        {
            Interlocked.Increment(ref _loggingIndent);
            return Disposable.Create(() =>
            {
                Interlocked.Decrement(ref _loggingIndent);                
            });
        }

        /// <summary>
        /// 区間にかかった時間を計測します。
        /// example
        ///  using( ProfileTime("any"))
        ///  {
        ///     // ...
        ///  }
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Disposeすると計測を終了します</returns>
        protected IDisposable ProfileTime(string message)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.WriteLine(message,LogLevel.Default);
            var indent = IncrementLogIndent();
            
            return Disposable.Create(() =>
            {
                var elapsed = stopwatch.ElapsedMilliseconds;
                indent.Dispose();
                Logger.WriteLine($"/{message} : Elapsed = \"{elapsed} ms\"",LogLevel.Default);                
            });
        }

        /// <summary>
        /// 区間のメッセージをバッファリングしてまとめて出力します。。
        /// example
        ///  using( Buffering())
        ///  {
        ///     // ...
        ///  }// ここでメッセージがまとめて出力されます。
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Disposeするとログを出力します。</returns>
        protected IDisposable LogBuffering()
        {
            _isBuffering = true;
            _buffer.Clear();
            
            return Disposable.Create(() =>
            {
                while (_buffer.TryDequeue(out var message))
                {
                    _helper.WriteLine(message);
                }

                _isBuffering = false;
            });
        }

        /// <summary>
        /// 実行中のランタイムがデバッグ実行かどうかを取得します。
        /// </summary>
        protected bool IsDebugging =>
        #if DEBUG
            true;
        #else
            false;
        #endif
    }
}