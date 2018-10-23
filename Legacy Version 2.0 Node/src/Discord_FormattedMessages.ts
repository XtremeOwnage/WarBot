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





