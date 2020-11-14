using System;
using System.Collections;
using System.Collections.Generic;

namespace Guting.Data
{
    public abstract class LinkedNodeList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICollection, IList
        where T : Node
    {
        private object _syncRoot = new object();
        private int _count = 0;

        protected LinkedNodeList()
        {
        }

        protected LinkedNodeList(IEnumerable<T> values)
        {
            if (values != null)
            {
                foreach (var value in values)
                {
                    Add(value);
                }
            }
        }

        protected LinkedNodeList(params T[] values)
        {
            foreach (var value in values)
            {
                Add(value);
            }
        }

        object IList.this[int index] { get => GetValue(index); set => Insert(index, value); }

        public T this[int index] { get => GetValue(index); set => Insert(index, value); }

        public T First { get; private set; }

        public T Last { get; private set; }

        public int Count => _count;

        public bool IsReadOnly => false;

        public bool IsSynchronized => false;

        public object SyncRoot => _syncRoot;

        public bool IsFixedSize => false;

        public void Add(T item)
        {
            if (item != null)
            {
                _count++;
                if (First == null || Last == null)
                {
                    First = item;
                }
                else
                {
                    Last.SetNext(item);
                }
                item.CalculateIndex();
                Last = item;
            }
        }

        public int Add(object value)
        {
            if (value is T node)
            {
                Add(node);
                return node.Index;
            }
            return -1;
        }

        public T GetValue(int index)
        {
            CheckIndexOutOfRange(index);
            var value = GetNode(index);
            return value;
        }

        public T GetNode(int index)
        {
            CheckIndexOutOfRange(index);
            Node current = First;
            for (int i = 0; i < index; i++)
            {
                current = current.GetNext();
            }
            return (T)current;
        }

        public void Clear()
        {
            _count = 0;
            First = null;
            Last = null;
        }

        public bool Contains(T item)
        {
            Node current = First;
            while (current != null)
            {
                if (current.Equals(item))
                {
                    return true;
                }
                current = current.GetNext();
            }
            return false;
        }

        public bool Contains(object value)
        {
            if (value is T val)
            {
                return Contains(val);
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        private void CutOffFromList(Node item)
        {
            if (item != null)
            {
                var previous = item.GetPrevious();
                var next = item.GetNext();
                previous.SetNext(next);
                next.SetPrevious(previous);
            }
        }

        public void Remove(Node item)
        {
            if (item != null)
            {
                var previous = item.GetPrevious();
                CutOffFromList(item);
                ReCalculateNodeIndex(previous);
                _count--;
            }
        }

        public bool Remove(T item)
        {
            Node current = First;
            while (current != null)
            {
                if (current.Equals(item))
                {
                    Remove(current);
                    return true;
                }
                current = current.GetNext();
            }
            return false;
        }

        public void Remove(object value)
        {
            if (value is T val)
            {
                Remove(val);
            }
        }

        public int IndexOf(T item)
        {
            var index = 0;
            Node current = First;
            while (current != null)
            {
                if (current.Equals(item))
                {
                    return index;
                }
                index++;
                current = current.GetNext();
            }
            return -1;
        }

        public int IndexOf(object value)
        {
            if (value is T val)
            {
                return IndexOf(val);
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            CheckIndexOutOfRange(index);
            var node = GetNode(index);
            item.SetPrevious(node.GetPrevious());
            item.SetNext(node);
            node.SetPrevious(item);
            ReCalculateNodeIndex(node);
            _count++;
        }

        public void Insert(int index, object value)
        {
            if (value is T val)
            {
                Insert(index, val);
            }
        }

        private void InsertAt(Node item, Node position)
        {
            if (item != null && position != null)
            {
                var previous = position.GetPrevious();
                item.SetPrevious(previous);
                item.SetNext(position);
                position.SetPrevious(item);
            }
        }

        public void MoveTo(int fromIndex, int toIndex)
        {
            var fromNode = GetNode(fromIndex);
            var toNode = GetNode(toIndex);
            var first = fromIndex < toIndex ? fromNode.GetPrevious() : toNode.GetPrevious();
            var last = fromIndex < toIndex ? toNode.GetNext() : fromNode.GetNext();
            CutOffFromList(fromNode);
            InsertAt(fromNode, toNode);
            ReCalculateNodeIndex(first, last);
        }

        public void RemoveAt(int index)
        {
            CheckIndexOutOfRange(index);
            Node current = First;
            for (int i = 0; i < index; i++)
            {
                current = current.GetNext();
            }
            Remove(current);
        }

        private bool CheckIndexOutOfRange(int index, bool throwException = true)
        {
            if (index >= _count || index < 0)
            {
                if (throwException)
                {
                    throw new IndexOutOfRangeException($"Provided index value '{index}' is out of range");
                }
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public IEnumerable<To> Cast<To>() where To : T
        {
            foreach (T item in this)
            {
                yield return (To)item;
            }
        }

        public T[] ToArray()
        {
            var result = new T[_count];
            var index = 0;
            var current = First;
            while (current != null)
            {
                result[index] = current;
                index++;
                current = (T)current.GetNext();
            }
            return result;
        }

        internal void ReCalculateNodeIndex(Node from = null, Node to = null)
        {
            Node current = from ?? First;
            while (current != null)
            {
                current.CalculateIndex();
                current = current.GetNext();
                if (current.Equals(to))
                {
                    break;
                }
            }
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private bool _isDisposed;
            private LinkedNodeList<T> _collection;
            private T _currentNode;

            public Enumerator(LinkedNodeList<T> collection)
            {
                _isDisposed = false;
                _collection = collection;
                _currentNode = _collection.First;
            }

            public T Current => _currentNode;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                CheckDisposed();
                var nextNode = _currentNode?.GetNext();
                if (nextNode != null)
                {
                    _currentNode = (T)nextNode;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                CheckDisposed();
                _currentNode = _collection.First;
            }

            public void Dispose()
            {
                _collection = null;
                _currentNode = null;
                _isDisposed = true;
            }

            private bool CheckDisposed(bool throwException = true)
            {
                if (_isDisposed)
                {
                    if (throwException)
                    {
                        throw new ObjectDisposedException(nameof(Enumerator));
                    }
                    return true;
                }
                return false;
            }
        }
    }
}