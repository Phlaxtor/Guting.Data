using System;
using System.Collections.Generic;

namespace Guting.Data
{
    public abstract class Column : Node<Column>, IEquatable<Column>
    {
        protected Column(Table table)
        {
            Table = table;
            Id = table.GetNextId();
            Cells = new ColumnCells();
        }

        public Table Table { get; }
        public int Index { get; internal set; }
        public string Id { get; }
        public string Name { get; set; }
        internal ColumnCells Cells { get; }

        protected override bool IsEqualNodeValue(Column other)
        {
            return Equals(other);
        }

        internal abstract bool CanCreateCell(object value);

        public bool Equals(Column other)
        {
            if (other != null)
            {
                return other.Id == Id;
            }
            return false;
        }
    }

    public abstract class Column<TCell, T> : Column
        where TCell : Cell<T>, IEquatable<TCell>
    {
        protected Column(Table table) : base(table)
        {
        }

        internal override bool CanCreateCell(object value)
        {
            if (value is T)
            {
                return true;
            }
            if (value == null && default(T) == null)
            {
                return true;
            }
            return false;
        }

        protected abstract TCell CreateCell(Row row);

        public IEnumerable<TCell> GetCells()
        {
            foreach (var node in Cells)
            {
                yield return (TCell)node.Cell;
            }
        }
    }

    public sealed class GenericColumn<T> : Column<GenericCell<T>, T> where T : IComparable<T>, IEquatable<T>
    {
        internal GenericColumn(Table table) : base(table)
        {
        }

        protected override GenericCell<T> CreateCell(Row row)
        {
            return new GenericCell<T>(Table, this, row);
        }
    }

    public sealed class DefaultColumn : Column<DefaultCell, object>
    {
        internal DefaultColumn(Table table) : base(table)
        {
        }

        protected override DefaultCell CreateCell(Row row)
        {
            return new DefaultCell(Table, this, row);
        }
    }

    public sealed class StringColumn : Column<StringCell, string>
    {
        internal StringColumn(Table table) : base(table)
        {
        }

        protected override StringCell CreateCell(Row row)
        {
            return new StringCell(Table, this, row);
        }
    }

    public sealed class Columns : LinkedNodeList<Column>
    {
    }

    public sealed class ColumnCells : LinkedNodeList<TableColumnCellNode>
    {
    }

    public sealed class TableColumnCellNode : Node
    {
        internal TableColumnCellNode(TableNode node)
        {
            Node = node;
        }

        public TableNode Node { get; }

        public Cell Cell => (Cell)Node.GetValue();

        internal override Node GetNext() => Node.GetNextColumn();

        internal override Node GetPrevious() => Node.GetPreviousColumn();

        internal override bool IsEqualNodeValue(Node other)
        {
            if (other is TableColumnCellNode node)
            {
                return Cell.Equals(node.Cell);
            }
            return false;
        }

        internal override void SetNext(Node node)
        {
            Node.SetNextColumn(node);
        }

        internal override void SetPrevious(Node node)
        {
            Node.SetPreviousColumn(node);
        }
    }
}