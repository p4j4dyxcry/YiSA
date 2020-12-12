using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace YiSA.Foundation.Operation.Extensions
{
    public class TransactionOperation : IOperation
    {
        public string Message { get; set; } = string.Empty;

        private readonly IOperator _operator;
        private readonly IList<IOperation> _operations = new List<IOperation>();
        public TransactionOperation(IOperator @operator)
        {
            _operator = @operator;
        }

        public void BindStackChanged()
        {
            _operator.StackChanged += OperatorOnStackChanged;
        }

        public ICompositeOperation UnBindStackChanged()
        {
            _operator.StackChanged -= OperatorOnStackChanged;
            return _operations.ToCompositeOperation();
        }

        private void OperatorOnStackChanged(object? sender, OperationStackChangedEventArgs e)
        {
            if (e.EventType == OperationStackChangedEvent.Push ||
                e.EventType == OperationStackChangedEvent.Redo)
            {
                Debug.Assert(e.Operation != null);
                _operations.Add(e.Operation!);
            }
            else if (e.EventType == OperationStackChangedEvent.Pop)
            {
                Debug.Assert(e.Operation != null);
                _operations.Remove(e.Operation!);
            }
            else
            {
                // Transaction 構築中の Undo / Clear は不正なトランザクションになるので例外を送出
                throw new InvalidOperationException($"{e.EventType}");
            }
        }

        public void RollForward()
        {
            throw new InvalidOperationException();
        }

        public void Rollback()
        {
            throw new InvalidOperationException();
        }
    }
}
