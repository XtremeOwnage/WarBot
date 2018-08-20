using System;
using System.Collections.Generic;
using System.Text;

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
        public static bool IsNotNull<T>(this T Input, out T Output)
        {
            Output = Input;
            return Input != null;
        }
    }
}
