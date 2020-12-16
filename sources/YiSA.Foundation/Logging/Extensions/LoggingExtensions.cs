using System;
using System.Threading.Tasks;

namespace YiSA.Foundation.Logging.Extensions
{
    public static class LoggingExtensions
    {
        public static Task Catch<T>(this Task task ,ILogger logger , Func<Exception,string>? formatter = null)
        {
            formatter ??= e => e.ToString();

            return task.ContinueWith(t =>
            {
                if(t.Exception is null)
                    return;

                foreach (var e in t.Exception.InnerExceptions)
                {
                    logger.WriteLine(formatter(e), LogLevel.Error);
                }
            });
        }

        public static void TryCatch(this ILogger logger , Action action, Func<Exception, string>? formatter = null)
        {
            formatter ??= e => e.ToString();
            try
            {
                action();
            }
            catch(Exception e)
            {
                logger.WriteLine(formatter(e),LogLevel.Error);
            }
        }
    }
}