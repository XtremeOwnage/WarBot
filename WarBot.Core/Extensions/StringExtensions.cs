using System;
using System.Collections.Generic;
using System.Text;

namespace WarBot.Core
{
    public static class StringExtensions
    {
        /// <summary>
        /// Used to ensure a string does not start with a specific character.
        /// Will also trim the string.
        /// </summary>
        /// <example>(", something") will return "something"</example>
        /// <param name="input">Input string</param>
        /// <param name="Character">The character to remove.</param>
        /// <returns>A string that does not start with the specified character</returns>
        public static string RemovePrecedingChar(this string input, char Character)
        {
            while (input.StartsWith(Character))
                input = input.Substring(1, input.Length - 1).Trim();
            return input;
        }
    }
}
