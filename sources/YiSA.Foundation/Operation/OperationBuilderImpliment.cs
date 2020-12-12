﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using YiSA.Foundation.Internal;
using YiSA.Foundation.Operation.Extensions;

namespace YiSA.Foundation.Operation
{
    public partial class OperationBuilder
    {
        private class EventBinder
        {
            private readonly ICollection<Action> _postEvents = new HashSet<Action>();
            private readonly ICollection<Action> _prevEvents = new HashSet<Action>();

            public void AddPostEvent(Action action)
            {
                _postEvents.Add(action);
            }

            public void AddPreEvent(Action action)
            {
                _prevEvents.Add(action);
            }

            public IOperation BindEvents(IOperation operation)
            {
                if (_prevEvents.Any())
                    operation = operation.AddPreEvent(() =>
                    {
                        foreach (var e in _prevEvents)
                            e.Invoke();
                    });

                if (_postEvents.Any())
                    operation = operation.AddPostEvent(() =>
                    {
                        foreach (var e in _postEvents)
                            e.Invoke();
                    });

                return operation;
            }
        }

        private class OperationBuilderFromValues<T> :  IBuilderFromValues<T>
        {
            private readonly Action<T> _function;
            private T _prev = default!;
            private T _new = default!;
            private IMergeJudge _mergeJudge = new EmptyMergeJudge();
            private readonly EventBinder _eventEventBinder = new EventBinder();
            private bool _canBuild;
            private string _name = string.Empty;

            public OperationBuilderFromValues(Action<T> function)
            {
                _function = function;
            }

            public IBuilderFromValues<T> Values(T newValue, T prevValue)
            {
                _canBuild = true;
                _prev = prevValue;
                _new = newValue;
                return this;
            }

            public IBuilderFromValues<T> Throttle(object key , TimeSpan timeSpan)
            {
                _mergeJudge = new ThrottleMergeJudge<int>(key.GetHashCode(),timeSpan);
                return this;
            }

            public IOperationBuilder Message(string name)
            {
                _name = name;
                return this;
            }

            public IOperationBuilder PostEvent(Action action)
            {
                _eventEventBinder.AddPostEvent(action);
                return this;
            }

            public IOperationBuilder PrevEvent(Action action)
            {
                _eventEventBinder.AddPreEvent(action);
                return this;
            }

            public IOperation Build()
            {
                if(_canBuild is false)
                    throw new Exception();

                var operation = new MergeableOperation<T>(_function, _new, _prev, _mergeJudge)
                {
                    Message = _name
                };
                return _eventEventBinder.BindEvents(operation);
            }
        }

        private class BuilderFromNewOperationValue<T> : IBuilderFromNewValue<T>
        {
            private readonly object _sender;
            private readonly string _propertyName;
            private T _newValue= default!;
            private TimeSpan _throttleTimeSpan;
            private readonly EventBinder _eventEventBinder = new EventBinder();
            private bool _canBuild;
            private string _name = string.Empty;

            public BuilderFromNewOperationValue(object sender, string propertyName)
            {
                Debug.Assert(propertyName != string.Empty);

                _sender = sender;
                _propertyName = propertyName;
                _throttleTimeSpan = Extensions.Operation.DefaultMergeSpan;
            }

            public IBuilderFromNewValue<T> NewValue(T newValue)
            {
                _canBuild = true;
                _newValue = newValue;
                return this;
            }

            public IBuilderFromNewValue<T> Throttle(TimeSpan timeSpan)
            {
                _throttleTimeSpan = timeSpan;
                return this;
            }

            public IOperationBuilder Message(string name)
            {
                _name = name;
                return this;
            }

            public IOperationBuilder PostEvent(Action action)
            {
                _eventEventBinder.AddPostEvent(action);
                return this;
            }

            public IOperationBuilder PrevEvent(Action action)
            {
                _eventEventBinder.AddPreEvent(action);
                return this;
            }

            public IOperation Build()
            {
                Debug.Assert(_canBuild);

                var operation = _sender.GenerateSetPropertyOperation(_propertyName, _newValue, _throttleTimeSpan);
                operation.Message = _name;
                return _eventEventBinder.BindEvents(operation);
            }
        }

        private class CollectionOperationBuilder<T> : ICollectionOperationBuilder<T>
        {
            private readonly IList<T> _list;

            public CollectionOperationBuilder(IList<T> list)
            {
                _list = list;
            }

            public IOperation BuildAddOperation(T addValue)
            {
                return _list.ToAddOperation(addValue);
            }

            public IOperation BuildRemoveOperation(T removeValue)
            {
                return _list.ToRemoveOperation(removeValue);
            }

            public IOperation BuildAddRangeOperation(IEnumerable<T> addValues)
            {
                return _list.ToAddRangeOperation(addValues);
            }

