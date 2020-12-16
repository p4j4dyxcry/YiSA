using System;
using System.Collections.Generic;

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
    }
}