﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YiSA.Foundation.Internal
{
    internal static class InternalPropertyReflection
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, IAccessor?>> Cache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<string, IAccessor?>>();

        public static bool PublicOnly { get; set; } = true;

        private static IAccessor? MakeAccessor(object @object, string propertyName)
        {
            var propertyInfo = @object.GetType().GetProperty(propertyName,
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Instance);

            if (propertyInfo == null)
                return null;

            if (@object.GetType().IsClass == false)
            {
                return new StructAccessor(propertyInfo, PublicOnly);
            }

            var getInfo = propertyInfo.GetGetMethod(PublicOnly is false);
            var setInfo = propertyInfo.GetSetMethod(PublicOnly is false);

            var getterDelegateType = typeof(Func<,>).MakeGenericType(propertyInfo.DeclaringType!, propertyInfo.PropertyType);
            var getter = getInfo != null ? Delegate.CreateDelegate(getterDelegateType, getInfo) : null;

            var setterDelegateType = typeof(Action<,>).MakeGenericType(propertyInfo.DeclaringType!, propertyInfo.PropertyType);
            var setter = setInfo != null? Delegate.CreateDelegate(setterDelegateType, setInfo) : null;

            var accessorType = typeof(PropertyAccessor<,>).MakeGenericType(propertyInfo.DeclaringType!, propertyInfo.PropertyType);

            return (IAccessor)Activator.CreateInstance(accessorType, getter, setter)!;
        }

        private static IAccessor? GetAccessor(object @object, string propertyName)
        {
            var accessors = Cache.GetOrAdd(@object.GetType(), x => new ConcurrentDictionary<string, IAccessor?>());
            return accessors.GetOrAdd(propertyName, x => MakeAccessor(@object, propertyName));
        }

        public static void SetProperty(object @object, string property, object? value)
        {
            GetAccessor(@object,property)?.SetValue(@object,value);
        }

        public static object? GetProperty(object @object , string property)
        {
            return GetAccessor(@object, property)?.GetValue( @object);
        }

        public static Type? GetPropertyType(object @object, string property)
        {
            return GetAccessor(@object, property)?.PropertyType;
        }

        public static T GetProperty<T>(object @object, string property)
        {
            var value = GetAccessor(@object, property)?.GetValue(@object);

            if (value is T tValue)
                return tValue;

            throw new InvalidCastException();
        }

        public static bool ExistsGetter(object @object, string property)
        {
            return GetAccessor(@object, property)?.HasGetter ?? false;
        }

        public static bool ExistsSetter(object @object, string property)
        {
            return GetAccessor(@object, property)?.HasSetter ?? false;
        }

        internal static string GetMemberName<T,TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            return ((MemberExpression)expression.Body).Member.Name;
        }

        private static MethodInfo[]? _regisMethodInfo ;
        private static readonly Dictionary<int, MethodInfo> MethodCache = new Dictionary<int, MethodInfo>();

        public static TDelegate CreateDelegate<TDelegate>(object o, MethodInfo method)
        {
            return (TDelegate)(object)Delegate.CreateDelegate(typeof(TDelegate), o, method);
        }

        public static void InvokeGenericMethod(object classInstance , Type type, string methodName,params object[] args)
        {
            if (_regisMethodInfo is null)
            {
                _regisMethodInfo = classInstance.GetType()
                    .GetMethods()
                    .Where(x => x.Name == methodName)
                    .Where(x => x.IsGenericMethod)
                    .OrderBy(x => x.GetParameters().Length)
                    .ToArray();
            }

            var hash = type.GetHashCode();

            hash = args.Aggregate(hash, (current, arg) => current ^ arg.GetType().GetHashCode());

            if (MethodCache.TryGetValue(hash, out var genericMethodInfo) is false)
            {
                MethodCache[hash] =
                    genericMethodInfo =
                        _regisMethodInfo[args.Length - 1].MakeGenericMethod(type);
            }
            Debug.Assert(genericMethodInfo != null);
            genericMethodInfo!.Invoke(classInstance, args.Length != 0 ? args : null);
        }
    }
}
