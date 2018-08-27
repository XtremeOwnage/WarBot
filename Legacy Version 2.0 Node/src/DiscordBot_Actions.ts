export async function AdminMessage_async(cfg: BotCommonConfig, from: string, msg: string) {
    if (!cfg.CH_Officers) {
        return await LOG.DebugOutput_async(cfg.Guild, "No officers channel configured to send admin message to.");
    }
    const embed = new RichEmbed()
        .setTitle("WarBot Global Message")
        .setColor("#ad42f4")
        .addField("From", from)
        .setDescription(msg)
        .setFooter("Note- We cannot see responses to this message. To respond, please join the WARBOT test server @ https://discord.gg/ywSDGCf");
    try {
        cfg.CH_Officers.send(embed);
        await LOG.DebugOutput_async(cfg.Guild, "Sent global message successfully.");
    } catch (err) {
        await LOG.Error_async(cfg, "AdminMessage", null, err);
    }
}

export function ComeOnline(bot: Client) {
    console.info("Coming online.");
    bot.user.setAFK(false);
    bot.user.setStatus("online");
    bot.user.setActivity('Hustle Castle', { type: 'PLAYING' });
}
export function GoAway(bot: Client) {
    console.info("Going AFK.");
    bot.user.setActivity('');
    bot.user.setAFK(true);
    bot.user.setStatus('idle');
}



