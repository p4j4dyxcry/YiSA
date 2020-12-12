using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using YiSA.Foundation.Internal;

namespace YiSA.Foundation.Operation.Extensions
{
    public static class OperatorServiceExtensions
    {
        public static void ExecuteAdd<T>(this IOperator controller, IList<T> list, T value)
        {
            var operation = list.ToAddOperation(value);
            controller.Execute(operation);
        }

        public static void ExecuteInsert<T>(this IOperator controller, IList<T> list, T value , int index)
        {
            var operation = new InsertOperation<T>(@list, value , index);
            controller.Execute(operation);
        }

        public static void ExecuteAddRange<T>(this IOperator controller, IList<T> list, IEnumerable<T> value )
        {
            var operation = list.ToAddRangeOperation(value);
            controller.Execute(operation);
        }

        public static void ExecuteRemove<T>(this IOperator controller, IList<T> list, T value)
        {
            var operation = list.ToRemoveOperation(value);
            controller.Execute(operation);
        }

        public static void ExecuteRemoveAt<T>(this IOperator controller, IList<T> list, int index)
        {
            if (list is IList iList)
            {
                var operation = iList.ToRemoveAtOperation(index);
                controller.Execute(operation);
            }
            else
            {
                var target = list[index];
                var operation = list.ToRemoveOperation(target);
                controller.Execute(operation);
            }
        }

        public static void ExecuteRemoveItems<T>(this IOperator controller, IList<T> list, IEnumerable<T> value )
        {
            var operation = list.ToRemoveRangeOperation(value);
            controller.Execute(operation);
        }

        public static IOperation ExecuteSetProperty<T,TProperty>(this IOperator controller, T owner , string propertyName , TProperty value)
            where T : class
        {
            var operation = owner
                .GenerateSetPropertyOperation(propertyName, value)
                .Merge(controller);

            return controller.Execute(operation);
        }

        public static IOperation ExecuteSetProperty<T,TProperty>(this IOperator controller, T owner , string propertyName , TProperty value,TimeSpan autoMergeTimeSpan)
            where T : class
        {
            var operation = owner
                .GenerateSetPropertyOperation(propertyName, value,autoMergeTimeSpan)
                .Merge(controller);

            return controller.Execute(operation);
        }

        public static IDisposable BindPropertyChanged<T>(this IOperator controller , INotifyPropertyChanged owner, string propertyName , bool autoMerge = true)
        {
            var prevValue = InternalPropertyReflection.GetProperty<T>(owner, propertyName);
            owner.PropertyChanged += PropertyChanged;

            return InternalDisposable.Make(() => owner.PropertyChanged -= PropertyChanged);

            // local function
            void PropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (controller.IsOperating)
                    return;

                if (args.PropertyName == propertyName)
                {
                    T newValue = InternalPropertyReflection.GetProperty<T>(owner, propertyName);
                    var operation = owner
                        .GenerateAutoMergeOperation(propertyName,newValue,prevValue,$"{sender.GetHashCode()}.{propertyName}",Operation.DefaultMergeSpan)
                        .SetMessage($"PropertyChanged {propertyName} Value = {newValue} , prev ={prevValue}");

                    if (autoMerge)
                    {
                        operation = operation.Merge(controller);
                    }

                    prevValue = newValue;

                    controller.Push(operation);
                }
            }
        }
    }
}
