using System;
using System.Collections;
using System.Collections.Generic;

namespace Guting.Data
{
    public abstract class Row
    {
        internal Row(Table table)
        {
            Table = table;
            Id = table.GetNextId();
        }

        protected Table Table { get; }
        public int Index { get; internal set; }
        public string Id { get; }
        public string Name { get; set; }
    }

    public abstract class Row<TCell> : Row
        where TCell : Cell
    {
        internal Row(Table table) : base(table)
        {
        }

        public CellCollection<TCell> Cells { get; private set; }
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

    public sealed class RowEnumerator<TRow> : IEnumerator<TRow>, IEnumerator, IDisposable, ICloneable
       where TRow : Row
    {
        private TRow _current;
        private int _index;

        internal RowEnumerator()
        {
            _index = -1;
            _current = null;
        }

        public TRow Current => _current;

        object IEnumerator.Current => _current;

        public object Clone()
        {
            return new RowEnumerator<TRow>();
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

    public sealed class RowCollection<TRow> : ICloneable, IEnumerable, ICollection, IList, IEnumerable<TRow>, ICollection<TRow>, IList<TRow>
      where TRow : Row
    {
        internal RowCollection()
        {
        }
    }
}