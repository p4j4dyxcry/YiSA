using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using YiSA.Foundation.Internal;

namespace YiSA.Foundation.Operation
{
    /// <summary>
    /// 標準的なオペレーション操作
    /// </summary>
    public class Operator : IOperator
    {
        private readonly InternalUndoStack<IOperation> _internalUndoStack;
        public bool CanUndo => _internalUndoStack.CanUndo;
        public bool CanRedo => _internalUndoStack.CanRedo;

        public Operator()
            : this(1024)
        {

        }

        public Operator(int capacity)
        {
            Debug.Assert(capacity > 0);
            StackSize = capacity;
            _internalUndoStack = new InternalUndoStack<IOperation>(capacity);
        }

        public void Undo()
        {
            if (!CanUndo)
                return;

            PreStackChanged();
            var undoOperation = _internalUndoStack.Undo();
            undoOperation.Rollback();
            OnStackChanged(OperationStackChangedEvent.Undo,undoOperation);
        }

        public void Redo()
        {
            if (!CanRedo)
                return;

            PreStackChanged();
            var redo = _internalUndoStack.Redo();
            redo.RollForward();
            OnStackChanged(OperationStackChangedEvent.Redo,redo);
        }

        public IOperation Peek()
        {
            return _internalUndoStack.Peek();
        }

        public IOperation Pop()
        {
            PreStackChanged();
            var result = _internalUndoStack.Pop();
            OnStackChanged(OperationStackChangedEvent.Pop,result);
            return result;
        }

        public IOperation Push(IOperation operation)
        {
            PreStackChanged();
            _internalUndoStack.Push(operation);
            OnStackChanged(OperationStackChangedEvent.Push,operation);
            return operation;
        }

        public IOperation Execute(IOperation operation)
        {
            Push(operation).RollForward();
            return operation;
        }

        public void Flush()
        {
            PreStackChanged();
            _internalUndoStack.Clear();
            OnStackChanged(OperationStackChangedEvent.Clear,null);
        }

        #region PropertyChanged

        private int _preStackChangedCall;

        public IEnumerable<IOperation> RollForwardTargets => _internalUndoStack.RedoStack.Reverse();
        public event EventHandler<OperationStackChangedEventArgs>? StackChanged;

        private void PreStackChanged()
        {
            //! Operationの再帰呼び出しを検知するとassert
            Debug.Assert(_preStackChangedCall == 0 );
            _preStackChangedCall++;
        }

        private void OnStackChanged(OperationStackChangedEvent eventType , IOperation? operation)
        {
            Debug.Assert(_preStackChangedCall == 1);
            _preStackChangedCall--;
            StackChanged?.Invoke(this, new OperationStackChangedEventArgs()
            {
                EventType = eventType,
                Operation = operation,
            });
        }
        #endregion

        public IEnumerable<IOperation> Operations => _internalUndoStack;

        public bool IsOperating =>_preStackChangedCall != 0;

        public int StackSize { get; }
    }
}
