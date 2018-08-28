export function ShowGuilds(bot: Client): RichEmbed[] {
    let Embeds: RichEmbed[] = new Array<RichEmbed>();

    let GuildsArray: Array<Guild> = bot.guilds.array().sort(function (a: Guild, b: Guild): number {
        return a.memberCount - b.memberCount;
    }).reverse();
    let Guilds: Array<Guild[]> = new Array<Guild[]>();

    while (GuildsArray.length > 0)
        Guilds.push(GuildsArray.splice(0, 20));

    Guilds.forEach(function (guilds: Guild[]) {
        let embed: RichEmbed = new RichEmbed()
            .setTitle("Current Guilds (And Member Count) Using WarBot");
        guilds.forEach(function (guild: Guild) {
            embed.addField(guild.name, guild.memberCount, false);
        });
        Embeds.push(embed);
    });


    return Embeds;
}

export function DefaultChannelSet(ch: TextChannel): RichEmbed {
    const embed = new RichEmbed()
        .setTitle("Default Member Channel Configured")
        .setDescription("My default announcements channel has been set to " + ch)
        .addField("To set a another channel", "bot, set member channel #ChannelTag");

    return embed;
}

export function OfficerChannelSet(ch: TextChannel): RichEmbed {
    const embed = new RichEmbed()
        .setTitle("Officer Notifications Channel Configured")
        .setDescription("My officer announcements channel has been set to " + ch)
        .addField("To set a another channel", "bot, set officer channel #ChannelTag");

    return embed;
}





