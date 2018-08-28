using System.Collections.Generic;
using Tababular;
namespace WarBot.Core
{
    /// <summary>
    /// Wrapper class for Tababluar library.
    /// https://github.com/rebus-org/Tababular
    /// </summary>
    public static class TableHelper
    {
        public static string FormatTable(IEnumerable<dynamic> Rows)
        {
            return new TableFormatter()
                .FormatObjects(Rows);
        }

        public static string FormatTable<T>(IEnumerable<IDictionary<string, T>> Input)
        {
            return new TableFormatter()
                .FormatDictionaries<T>(Input);
        }
    }
}
