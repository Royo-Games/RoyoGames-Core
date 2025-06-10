using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace RoyoGames.Document
{
    public class EntityTable
    {
        private ReaderWriterLockSlim _lck;
        private Dictionary<string, Column> _columns;
        private List<Row> _rows;

        public class Column
        {
            public int Index { get; set; }
            public bool IsUnique { get; set; }
            public Dictionary<string, Row> Rows { get; set; }
        }

        public class Row
        {
            internal List<string> Cells { get; set; }
            private EntityTable table;

            public Row(EntityTable table)
            {
                this.table = table;
                Cells = new List<string>();
            }
            public string GetValue(string columnName)
            {
                table._lck.EnterReadLock();
                try
                {
                    if (table._columns.TryGetValue(columnName, out Column column))
                    {
                        return Cells[column.Index];
                    }
                }
                finally
                {
                    table._lck.ExitReadLock();
                }

                return "";
            }
        }
        public EntityTable()
        {
            _lck = new ReaderWriterLockSlim();
            _columns = new Dictionary<string, Column>();
            _rows = new List<Row>();
        }
        public void LoadFromString(string source)
        {
            _lck.EnterWriteLock();
            try
            {
                using (CsvReader reader = new CsvReader(source))
                {
                    CreateColumns(reader);

                    while (reader.Available)
                    {
                        var row = new Row(this);
                        var cells = reader.Read();

                        int i = 0;

                        foreach (var column in _columns)
                        {
                            var cell = cells[i];

                            if (column.Value.IsUnique)
                            {
                                column.Value.Rows.Add(cell, row);
                            }

                            row.Cells.Add(cell);
                            i++;
                        }

                        _rows.Add(row);
                    }
                }
            }
            finally
            {
                _lck.ExitWriteLock();
            }
        }
        private void CreateColumns(CsvReader reader)
        {
            var columnNames = reader.Read();

            for (int i = 0; i < columnNames.Count; i++)
            {
                var columnName = columnNames[i];

                var column = new Column();
                column.Index = i;

                if (columnName.EndsWith("[Unique]"))
                {
                    column.IsUnique = true;
                    column.Rows = new Dictionary<string, Row>();
                    columnName = columnName.Replace("[Unique]", "");
                }

                columnName = columnName.Trim();
                _columns.Add(columnName, column);
            }

            reader.Index = 1;
        }
        public Row GetRow(string uniqueColumnName, string key)
        {
            _lck.EnterReadLock();
            try
            {
                if (_columns.TryGetValue(uniqueColumnName, out Column uniqueColumn))
                {
                    if (uniqueColumn.IsUnique)
                    {
                        if (uniqueColumn.Rows.TryGetValue(key, out Row row))
                        {
                            return row;
                        }
                    }
                }

                return null;
            }
            finally
            {
                _lck.ExitReadLock();
            }
        }

        public string[] GetColumns()
        {
            string[] columnNames = new string[_columns.Count];

            int index = 0;

            foreach (var item in _columns)
            {
                columnNames[index] = item.Key;
                index++;
            }

            return columnNames;
        }

        public Row GetRow(int rowIndex)
        {
            _lck.EnterReadLock();
            try
            {
                return _rows[rowIndex];
            }
            finally
            {
                _lck.ExitReadLock();
            }
        }
        public void Clear()
        {
            _rows.Clear();
            _columns.Clear();
        }
    }
    public class EntityTable<T> where T : class, new()
    {
        private List<T> _rows;
        private Dictionary<string, Dictionary<object, T>> _uniqueColumns;

        private ReaderWriterLockSlim _lck;

        public int Count
        {
            get
            {
                _lck.EnterReadLock();
                try
                {
                    return _rows.Count;
                }
                finally
                {
                    _lck.ExitReadLock();
                }
            }
        }

        public EntityTable()
        {
            _rows = new List<T>();
            _uniqueColumns = new Dictionary<string, Dictionary<object, T>>();
            _lck = new ReaderWriterLockSlim();
        }

        public T GetRow(int rowIndex)
        {
            _lck.EnterReadLock();
            try
            {
                return _rows[rowIndex];
            }
            finally
            {
                _lck.ExitReadLock();
            }
        }

        public T GetRow(Predicate<T> predicate)
        {
            _lck.EnterReadLock();
            try
            {
                for (int i = 0; i < _rows.Count; i++)
                {
                    var row = _rows[i];

                    if (predicate(row))
                        return row;
                }

                return null;
            }
            finally
            {
                _lck.ExitReadLock();
            }
        }

        public T GetRow(string uniqColumnName, object key)
        {
            _lck.EnterReadLock();
            try
            {
                if (_uniqueColumns.TryGetValue(uniqColumnName, out Dictionary<object, T> values))
                {
                    if (values.TryGetValue(key, out T row))
                    {
                        return row;
                    }
                }

                return null;
            }
            finally
            {
                _lck.ExitReadLock();
            }
        }

        public List<T> GetRows(Predicate<T> predicate)
        {
            _lck.EnterReadLock();
            try
            {
                List<T> result = new List<T>();

                for (int i = 0; i < _rows.Count; i++)
                {
                    var row = _rows[i];

                    if (predicate(row))
                        result.Add(row);
                }

                return result;
            }
            finally
            {
                _lck.ExitReadLock();
            }
        }

        public List<T> GetRows()
        {
            _lck.EnterReadLock();
            try
            {
                return _rows.ToList();
            }
            finally
            {
                _lck.ExitReadLock();
            }
        }

        public List<T> GetRows(int startIndex, int endIndex)
        {
            _lck.EnterReadLock();
            try
            {
                List<T> result = new List<T>();

                for (int i = startIndex; i < endIndex; i++)
                {
                    result.Add(_rows[i]);
                }

                return result;
            }
            finally
            {
                _lck.ExitReadLock();
            }
        }

        public void LoadFromString(string source, bool hasColumns = true)
        {
            _lck.EnterWriteLock();
            try
            {
                using (CsvReader reader = new CsvReader(source))
                {
                    if (hasColumns)
                        reader.Index = 1;

                    while (reader.Available)
                    {
                        var cells = reader.Read();
                        AddRow(cells);
                    }
                }
            }
            finally
            {
                _lck.ExitWriteLock();
            }
        }

        public void Clear()
        {
            _rows.Clear();
            _uniqueColumns.Clear();
        }

        private void AddRow(List<string> cells)
        {
            T row = new T();

            Type type = typeof(T);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var value = StringToObject(field.FieldType, cells[i]);
                field.SetValue(row, value);

                if (field.IsDefined(typeof(UniqueValue)))
                    AddUniqValue(field.Name, value, row);
            }

            _rows.Add(row);
        }

        private void AddUniqValue(string columnName, object value, T row)
        {
            if (!_uniqueColumns.TryGetValue(columnName, out Dictionary<object, T> v))
            {
                v = new Dictionary<object, T>();
                _uniqueColumns.Add(columnName, v);
            }

            v.Add(value, row);
        }

        private object StringToObject(Type type, string str)
        {
            if (type.IsEnum)
            {
                return Enum.Parse(type, str);
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean: return bool.Parse(str);
                case TypeCode.Char: return char.Parse(str);
                case TypeCode.SByte: return sbyte.Parse(str);
                case TypeCode.Byte: return byte.Parse(str);
                case TypeCode.Int16: return short.Parse(str);
                case TypeCode.UInt16: return ushort.Parse(str);
                case TypeCode.Int32: return int.Parse(str);
                case TypeCode.UInt32: return uint.Parse(str);
                case TypeCode.Int64: return long.Parse(str);
                case TypeCode.UInt64: return ulong.Parse(str);
                case TypeCode.Single: return float.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
                case TypeCode.Double: return double.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
                case TypeCode.Decimal: return decimal.Parse(str);
                case TypeCode.DateTime: return DateTime.Parse(str);
                case TypeCode.String: return str;
            }

            return null;
        }
    }
}
