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




export function Commands_GlobalAdmin(): RichEmbed {
    const embed = new RichEmbed()
        .setTitle("Global Admin Commands")
        .setColor("#FF0000")
        .addField("ping", "I will return super pong to let you know I am functional.")
        .addField("go die", "This will terminate my process. Automation will restart me.")
        .addField("show guilds", "I will print a list of all guilds using me.");

    return embed;
}

export function Commands_ServerAdmin(): RichEmbed {
    const embed = new RichEmbed()
        .setTitle("Server Admin Commands")
        .setColor("#335BFF")
        .addField('reset config', 'This will reset all of my configured settings for your guild. Note- Irreversible.')
        .addField('go away', 'I will remove myself from your guild permanently.')
        .addField('clear messages', 'I will delete all of the messages from this channel.')
        .addField('(enable|disable) update notifications', 'Enables or disable message to officers channel, when new features are added to me.')
        .addField('set admin role @RoleTag', 'Set the admin role for this bot.')
        .addField("test messages", "I will trigger all of the available war notifications, for testing purposes.");

    return embed;
}

export function Commands_Leader(): RichEmbed {
    const embed = new RichEmbed()
        .setTitle("Leader Commands")
        .setColor("#335BFF")
        .addField("show config", "I will show you my current configured settings.")
        .addField("set loot {URL}", "Sets the current loot URL.")
        .addField('set website {URL}', 'I will set the loot URL to the specified URL.')
        .addField('set member channel #ChannelTag', 'I will set channel for member notifications.')
        .addField('set officer channel #ChannelTag', 'I will set channel for officer notifications.')
        .addField('set member role @RoleTag', 'I will set the role for member notifications.')
        .addField('set officer role @RoleTag', 'I will set the role for officer notifications.')
        .addField('set leader role @RoleTag', 'I will set the role for notifications, AND permissions.')
        .addField('set member @UserTag', 'Promote/Demote user to member.')
        .addField('set officer @UserTag', 'Promote/Demote user to officer.')
        .addField('set leader @UserTag', 'Promote user to leader.')
        .addField('set name <My New NickName>', 'Changes my name.')
        .addField('kick @UserTag', 'Kicks specified user.')
        .addField('(enable|disable) war prep started', 'Enables or disables the announcement for war prep started.')
        .addField('(enable|disable) war prep ending', 'Enables or disables the announcement for war prep ending (Happens 15 minutes befor war.')
        .addField('(enable|disable) war started', 'Enables or disables the announcement for when the war starts.')
        .addField('(enable|disable) notifications', 'Enables or disable all notifications')
        .addField('(enable|disable) war reminder (1|2|3|4)', 'Enables or disable reminders for a specific war. 1 being the 2am CST war, 4 being the 8pm cst war')
        .addField('set war prep started {Message}', 'Specifies the war prep started message.')
        .addField('set war prep ending {Message}', 'Specifies the war prep ending message.')
        .addField('set war started {Message}', 'Specifies the war started message.')
        .addField('(enable|disable) notifications', 'Enables or disable all notifications')

    return embed;
}

export function Commands_Member(): RichEmbed {
    const embed = new RichEmbed()
        .setTitle("Member Commands")
        .setColor("#33FF74")
        .addField('loot', 'This will return your clans loot thread, if set by your leadership.');

    return embed;
}

export function Commands_Everybody(): RichEmbed {
    const embed = new RichEmbed()
        .setTitle("Commands for Everybody")
        .setColor("#C9C9C9")
        .addField('ping', 'I will return a pong, to let you know I am working')
        .addField('website', 'This will return your clans website, if configured by your leadership.')
        .addField('loot', 'This will return your clans loot thread, if set by your leadership.')
        .addField('stats', 'Display a few stats related to myself.')
        .setFooter("For more support, visit https://xtremeownage.com/index.php?forums/WARBOT/");

    return embed;
}

