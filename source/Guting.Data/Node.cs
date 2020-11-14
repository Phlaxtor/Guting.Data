using System;

namespace Guting.Data
{
    public abstract class Node : IEquatable<Node>
    {
        protected Node()
        {
            NodeId = Tools.GetUniqueId();
        }

        public string NodeId { get; }

        public int Index { get; private set; }

        internal void SetIndex(int index) => Index = index;

        internal abstract Node GetPrevious();

        internal abstract Node GetNext();

        internal abstract void SetPrevious(Node node);

        internal abstract void SetNext(Node node);

        internal abstract bool IsEqualNodeValue(Node other);

        internal void CalculateIndex()
        {
            var previous = GetPrevious();
            if (previous != null)
            {
                SetIndex(previous.Index + 1);
            }
            else
            {
                SetIndex(0);
            }
        }

        public virtual bool Equals(Node other)
        {
            if (other != null)
            {
                return string.CompareOrdinal(NodeId, other.NodeId) == 0;
            }
            return false;
        }
    }

    public abstract class Node<TNode> : Node where TNode : Node
    {
        protected Node(TNode previous = null, TNode next = null) : base()
        {
            Previous = previous;
            Next = next;
        }

        public TNode Previous { get; private set; }

        public TNode Next { get; private set; }

        protected abstract bool IsEqualNodeValue(TNode other);

        internal override sealed Node GetPrevious() => Previous;

        internal override sealed Node GetNext() => Next;

        internal override sealed void SetPrevious(Node node) => Previous = (TNode)node;

        internal override sealed void SetNext(Node node) => Next = (TNode)node;

        internal override sealed bool IsEqualNodeValue(Node other)
        {
            if (other is TNode node)
            {
                return IsEqualNodeValue(node);
            }
            return false;
        }
    }

    public abstract class TableNode
    {
        internal abstract object GetValue();

        internal abstract Node GetColumn();

        internal abstract Node GetRow();

        internal abstract Node GetPreviousColumn();

        internal abstract Node GetNextColumn();

        internal abstract Node GetPreviousRow();

        internal abstract Node GetNextRow();

        internal abstract void SetPreviousRow(Node node);

        internal abstract void SetNextRow(Node node);

        internal abstract void SetPreviousColumn(Node node);

        internal abstract void SetNextColumn(Node node);
    }

    public sealed class TableNode<TValue, TColumnNode, TRowNode> : TableNode
        where TValue : IEquatable<TValue>
        where TColumnNode : Node
        where TRowNode : Node
    {
        internal TableNode(TValue value, Func<TableNode, TColumnNode> columnNodeCreator, Func<TableNode, TRowNode> rowNodeCreator)
        {
            Value = value;
            Column = columnNodeCreator(this);
            Row = rowNodeCreator(this);
        }

        public TValue Value { get; }

        public TColumnNode Column { get; }

        public TRowNode Row { get; }

        public TColumnNode PreviousColumn => (TColumnNode)Column.GetPrevious();

        public TColumnNode NextColumn => (TColumnNode)Column.GetNext();

        public TRowNode PreviousRow => (TRowNode)Row.GetPrevious();

        public TRowNode NextRow => (TRowNode)Row.GetNext();

        internal override object GetValue() => Value;

        internal override Node GetColumn() => Column;

        internal override Node GetRow() => Row;

        internal override Node GetPreviousColumn() => PreviousColumn;

        internal override Node GetNextColumn() => NextColumn;

        internal override Node GetPreviousRow() => PreviousRow;

        internal override Node GetNextRow() => NextRow;

        internal override void SetPreviousRow(Node node) => Row.SetPrevious(node);

        internal override void SetNextRow(Node node) => Row.SetNext(node);

        internal override void SetPreviousColumn(Node node) => Column.SetPrevious(node);

        internal override void SetNextColumn(Node node) => Column.SetNext(node);
    }

    public sealed class GenericNode<TValue> : Node<GenericNode<TValue>>
        where TValue : IEquatable<TValue>
    {
        internal GenericNode(TValue value, GenericNode<TValue> previous = null, GenericNode<TValue> next = null) : base(previous, next)
        {
            Value = value;
        }

        public TValue Value { get; }

        protected override bool IsEqualNodeValue(GenericNode<TValue> other)
        {
            if (other == null)
            {
                return false;
            }
            return other.Value.Equals(Value);
        }
    }
}