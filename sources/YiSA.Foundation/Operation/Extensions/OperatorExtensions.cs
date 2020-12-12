using System;
using System.Linq;

namespace YiSA.Foundation.Operation.Extensions
{
    public static class OperatorExtensions
    {
        public static void MoveTo(this IOperator controller, IOperation target)
        {
            var isRollBack    = controller.Operations.Contains(target);

            var isRollForward = controller.RollForwardTargets.Contains(target);

            if(isRollBack is false && isRollForward is false)
                return;

            if (isRollBack)
            {
                while (controller.Peek() != target)
                    controller.Undo();
            }

            if (isRollForward)
            {
                while (controller.RollForwardTargets.FirstOrDefault() != target)
                    controller.Redo();
                controller.Redo();
            }
        }

        public static void Distinct(this IOperator controller, object key)
        {
            _distinct_internal<MergeableOperation>(controller, key,
                (x, y) => MergeableOperation.MakeMerged(x, y, false));
        }

        public static void Distinct<T>(this IOperator controller, object key)
        {
            _distinct_internal<MergeableOperation<T>>(controller, key,
                (x, y) => MergeableOperation<T>.MakeMerged(x, y, false));
        }

        private static void _distinct_internal<T>(this IOperator controller, object key,
            Func<T, T, IOperation> generateMergedOperation)
            where T : class, IMergeableOperation
        {
            var operations = controller.Operations.ToList();

            var mergeable = operations
                .OfType<T>()
                .Where(x => x.MergeJudge.GetMergeKey() == key)
                .ToArray();

            var first = mergeable.FirstOrDefault();
            var last = mergeable.LastOrDefault();

            if (first is null || last is null)
                return;

            var lastIndex = 0;
            foreach (var operation in mergeable)
            {
                lastIndex = operations.IndexOf(operation);
                operations.RemoveAt(lastIndex);
            }

            var newOperation = generateMergedOperation(first, last);
            operations.Insert(lastIndex, newOperation);

            controller.Flush();
            operations.ForEach(x => controller.Push(x));
        }

        public static void BeginTransaction(this IOperator @operator , string message)
        {
            // 既にTransactionは開始されている
            if (@operator.Operations.OfType<TransactionOperation>().Any())
            {
                throw new InvalidOperationException();
            }

            var transaction = new TransactionOperation(@operator)
            {
                Message = message,
            };
            @operator.Push(transaction);
            transaction.BindStackChanged();
        }

        public static void EndTransaction(this IOperator @operator)
        {
            var transaction = @operator.Operations.OfType<TransactionOperation>().FirstOrDefault();

            // 既にTransactionが見つからない
            if (transaction is null)
            {
                throw new InvalidOperationException();
            }

            var newOperation = transaction.UnBindStackChanged().SetMessage(transaction.Message);
            var list = @operator.Operations.ToList();

            var first = list.IndexOf(transaction);
            list.RemoveRange(first,list.Count - first);
            list.Add(newOperation);

            @operator.Flush();
            list.ForEach(x => @operator.Push(x));
        }
    }
}
