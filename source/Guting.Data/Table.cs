using System;
using System.Collections.Generic;

namespace Guting.Data
{
    public abstract class Table
    {
        protected Table()
        {
            Columns = new Columns();
            Rows = new Rows();
        }

        internal string GetNextId()
        {
            return Tools.GetUniqueId();
        }

        public abstract void AddColumn(string name);

        public abstract void AddRow(params object[] values);

        internal Columns Columns { get; }

        internal Rows Rows { get; }
    }

    public abstract class Table<TCell, TColumn, TRow> : Table
        where TCell : Cell, IEquatable<TCell>
        where TColumn : Column
        where TRow : Row
    {
        protected Table() : base()
        {
        }

        public IEnumerable<TCell> GetCells()
        {
            foreach (var row in Rows)
            {
                foreach (var node in row.Cells)
                {
                    yield return (TCell)node.Cell;
                }
            }
        }

        public IEnumerable<TColumn> GetColumns()
        {
            return Columns.Cast<TColumn>();
        }

        public IEnumerable<TRow> GetRows()
        {
            return Rows.Cast<TRow>();
        }

        protected abstract TColumn CreateColumn();

        public TColumn NewColumn()
        {
            var result = CreateColumn();
            Columns.Add(result);
            //update all rows
            return result;
        }

        public override sealed void AddColumn(string name)
        {
            var result = NewColumn();
            result.Name = name;
        }

        protected abstract TRow CreateRow();

        public TRow NewRow()
        {
            var result = CreateRow();
            Rows.Add(result);
            return result;
        }

        public override sealed void AddRow(params object[] values)
        {
            if (values.Length > Columns.Count)
            {
                throw new IndexOutOfRangeException($"Too many values provided when adding a new row");
            }
            if (values.Length < Columns.Count)
            {
                throw new ArgumentException($"Too many values provided when adding a new row", nameof(values));
            }

            var columns = Columns.ToArray();
            for (int i = 0; i < Columns.Count; i++)
            {
                var value = values[i];
                var column = columns[i];
                if (column.CanCreateCell(value) == false)
                {
                    var type = value != null ? value.GetType().Name : "null";
                    throw new ArgumentException($"Provided value for column '{column.Name}' #{column.Index} is of wrong type '{type}'", nameof(values));
                }
            }

            var result = NewRow();
            for (int i = 0; i < Columns.Count; i++)
            {
                var value = values[i];
                var column = columns[i];
                var node = AddNewNode((TCell)value, column, result);
                //////////////////////////////////////////////
            }
        }

        private TableNode AddNewNode(TCell cell, Column column, Row row)
        {
            var node = new TableNode<TCell, TableColumnCellNode, TableRowCellNode>(cell, n => new TableColumnCellNode(n), n => new TableRowCellNode(n));
            column.Cells.Add(node.Column);
            row.Cells.Add(node.Row);
            return node;
        }
    }

    public sealed class GenericTable<T> : Table<GenericCell<T>, GenericColumn<T>, GenericRow<T>> where T : IComparable<T>, IEquatable<T>
    {
        public GenericTable() : base()
        {
        }

        protected override GenericColumn<T> CreateColumn()
        {
            throw new NotImplementedException();
        }

        protected override GenericRow<T> CreateRow()
        {
            throw new NotImplementedException();
        }
    }

    public sealed class DefaultTable : Table<DefaultCell, DefaultColumn, DefaultRow>
    {
        public DefaultTable() : base()
        {
        }

        protected override DefaultColumn CreateColumn()
        {
            throw new NotImplementedException();
        }

        protected override DefaultRow CreateRow()
        {
            throw new NotImplementedException();
        }
    }

    public sealed class StringTable : Table<StringCell, StringColumn, StringRow>
    {
        public StringTable() : base()
        {
        }

        protected override StringColumn CreateColumn()
        {
            throw new NotImplementedException();
        }

        protected override StringRow CreateRow()
        {
            throw new NotImplementedException();
        }
    }

    public sealed class CustomTable : Table<Cell, Column, Row>
    {
        public CustomTable() : base()
        {
        }

        protected override Column CreateColumn()
        {
            throw new NotImplementedException();
        }

        protected override Row CreateRow()
        {
            throw new NotImplementedException();
        }
    }
}