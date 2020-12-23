using System;
using YiSA.Foundation.Common;
using YiSA.Foundation.Internal;

namespace YiSA.Foundation.Logging.Extensions
{

    public static class RelayLoggerExtensions
    {
        public static IDisposable SubscribeWithCurrentThread(this RelayLogger relayLogger, Action<string, LogLevel> action)
        {
            var context = InternalSynchronizeContextUtility.GetOrCreate();
            return relayLogger.Subscribe(
                (msg, lv) => context.Send(() => action(msg, lv)));
        }
    }
}