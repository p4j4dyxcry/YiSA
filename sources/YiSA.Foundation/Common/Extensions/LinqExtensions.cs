using System;
using System.Collections.Generic;
using System.Linq;

namespace YiSA.Foundation.Common.Extensions
{
    public static class LinqExtensions
    {
        public static T AddTo<T,TBase>(this T item , IList<TBase> collection)
            where T : TBase
        {
            collection.Add(item);
            return item;
        }

        public static T DisposeBy<T,TBase>(this T item, IList<TBase> collection) 
            where T : TBase , IDisposable
        {
            return item.AddTo(collection);
        }

        public static T MinBy<T, U>(this IEnumerable<T> source, Func<T, U> selector)
        {
            return source.MinMaxBy(selector).min;
        }
        
        public static T MaxBy<T, U>(this IEnumerable<T> source, Func<T, U> selector)
        {
            return source.MinMaxBy(selector).max;
        }
        
        public static (T min ,T max) MinMaxBy<T, U>(this IEnumerable<T> source, Func<T, U> selector)
        {
            var sorted = source.OrderBy(selector).ToArray();
            return (sorted.First(), sorted.Last());
        }

    }
}