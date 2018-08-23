using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace WarBot.Core
{
    public static class EmbedHelper
    {
        /// <summary>
        /// Embed builder does not allow null values. This replaces null with an empty string.
        /// </summary>
        /// <param name="eb"></param>
        public static EmbedBuilder CleanNullFields(this EmbedBuilder eb)
        {
            eb.Fields.ForEach(o =>
            {
                if (string.IsNullOrEmpty(o.Name))
                    o.Name = "-";
                if (o.Value == null)
                    o.Value = "-";
            });
            return eb;
        }
        public static EmbedBuilder AddBlankLine(this EmbedBuilder eb)
        {
            eb.Fields.Add(new EmbedFieldBuilder
            {
                Name = "_ _",
                Value = "_ _",
                IsInline = false,
            });
            return eb;
        }
        public static EmbedBuilder AddField_ex(this EmbedBuilder eb, string Name, object Value, bool Inline = false)
        {
            eb.AddField(new EmbedFieldBuilder
            {
                Name = string.IsNullOrEmpty(Name) ? "" : Name,
                Value = Value == null || string.IsNullOrWhiteSpace(Value.ToString()) ? "_ _" : Value,
                IsInline = Inline,
            });
            return eb;
        }
    }
}
