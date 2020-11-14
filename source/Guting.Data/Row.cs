using System;
using System.Collections.Generic;

namespace Guting.Data
{
    public abstract class Row : Node<Row>, IEquatable<Row>
    {
        internal Row(Table table)
        {
            Table = table;
            Id = table.GetNextId();
            Cells = new RowCells();
        }

        public Table Table { get; }
        public int Index { get; internal set; }
        public string Id { get; }
        public string Name { get; set; }
        internal RowCells Cells { get; }

        protected override bool IsEqualNodeValue(Row other)
        {
            return Equals(other);
        }

        public bool Equals(Row other)
        {
            if (other != null)
            {
                return other.Id == Id;
            }
            return false;
        }
    }

    public abstract class Row<TCell> : Row
        where TCell : Cell
    {
        internal Row(Table table) : base(table)
        {
        }

        public IEnumerable<TCell> GetCells()
        {
            foreach (var node in Cells)
            {
                yield return (TCell)node.Cell;
            }
        }
    }

    public sealed class GenericRow<T> : Row<GenericCell<T>> where T : IComparable<T>, IEquatable<T>
    {
        internal GenericRow(Table table) : base(table)
        {
        }
    }

    public sealed class DefaultRow : Row<DefaultCell>
    {
        internal DefaultRow(Table table) : base(table)
        {
        }
    }

    public sealed class StringRow : Row<StringCell>
    {
        internal StringRow(Table table) : base(table)
        {
        }
    }

    public sealed class CustomRow : Row<Cell>
    {
        internal CustomRow(Table table) : base(table)
        {
        }
    }

    public sealed class Rows : LinkedNodeList<Row>
    {
    }

    public sealed class RowCells : LinkedNodeList<TableRowCellNode>
    {
    }

    public sealed class TableRowCellNode : Node
    {
        internal TableRowCellNode(TableNode node)
        {
            Node = node;
        }

        public TableNode Node { get; }

        public Cell Cell => (Cell)Node.GetValue();

        internal override Node GetNext() => Node.GetNextRow();

        internal override Node GetPrevious() => Node.GetPreviousRow();

        internal override bool IsEqualNodeValue(Node other)
        {
            if (other is TableRowCellNode node)
            {
                return Cell.Equals(node.Cell);
            }
            return false;
        }

        internal override void SetNext(Node node)
        {
            Node.SetNextRow(node);
        }

        internal override void SetPrevious(Node node)
        {
            Node.SetPreviousRow(node);
        }
    }
}