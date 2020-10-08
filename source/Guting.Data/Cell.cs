using System;
using System.Collections;
using System.Collections.Generic;

namespace Guting.Data
{
    public abstract class Cell : IComparable
    {
        internal Cell(Table table, Column column, Row row)
        {
            Table = table;
            Column = column;
            Row = row;
            Id = table.GetNextId();
        }

        protected Table Table { get; }
        protected Column Column { get; }
        protected Row Row { get; }
        public int Index { get; internal set; }
        public string Id { get; }
        public string Name { get; set; }

        public abstract object GetValue();

        public abstract void SetValue(object value);

        public abstract int CompareTo(object obj);
    }

    public abstract class Cell<T> : Cell, IComparable<Cell<T>>, IEquatable<Cell<T>>
    {
        internal Cell(Table table, Column column, Row row) : base(table, column, row)
        {
        }

        public T Value { get; set; }

        public override sealed object GetValue() => Value;

        public override sealed void SetValue(object value) => Value = (T)value;

        public override int CompareTo(object obj) => CompareTo((Cell<T>)obj);

        public override bool Equals(object obj) => Equals((Cell<T>)obj);

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public abstract int CompareTo(Cell<T> other);

        public abstract bool Equals(Cell<T> other);
    }

    public sealed class GenericCell<T> : Cell<T> where T : IComparable<T>, IEquatable<T>
    {
        internal GenericCell(Table table, Column column, Row row) : base(table, column, row)
        {
        }
    }

    public sealed class DefaultCell : Cell<object>
    {
        internal DefaultCell(Table table, Column column, Row row) : base(table, column, row)
        {
        }
    }

    public sealed class StringCell : Cell<string>
    {
        internal StringCell(Table table, Column column, Row row) : base(table, column, row)
        {
        }
    }

    public sealed class CellEnumerator<TCell> : IEnumerator<TCell>, IEnumerator, IDisposable, ICloneable
        where TCell : Cell
    {
        private TCell _current;
        private int _index;

        internal CellEnumerator()
        {
            _index = -1;
            _current = null;
        }

        public TCell Current => _current;

        object IEnumerator.Current => _current;

        public object Clone()
        {
            return new CellEnumerator<TCell>();
        }

        public void Dispose()
        {
            _index = -1;
            _current = null;
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            _index = -1;
            _current = null;
        }
    }

    public sealed class CellCollection<TCell> : ICloneable, IEnumerable, ICollection, IList, IEnumerable<TCell>, ICollection<TCell>, IList<TCell>
        where TCell : Cell
    {
        internal CellCollection()
        {
        }
    }
}