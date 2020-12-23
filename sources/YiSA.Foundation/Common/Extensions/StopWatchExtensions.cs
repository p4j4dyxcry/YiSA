using System.Diagnostics;

namespace YiSA.Foundation.Common.Extensions
{
    public static class StopWatchExtensions
    {
        public static long ElapsedNanoseconds(this Stopwatch stopwatch)
        {
           return (long)(stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000000000);
        }
        
        public static long ElapsedMicroseconds(this Stopwatch stopwatch)
        {
            return (long)(stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000000);
        }
    }
}