using System;
using System.Collections;
using System.Collections.Generic;

namespace Guting.Data
{
    public abstract class Column
    {
        internal Column(Table table)
        {
            Table = table;
            Id = table.GetNextId();
        }

        protected Table Table { get; }
        public int Index { get; internal set; }
        public string Id { get; }
        public string Name { get; set; }
        internal abstract Cell GetCell(Row row);
    }

    public abstract class Column<TCell> : Column where TCell : Cell
    {
        internal Column(Table table) : base(table)
        {
        }

        public CellCollection<TCell> Cells { get; private set; }
    }

    public sealed class GenericColumn<T> : Column<GenericCell<T>> where T : IComparable<T>, IEquatable<T>
    {
        internal GenericColumn(Table table) : base(table)
        {
        }

        internal override Cell GetCell(Row row)
        {
            //check if cell exist
            return new GenericCell<T>(Table, this, row);
        }
    }

    public sealed class DefaultColumn : Column<DefaultCell>
    {
        internal DefaultColumn(Table table) : base(table)
        {
        }

        internal override Cell GetCell(Row row)
        {
            //check if cell exist
            return new DefaultCell(Table, this, row);
        }
    }

    public sealed class StringColumn : Column<StringCell>
    {
        internal StringColumn(Table table) : base(table)
        {
        }

        internal override Cell GetCell(Row row)
        {
            //check if cell exist
            return new StringCell(Table, this, row);
        }
    }

    public sealed class ColumnEnumerator<TColumn> : IEnumerator<TColumn>, IEnumerator, IDisposable, ICloneable
       where TColumn : Column
    {
        private TColumn _current;
        private int _index;

        internal ColumnEnumerator()
        {
            _index = -1;
            _current = null;
        }

        public TColumn Current => _current;

        object IEnumerator.Current => _current;

        public object Clone()
        {
            return new ColumnEnumerator<TColumn>();
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

    public sealed class ColumnCollection<TColumn> : ICloneable, IEnumerable, ICollection, IList, IEnumerable<TColumn>, ICollection<TColumn>, IList<TColumn>
      where TColumn : Column
    {
        internal ColumnCollection()
        {
        }
    }
}