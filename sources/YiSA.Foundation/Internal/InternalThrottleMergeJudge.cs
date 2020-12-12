using System;
using System.Diagnostics;
using YiSA.Foundation.Operation;

namespace YiSA.Foundation.Internal
{
    /// <summary>
    /// 識別子とタイムスタンプからマージ可能か判断する
    /// </summary>
    internal class ThrottleMergeJudge<T> : IMergeJudge
    {
        public T Key { get; }

        /// <summary>
        /// マージ間隔
        /// </summary>
        public TimeSpan ConvergeTimeSpan { get; set; }

        /// <summary>
        /// 時間計測用のストップウォッチ
        /// </summary>
        private Stopwatch Stopwatch { get; }

        public bool CanMerge(IMergeJudge mergeJudge)
        {
            if (mergeJudge is ThrottleMergeJudge<T> timeStampMergeInfo)
            {
                return Equals(Key, timeStampMergeInfo.Key) &&
                       timeStampMergeInfo.Stopwatch.ElapsedMilliseconds < timeStampMergeInfo.ConvergeTimeSpan.TotalMilliseconds;
            }
            return false;
        }

        public ThrottleMergeJudge(T key , TimeSpan convergeTimeSpan)
        {
            Key = key;
            ConvergeTimeSpan = convergeTimeSpan;
            Stopwatch = Stopwatch.StartNew();
        }
        public IMergeJudge Update(IMergeJudge prevMergeJudge)
        {
            if (prevMergeJudge is ThrottleMergeJudge<T> throttleMergeJudge)
            {
                throttleMergeJudge.Stopwatch.Restart();
                throttleMergeJudge.ConvergeTimeSpan = ConvergeTimeSpan;
                return throttleMergeJudge;
            }

            return this;
        }

        public object? GetMergeKey()
        {
            return Key;
        }
    }
}
