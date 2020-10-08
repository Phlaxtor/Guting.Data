using System;
using System.Dynamic;

namespace Guting.Data
{
    public abstract class Table
    {
        private int _lastId = 0;
        private int _lastCellIndex = 0;
        private int _lastColumnIndex = 0;
        private int _lastRowIndex = 0;

        protected Table()
        {
        }

        internal string GetNextId()
        {
            return $"i{_lastId++}";
        }
    }

    public abstract class Table<TCell, TColumn, TRow> : Table
        where TCell : Cell
        where TColumn : Column
        where TRow : Row
    {
        protected Table() : base()
        {
            Cells = new CellCollection<TCell>();
            Columns = new ColumnCollection<TColumn>();
            Rows = new RowCollection<TRow>();
        }

        public CellCollection<TCell> Cells { get; }
        public ColumnCollection<TColumn> Columns { get; }
        public RowCollection<TRow> Rows { get; }
    }

    public sealed class GenericTable<T> : Table<GenericCell<T>, GenericColumn<T>, GenericRow<T>> where T : IComparable<T>, IEquatable<T>
    {
        public GenericTable() : base()
        {
        }
    }

    public sealed class DefaultTable : Table<DefaultCell, DefaultColumn, DefaultRow>
    {
        public DefaultTable() : base()
        {
        }
    }

    public sealed class StringTable : Table<StringCell, StringColumn, StringRow>
    {
        public StringTable()
        {
        }
    }

    public sealed class CustomTable : Table<Cell, Column, Row>
    {
        public CustomTable() : base()
        {
        }
    }
}