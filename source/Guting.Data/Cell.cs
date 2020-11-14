using System;

namespace Guting.Data
{
    public abstract class Cell : IComparable, IEquatable<Cell>
    {
        internal Cell(Table table, Column column, Row row)
        {
            Table = table;
            Column = column;
            Row = row;
            Id = table.GetNextId();
        }

        public Table Table { get; }
        public Column Column { get; }
        public Row Row { get; }
        public int Index { get; internal set; }
        public string Id { get; }
        public string Name { get; set; }

        public abstract object GetValue();

        public abstract void SetValue(object value);

        public abstract int CompareTo(object obj);

        public abstract bool Equals(Cell other);
    }

    public abstract class Cell<T> : Cell, IComparable<Cell<T>>, IEquatable<Cell<T>>
    {
        internal Cell(Table table, Column column, Row row) : base(table, column, row)
        {
        }

        public T Value { get; set; }

        public override sealed object GetValue() => Value;

        public override sealed void SetValue(object value) => Value = (T)value;

        public override int CompareTo(object obj)
        {
            if (obj is Cell<T> other)
            {
                return CompareTo(other);
            }
            return 0;
        }

        public abstract int CompareTo(Cell<T> other);

        public override bool Equals(object obj)
        {
            if (obj is Cell<T> other)
            {
                return Equals(other);
            }
            return false;
        }

        public abstract bool Equals(Cell<T> other);

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public override string ToString() => Value?.ToString() ?? string.Empty;

        public override bool Equals(Cell other)
        {
            if (other is Cell<T> o)
            {
                return Equals(o);
            }
            return false;
        }
    }

    public sealed class GenericCell<T> : Cell<T>, IEquatable<GenericCell<T>> where T : IComparable<T>, IEquatable<T>
    {
        internal GenericCell(Table table, Column column, Row row) : base(table, column, row)
        {
        }

        public override int CompareTo(Cell<T> other)
        {
            if (other != null)
            {
                if (Value != null && other.Value != null)
                {
                    return Value.CompareTo(other.Value);
                }
                if (Value != null && other.Value == null)
                {
                    return 1;
                }
                if (Value == null && other.Value != null)
                {
                    return -1;
                }
            }
            if (Value != null)
            {
                return 1;
            }
            return 0;
        }

        public override bool Equals(Cell<T> other)
        {
            if (other != null)
            {
                if (Value != null && other.Value != null)
                {
                    return Value.Equals(other.Value);
                }
            }
            return false;
        }

        public bool Equals(GenericCell<T> other)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class DefaultCell : Cell<object>, IEquatable<DefaultCell>
    {
        internal DefaultCell(Table table, Column column, Row row) : base(table, column, row)
        {
        }

        public override int CompareTo(Cell<object> other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(Cell<object> other)
        {
            if (Value != null && other != null)
            {
                return Value.Equals(other.Value);
            }
            return false;
        }

        public bool Equals(DefaultCell other)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class StringCell : Cell<string>, IEquatable<StringCell>
    {
        internal StringCell(Table table, Column column, Row row) : base(table, column, row)
        {
        }

        public override int CompareTo(Cell<string> other)
        {
            return string.Compare(Value, other?.Value);
        }

        public override bool Equals(Cell<string> other)
        {
            return string.Equals(Value, other?.Value);
        }

        public bool Equals(StringCell other)
        {
            throw new NotImplementedException();
        }
    }
}