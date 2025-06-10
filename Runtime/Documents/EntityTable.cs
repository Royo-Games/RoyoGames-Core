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
        private ReaderWriterLockSlim Lck;
        internal Dictionary<string, Column> columns;
        private List<Row> rows;

        internal class Column
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
                table.Lck.EnterReadLock();
                try
                {
                    if (table.columns.TryGetValue(columnName, out Column column))
                    {
                        return Cells[column.Index];
                    }
                }
                finally
                {
                    table.Lck.ExitReadLock();
                }

                return "";
            }
        }
        public EntityTable()
        {
            Lck = new ReaderWriterLockSlim();
            columns = new Dictionary<string, Column>();
            rows = new List<Row>();
        }
        public void LoadFromString(string source)
        {
            Lck.EnterWriteLock();
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

                        foreach (var column in columns)
                        {
                            var cell = cells[i];

                            if (column.Value.IsUnique)
                            {
                                column.Value.Rows.Add(cell, row);
                            }

                            row.Cells.Add(cell);
                            i++;
                        }

                        rows.Add(row);
                    }
                }
            }
            finally
            {
                Lck.ExitWriteLock();
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
                columns.Add(columnName, column);
            }

            reader.Index = 1;
        }
        public Row GetRow(string uniqueColumnName, string key)
        {
            Lck.EnterReadLock();
            try
            {
                if (columns.TryGetValue(uniqueColumnName, out Column uniqueColumn))
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
                Lck.ExitReadLock();
            }
        }
        public Row GetRow(int rowIndex)
        {
            Lck.EnterReadLock();
            try
            {
                return rows[rowIndex];
            }
            finally
            {
                Lck.ExitReadLock();
            }
        }
        public void Clear()
        {
            rows.Clear();
            columns.Clear();
        }
    }
    public class EntityTable<T> where T : class, new()
    {
        private List<T> rows;
        private Dictionary<string, Dictionary<object, T>> uniqueColumns;

        private ReaderWriterLockSlim lck;

        public int Count
        {
            get
            {
                lck.EnterReadLock();
                try
                {
                    return rows.Count;
                }
                finally
                {
                    lck.ExitReadLock();
                }
            }
        }

        public EntityTable()
        {
            rows = new List<T>();
            uniqueColumns = new Dictionary<string, Dictionary<object, T>>();
            lck = new ReaderWriterLockSlim();
        }
        public T GetRow(int rowIndex)
        {
            lck.EnterReadLock();
            try
            {
                return rows[rowIndex];
            }
            finally
            {
                lck.ExitReadLock();
            }
        }
        public T GetRow(Predicate<T> predicate)
        {
            lck.EnterReadLock();
            try
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];

                    if (predicate(row))
                        return row;
                }

                return null;
            }
            finally
            {
                lck.ExitReadLock();
            }
        }
        public T GetRow(string uniqColumnName, object key)
        {
            lck.EnterReadLock();
            try
            {
                if (uniqueColumns.TryGetValue(uniqColumnName, out Dictionary<object, T> values))
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
                lck.ExitReadLock();
            }
        }
        public List<T> GetRows(Predicate<T> predicate)
        {
            lck.EnterReadLock();
            try
            {
                List<T> result = new List<T>();

                for (int i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];

                    if (predicate(row))
                        result.Add(row);
                }

                return result;
            }
            finally
            {
                lck.ExitReadLock();
            }
        }
        public List<T> GetRows()
        {
            lck.EnterReadLock();
            try
            {
                return rows.ToList();
            }
            finally
            {
                lck.ExitReadLock();
            }
        }
        public List<T> GetRows(int startIndex, int endIndex)
        {
            lck.EnterReadLock();
            try
            {
                List<T> result = new List<T>();

                for (int i = startIndex; i < endIndex; i++)
                {
                    result.Add(rows[i]);
                }

                return result;
            }
            finally
            {
                lck.ExitReadLock();
            }
        }
        public void LoadFromString(string source, bool hasColumns = true)
        {
            lck.EnterWriteLock();
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
                lck.ExitWriteLock();
            }
        }
        public void Clear()
        {
            rows.Clear();
            uniqueColumns.Clear();
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

            rows.Add(row);
        }
        private void AddUniqValue(string columnName, object value, T row)
        {
            if (!uniqueColumns.TryGetValue(columnName, out Dictionary<object, T> v))
            {
                v = new Dictionary<object, T>();
                uniqueColumns.Add(columnName, v);
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
