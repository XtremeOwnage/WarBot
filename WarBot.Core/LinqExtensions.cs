using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace WarBot.Core
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
        //public static bool IsNotNull<T>(this T Input, out T Output)
        //{
        //    Output = Input;
        //    return Input != null;
        //}

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
