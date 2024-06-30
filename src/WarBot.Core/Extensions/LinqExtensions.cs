using System.Collections.Generic;
using System.Linq;

namespace WarBot.Core.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// This simple extension, makes my code look nice and clean.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Input"></param>
        /// <param name="Output"></param>
        /// <returns></returns>
        public static bool IsNotNull<T>(this T Input, out T Output)
        {
            Output = Input;
            return Input != null;
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> Input, out IEnumerable<T> Output)
        {
            Output = Input;
            return Input != null && Input.Count() > 0;
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return YieldBatchElements(enumerator, batchSize - 1);
        }

        private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (int i = 0; i < batchSize && source.MoveNext(); i++)
                yield return source.Current;
        }
    }
}
