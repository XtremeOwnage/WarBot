//Logs errors and exceptions to the error channel.
export async function Error_async(cfg: BotCommonConfig | null, Method: string, Args: string | null, error: Error) {
    const embed = new RichEmbed()
        .setTitle("Exception")
        .setColor("#FF0000");

    if (cfg && cfg.Guild)
        embed.addField("Guild", cfg.Guild.name, true)

    if (Method)
        embed.addField("Method", Method, true);

    if (Args)
        embed.addField("Args", Args, true);

    if (error instanceof DiscordAPIError) {
        embed
            .addField("Type", "DiscordAPIError")
            .addField("Message", error.message);
    } else if (error) {
        embed
            .addField("Error", error)
            .addField("Error Name", error.name);
    }

    await sendEmbedToChannel(TargetChannel.Errors, embed);
}



//Logs a debug message to the general channel.
export async function DebugOutput_async(guild: Guild, msg: string) {
    const embed = new RichEmbed()
        .setTitle("Debug Output");
    if (guild)
        embed.addField("Guild", guild.name, true);

    embed.addField("Message", msg, false);

    await sendEmbedToChannel(TargetChannel.General, embed);
}