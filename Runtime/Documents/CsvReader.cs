using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RoyoGames.Document
{
    public class CsvReader : IDisposable
    {
        public int Index { get; set; }

        public bool Available
        {
            get
            {
                return Index < lines.Length;
            }
        }

        const char escapeChar = '"';
        const char splitChar = ',';

        private StringBuilder sb = new StringBuilder();

        private enum Escape
        {
            None,
            Begin,
            End
        }

        private string[] lines;

        public CsvReader(string source)
        {
            lines = source.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }
        public List<string> Read()
        {
            string line = lines[Index++];

            bool inEscape = false;
            bool priorEscape = false;

            sb.Clear();

            List<string> cells = new List<string>();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                switch (c)
                {
                    case escapeChar:
                        if (!inEscape)
                            inEscape = true;
                        else
                        {
                            if (!priorEscape)
                            {
                                if (i + 1 < line.Length && line[i + 1] == escapeChar)
                                    priorEscape = true;
                                else
                                    inEscape = false;
                            }
                            else
                            {
                                sb.Append(c);
                                priorEscape = false;
                            }
                        }
                        break;
                    case splitChar:
                        if (inEscape)
                            sb.Append(c);
                        else
                        {
                            cells.Add(sb.ToString());
                            sb.Length = 0;
                        }
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            cells.Add(sb.ToString());

            return cells;
        }
        public void Dispose()
        {
            lines = null;
        }
    }
}
