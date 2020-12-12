using System;
using System.Collections.Generic;
using System.Linq;
using YiSA.Foundation.Internal;

namespace YiSA.Foundation.Operation
{
    /// <summary>
    /// Dispose時にHistoryから抹消されるOperationです。
    /// </summary>
    public class DisposableOperation : IDisposableOperation
    {
        private readonly IOperation _original;
        private readonly IDisposable _disposable;
        private readonly IList<IOperator> _bindOperators = new List<IOperator>();

        public DisposableOperation(IOperation operation)
        {
            Message = operation.Message;
            _original = operation;

            _disposable = InternalDisposable.Make(() =>
            {
                foreach (var ope in _bindOperators)
                {
                    var list = @ope.Operations.ToList();

                    // 全削除
                    while (list.Remove(this))
                    {

                    }

                    if (list.Count == ope.Operations.Count())
                    {
                        continue;
                    }

                    ope.Flush();
                    list.ForEach(x => ope.Push(x));
                }
            });
        }

        public string Message { get; set; }
        public void RollForward()
        {
            _original.RollForward();
        }

        public void Rollback()
        {
            _original.RollForward();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public void BindOperator(IOperator @operator)
        {
            _bindOperators.Add(@operator);
        }
    }
}