            public IOperation BuildRemoveRangeOperation(IEnumerable<T> removeValues)
            {
                return _list.ToRemoveRangeOperation(removeValues);
            }

            public IOperation BuildClearOperation()
            {
                return _list.ToClearOperation();
            }
        }

        private class CollectionOperationCustomizer<T> : ICollectionOperationCustomizer<T>
        {
            private readonly ICollection<Action<T>> _addEvents    = new HashSet<Action<T>>();
            private readonly ICollection<Action> _addedEvents     = new HashSet<Action>();
            private readonly ICollection<Action<T>> _removeEvents = new HashSet<Action<T>>();
            private readonly ICollection<Action> _removedEvents   = new HashSet<Action>();
            private readonly ICollection<Action> _clearEvents       = new HashSet<Action>();
            private readonly ICollection<Action> _rollbackEvents  = new HashSet<Action>();
            public ICollectionOperationCustomizer<T> RegisterAdd(Action<T> value)
            {
                _addEvents.Add(value);
                return this;
            }

            public ICollectionOperationCustomizer<T> RegisterRemove(Action<T> function)
            {
                _removeEvents.Add(function);
                return this;
            }

            public ICollectionOperationCustomizer<T> RegisterAdd(Action function)
            {
                _addedEvents.Add(function);
                return this;
            }

            public ICollectionOperationCustomizer<T> RegisterRemove(Action function)
            {
                _removedEvents.Add(function);
                return this;
            }

            public ICollectionOperationCustomizer<T> RegisterClear(Action function)
            {
                _clearEvents.Add(function);
                return this;
            }

            public ICollectionOperationCustomizer<T> RegisterRollback(Action function)
            {
                _rollbackEvents.Add(function);
                return this;
            }

            public IOperation BuildAddOperation(T addValue)
            {
                Debug.Assert(_addEvents.Any() && _removeEvents.Any());

                return new DelegateOperation(
                    MakeInvokeListAction(_addEvents,_addedEvents,addValue),
                    MakeInvokeListAction(_removeEvents, _removedEvents, addValue));
            }

            public IOperation BuildRemoveOperation(T removeValue)
            {
                Debug.Assert(_addEvents.Any() && _removeEvents.Any());

                return new DelegateOperation(
                    MakeInvokeListAction(_removeEvents, _removedEvents, removeValue),
                    MakeInvokeListAction(_addEvents, _addedEvents, removeValue));
            }

            public IOperation BuildAddRangeOperation(IEnumerable<T> addValues)
            {
                return addValues.Select(BuildAddOperation).ToCompositeOperation();
            }

            public IOperation BuildRemoveRangeOperation(IEnumerable<T> removeValues)
            {
                return removeValues.Select(BuildRemoveOperation).ToCompositeOperation();
            }

            public IOperation BuildClearOperation()
            {
                Debug.Assert(_clearEvents.Any() && _rollbackEvents.Any());

                return new DelegateOperation(
                    () =>
                    {
                        foreach (var action in _clearEvents)
                            action.Invoke();

                    },
                    () =>
                    {
                        foreach (var action in _rollbackEvents)
                            action.Invoke();
                    });
            }

            private static Action MakeInvokeListAction(IEnumerable<Action<T>> actions, IEnumerable<Action> postActions, T value)
            {
                return () =>
                {
                    foreach (var action in actions)
                        action.Invoke(value);

                    foreach (var action in postActions)
                        action.Invoke();

                };
            }
        }

        private class Builder : IOperationBuilder
        {
            private IOperation _operation;

            public Builder(IOperation operation)
            {
                _operation = operation;
            }

            public IOperationBuilder Message(string name)
            {
                _operation.Message = name;
                return this;
            }

            public IOperationBuilder PostEvent(Action action)
            {
                _operation = _operation.AddPostEvent(action);
                return this;
            }

            public IOperationBuilder PrevEvent(Action action)
            {
                _operation = _operation.AddPreEvent(action);
                return this;
            }

            public IOperation Build()
            {
                return _operation;
            }
        }

        private class MergeableBuilder : Builder , IMergeableOperationBuilder
        {
            private readonly MergeableOperation _mergeableOperation;

            public MergeableBuilder(MergeableOperation operation) : base(operation)
            {
                _mergeableOperation = operation;
            }

            public IMergeableOperationBuilder SetActionName(string executeAction, string rollbackAction)
            {
                _mergeableOperation.SetActionName(executeAction,rollbackAction);
                return this;
            }
        }

        private class MergeableBuilder<T> : Builder, IMergeableOperationBuilder
        {
            private readonly MergeableOperation<T> _mergeableOperation;

            public MergeableBuilder(MergeableOperation<T> operation) : base(operation)
            {
                _mergeableOperation = operation;
            }

            public IMergeableOperationBuilder SetActionName(string executeAction, string rollbackAction)
            {
                _mergeableOperation.Message = executeAction;
                return this;
            }
        }
    }
}
