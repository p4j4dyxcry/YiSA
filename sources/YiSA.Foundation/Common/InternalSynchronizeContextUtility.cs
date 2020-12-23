using System;
using System.Threading;

namespace YiSA.Foundation.Common
{
    public static class InternalSynchronizeContextUtility
    {
        public static SynchronizationContext GetOrCreate () =>
            SynchronizationContext.Current ?? new SynchronizationContext();
    }
    
    public static class InternalSynchronizeContextExtensions
    {
        private static readonly object Empty = new object();

        public static void Send(this SynchronizationContext context , Action action)
        {
            context.Send(_=>action(),Empty);
        }
        public static void Post(this SynchronizationContext context , Action action)
        {
            context.Post(_=>action(),Empty);
        }
    }
}