using System;
using System.Collections.Generic;
using System.Text;

namespace WarBot.Core.Helper
{
    public static class BooleanParser
    {
        /// <summary>
        /// Simple, stupid method to parse a boolean from a user's input.
        /// </summary>
        /// <param name="Input">Unformatted input text</param>
        /// <returns>Boolean if successfully parsed, NULL if unable to parse.</returns>
        public static bool? ParseBool(this string Input)
        {
            string raw = Input.Trim().ToLowerInvariant();

            var True = new HashSet<string>()
            {
                "yes",
                "y",
                "true",
                "1",
                "sure"
            };
            var False = new HashSet<string>{
                "no",
                "n",
                "false",
                "0",
            };

            if (True.Contains(raw))
                return true;
            else if (False.Contains(raw))
                return false;
            else
                //Return null - unable to parse results.
                return null;

        }
    }
}
