using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace YiSA.Foundation.Operation
{
    public abstract class ListOperationBase
    {
        private readonly Func<IList>? _generator;
        private readonly IList? _list;

        protected IList get_list()
            => _list ?? _generator!.Invoke();

        protected ListOperationBase(Func<IList> listGenerator)
        {
            _generator = listGenerator;

        }
        protected ListOperationBase(IList list)
        {
            _list = list;
        }
    }

    public abstract class ListOperationBase<T>
    {
        private readonly Func<IList<T>>? _generator;
        private readonly IList<T>? _list;

        protected IList<T> get_list()
            => _list ?? _generator!.Invoke();

        protected ListOperationBase(Func<IList<T>> listGenerator)
        {
            _generator = listGenerator;

        }
        protected ListOperationBase(IList<T> list)
        {
            Debug.Assert(list != null);
            _list = list;
        }
    }

    /// <summary>
    /// 追加オペレーション
    /// </summary>
    public class InsertOperation<T> : ListOperationBase<T> , IOperation
    {
        public string Message { get; set; } = string.Empty;

        private readonly T _property;
        private readonly int _insertIndex;

        public InsertOperation(Func<IList<T>> listGenerator, T insertValue , int insertIndex = -1)
            :base(listGenerator)
        {
            _property = insertValue;
            _insertIndex = insertIndex;
        }

        public InsertOperation(IList<T> list, T insertValue, int insertIndex = -1)
            :base(list)
        {
            _property = insertValue;
            _insertIndex = insertIndex;
        }

        public void RollForward()
        {
            if(_insertIndex < 0)
                get_list().Add(_property);
            else
                get_list().Insert(_insertIndex,_property);
        }

        public void Rollback()
        {
            var list = get_list();
            list.RemoveAt( _insertIndex < 0 ? list.Count - 1 : _insertIndex );
        }
    }

    /// <summary>
    /// 削除オペレーション
    /// RollBack時に削除位置も復元する
    /// </summary>
    public class RemoveOperation<T> : ListOperationBase<T>, IOperation
    {
        public string Message { get; set; } = string.Empty;

        private readonly T _property;
        private int _insertIndex = -1;
        public RemoveOperation(Func<IList<T>> listGenerator, T removeValue)
            :base(listGenerator)
        {
            _property = removeValue;
        }

        public RemoveOperation(IList<T> list, T removeValue)
            : base(list)
        {
            _property = removeValue;
        }

        public void RollForward()
        {
            _insertIndex = get_list().IndexOf(_property);

            if (_insertIndex < 0)
                return;

            get_list().RemoveAt(_insertIndex);
        }

        public void Rollback()
        {
            if (_insertIndex < 0)
                return;

            get_list().Insert(_insertIndex, _property);
        }
    }

    /// <summary>
    /// インデックス指定削除オペレーション
    /// RollBack時に削除位置も復元する
    /// </summary>
    public class RemoveAtOperation : ListOperationBase, IOperation
    {
        public string Message { get; set; } = string.Empty;

        private object? _data;
        private readonly int _index ;

        public RemoveAtOperation(Func<IList> listGenerator, int index)
            :base(listGenerator)
        {
            Debug.Assert(_index >= 0);
            _index = index;
        }

        public RemoveAtOperation(IList list, int index)
            :base(list)
        {
            Debug.Assert(_index >= 0);
            _index = index;
        }

        public void RollForward()
        {
            var list = get_list();
            _data = list[_index];
            list.RemoveAt(_index);
        }

        public void Rollback()
        {
            get_list().Insert(_index, _data);
        }
    }

    /// <summary>
    /// クリアオペレーション
    /// </summary>
    public class ClearOperation<T> : ListOperationBase<T>, IOperation
    {
        public string Message { get; set; } = string.Empty;

        private T[]? _prevData;

        public ClearOperation(Func<IList<T>> listGenerator)
            :base(listGenerator)
        {
        }

        public ClearOperation(IList<T> list)
            :base(list)
        {
        }

        public void RollForward()
        {
            _prevData = get_list().ToArray();
            get_list().Clear();
        }

        public void Rollback()
        {
            foreach (var data in _prevData!)
                get_list().Add(data);
        }
    }
}
