using Discord;

namespace WarBot.Core.Extensions;
public static class EmbedBuilderExtensions
{
    /// <summary>
    /// Calls <see cref="EmbedBuilder.AddField(string, object, bool)"/> but, will set value to <paramref name="ReplacementValue"/> if the provided <paramref name="value"/> is null OR whitespace.
    /// </summary>
    public static EmbedBuilder AddField(this EmbedBuilder builder, string name, object value, string ReplacementValue, bool inline = false)
    {
        string Value = value?.ToString() ?? ReplacementValue;
        if (string.IsNullOrWhiteSpace(Value))
            Value = ReplacementValue;

        return builder.AddField(name, Value, inline);
    }
}

