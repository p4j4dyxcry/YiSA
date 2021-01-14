using System;
using System.Linq;

namespace YiSA.Foundation.Internal
{
    internal static class TryCatchUtility
    {
        public static T TryInvoke<T>(Func<T> action , T fallBack)
        {
            try
            {
                return action();
            }
            catch
            {
                return fallBack;
            }
        }
        
        public static void TryInvoke(Action action )
        {
            try
            {
                action();
            }
            catch
            {
                // ignored
            }
        }

        public static T RetryInvoke<T>(Func<T> action,T fallBack, int retryCount, int intervalMs)
        {
            try
            {
                return action();
            }
            catch
            {
                foreach (var _ in Enumerable.Range(0,retryCount))
                {
                    try
                    {
                        return action();
                    }
                    catch
                    {
                        // ignored.
                    }
                }

                return fallBack;
            }
        }
    }
}