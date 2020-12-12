using System;

namespace YiSA.Foundation.Operation
{
    public interface IMergeableOperation : IOperation
    {
        IMergeJudge MergeJudge { get; }
        IMergeableOperation Merge(IOperator @operator);
    }

    internal interface IOperationWithEvent : IOperation
    {
        event Action OnExecuted;
        event Action OnPreviewExecuted;
    }

    public class NamedAction
    {
        public string Name { get; set; } = string.Empty;
        public Action? Action { get; set; }

        public void Invoke() => Action?.Invoke();
    }

    /// <summary>
    /// マージ可能な操作
    /// </summary>
    public class MergeableOperation : IMergeableOperation, IOperationWithEvent
    {
        public string Message { get; set; } = string.Empty;
        public event Action? OnExecuted;
        public event Action? OnPreviewExecuted;

        private readonly NamedAction _rollForward;
        private NamedAction _rollBack;
        public IMergeJudge MergeJudge { get; private set; }

        public string RollForwardInfo => _rollForward.Name;
        public string RollBackInfo => _rollBack.Name;

        public MergeableOperation(Action invoke , Action rollback , IMergeJudge mergeJudge)
        {
            _rollForward = new NamedAction(){Action = invoke};
            _rollBack    = new NamedAction(){Action = rollback};

            MergeJudge = mergeJudge;
        }

        public MergeableOperation(NamedAction invoke, NamedAction rollback, IMergeJudge mergeJudge)
        {
            _rollForward = invoke;
            _rollBack = rollback;

            MergeJudge = mergeJudge;
        }

        public void RollForward()
        {
            OnPreviewExecuted?.Invoke();
            _rollForward.Invoke();
            OnExecuted?.Invoke();
        }

        public void Rollback()
        {
            OnPreviewExecuted?.Invoke();
            _rollBack.Invoke();
            OnExecuted?.Invoke();
        }

        public void SetActionName(string execute, string rollback)
        {
            _rollForward.Name = execute;
            _rollBack.Name = rollback;
        }

        /// <summary>
        /// OperationManagerのUndoStackとマージします。
        /// 統合されたOperationはUndoStackから除外されます。
        /// Operationが統合された場合OperationManagerのRedoStackはクリアされます。
        /// </summary>
        public IMergeableOperation Merge(IOperator @operator)
        {
            if (@operator.CanUndo is false)
                return this;

            var topCommand = @operator.Peek();
            var mergeInfo = MergeJudge!;
            while (topCommand is MergeableOperation mergeableOperation)
            {
                if (MergeJudge.CanMerge(mergeableOperation.MergeJudge) is false)
                    break;
                mergeInfo = mergeableOperation.MergeJudge;
                _rollBack = mergeableOperation._rollBack;
                @operator.Pop();
                topCommand = @operator.Peek();
            }

            MergeJudge = MergeJudge.Update(mergeInfo);
            return this;
        }

        /// <summary>
        /// 2つのオペレーションをマージしたオペレーションを作成
        /// 失敗した場合はOperation.Emptyが返却される
        /// </summary>
        public static IOperation MakeMerged(MergeableOperation prevOperation , MergeableOperation postOperation,bool checkKey = true)
        {
            if (checkKey )
            {
                if (postOperation.MergeJudge.CanMerge(prevOperation.MergeJudge))
                {
                    return new MergeableOperation(postOperation._rollForward,prevOperation._rollBack,postOperation.MergeJudge);
                }

                return Extensions.Operation.Empty;
            }
            return new MergeableOperation(postOperation._rollForward, prevOperation._rollBack, postOperation.MergeJudge);
        }
    }


    public class MergeableOperation<T> : IMergeableOperation , IOperationWithEvent
    {
        public string Message { get; set; } = string.Empty;
        private T PrevProperty { get; set; }
        private T Property { get; }
        private Action<T> Setter { get; }
        public IMergeJudge MergeJudge { get; set; }

        public event Action? OnExecuted;
        public event Action? OnPreviewExecuted;

        public MergeableOperation(
            Action<T> setter,
            T newValue,
            T oldValue,
            IMergeJudge mergeJudge)
        {
            Setter = setter;
            PrevProperty = oldValue;
            Property = newValue;
            MergeJudge = mergeJudge;
        }
        public void RollForward()
        {
            OnPreviewExecuted?.Invoke();
            Setter.Invoke(Property);
            OnExecuted?.Invoke();
        }

        public void Rollback()
        {
            OnPreviewExecuted?.Invoke();
            Setter.Invoke(PrevProperty);
            OnExecuted?.Invoke();
        }

        /// <summary>
        /// OperationManagerのUndoStackとマージします。
        /// 統合されたOperationはUndoStackから除外されます。
        /// Operationが統合された場合OperationManagerのRedoStackはクリアされます。
        /// </summary>
        public IMergeableOperation Merge(IOperator @operator)
        {
            if (@operator.CanUndo is false)
                return this;

            var topCommand = @operator.Peek();
            var prevValue = PrevProperty;
            var mergeInfo = MergeJudge;
            while (topCommand is MergeableOperation<T> propertyChangeOperation)
            {
                if (MergeJudge.CanMerge(propertyChangeOperation.MergeJudge) is false)
                    break;
                mergeInfo = propertyChangeOperation.MergeJudge;
                prevValue = propertyChangeOperation.PrevProperty;

                @operator.Pop();

                if(@operator.CanUndo is false)
                    break;
                topCommand = @operator.Peek();
            }

            PrevProperty = prevValue;
            MergeJudge = MergeJudge.Update(mergeInfo);
            return this;
        }

        /// <summary>
        /// 2つのオペレーションをマージしたオペレーションを作成
        /// 失敗した場合はOperation.Emptyが返却される
        /// </summary>
        public static IOperation MakeMerged(MergeableOperation<T> prevOperation, MergeableOperation<T> postOperation, bool checkKey = true)
        {
            if (checkKey)
            {
                if (postOperation.MergeJudge.CanMerge(prevOperation.MergeJudge))
                {
                    return new MergeableOperation<T>(postOperation.Setter,
                        postOperation.Property,
                        prevOperation.PrevProperty,
                        postOperation.MergeJudge);
                }

                return Extensions.Operation.Empty;
            }
            return new MergeableOperation<T>(postOperation.Setter,
                postOperation.Property,
                prevOperation.PrevProperty,
                postOperation.MergeJudge);
        }
    }
}
