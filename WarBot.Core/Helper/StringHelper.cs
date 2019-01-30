namespace WarBot.Core.Helper
{
    public static class StringHelper
    {
        /// <summary>
        /// Nicely truncate a string. If the string is over the max length, it will be trimmed, with ... added to indicate it was truncated.
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static string Truncate(string Input, int MaxLength)
        {
            if (Input == null)
                return null;

            if (Input.Length > MaxLength)
                Input = Input.Substring(0, MaxLength - 3) + "...";

            return Input;
        }
    }
}
