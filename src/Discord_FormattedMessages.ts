import { Client, TextChannel, GuildMember, RichEmbed, Guild } from 'discord.js';
import { BotCommonConfig } from './BotCommonConfig';



export function Statistics(cfg: BotCommonConfig): RichEmbed {
    let totalSeconds = (cfg.Bot.uptime / 1000);
    let hours = Math.floor(totalSeconds / 3600);
    totalSeconds %= 3600;
    let minutes = Math.floor(totalSeconds / 60);
    let seconds = totalSeconds % 60;
    const embed = new RichEmbed()
        .setTitle("Bot Stats")
        .addField("Process Uptime", `${hours} hours, ${minutes} minutes and ${seconds} seconds`)
        .addField("Guilds Using Bot", cfg.Bot.guilds.array().length);

    return embed;
}

export function Bot_Updated(Version: number, URL: string): RichEmbed {
    const embed = new RichEmbed()
        .setTitle("WarBot Updates")
        .setDescription("I have been updated to version " + Version + " 👏")
        .setFooter("To view my patch notes, open this embed.")
        .setURL(URL);

    return embed;
}

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

export function Bot_Joining(): RichEmbed {
    const embed = new RichEmbed()
        .setTitle("WarBOT")
        .setColor("#33FF74")
        .setDescription("Thanks for inviting me to your server. I will send you notifications related to Hustle Castle war events.")
        .addBlankField()
        .addField("For Help", "Just type 'bot, help'")
        .addField("For Support", "Either click this message or contact <@381654208073433091>.")
        .setURL("https://xtremeownage.com/index.php?forums/WARBOT/")
        .setImage("http://i1223.photobucket.com/albums/dd516/ericmck2000/download.jpg");

    return embed;
}

export function Bot_Leaving(): RichEmbed {
    const embed = new RichEmbed()
        .setTitle("GoodBye 😭")
        .setDescription("I am sorry I did not meet the expectations of your guild. If you wish to invite me back, you may click this embed.")
        .setURL("https://discordapp.com/oauth2/authorize?client_id=437983722193551363&scope=bot&permissions=0x00000008");

    return embed;
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

export function ShowConfig(cfg: BotCommonConfig): RichEmbed[] {
    const embed = new RichEmbed()
        .setTitle("Bot Configuration")
        .addField('Admin Role', cfg.Role_Admins)
        .addField('Leader Role', cfg.Role_Leaders)
        .addField('Officer Role', cfg.Role_Officers)
        .addField('Member Role', cfg.Role_Members)
        .addBlankField()
        .addField('Member Channel', cfg.CH_Members)
        .addField('Officer Channel', cfg.CH_Officers)
        .addBlankField()
        .addField('Website URL', cfg.Website_URL)
        .addField('Loot URL', cfg.Loot_URL)
        .addBlankField()
        .addField('UserName:', cfg.Nickname);

    const embed2 = new RichEmbed()
        .setTitle("Bot Configuration (2)")
        .addField("War Prep Started Notification Enabled", cfg.Notifications.WarStarted)
        .addField("War Prep Almost Over Notification Enabled", cfg.Notifications.WarPrepAlmostOver)
        .addField("War Started Notification Enabled", cfg.Notifications.WarStarted)
        .addField("Bot Update Notifications Enabled", cfg.Notifications.SendUpdateMessage)
        .addBlankField()
        .addField("War Prep Started Message", cfg.Notifications.WarPrepStartedMessage)
        .addField("War Prep Ending Message", cfg.Notifications.WarPrepEndingMessage)
        .addField("War Started Message", cfg.Notifications.WarStartedMessage)
        .addBlankField()
        .addField("War 1 Enabled (2am CST)", cfg.Notifications.War1Enabled)
        .addField("War 2 Enabled (8am CST)", cfg.Notifications.War2Enabled)
        .addField("War 3 Enabled (2pm CST)", cfg.Notifications.War3Enabled)
        .addField("War 4 Enabled (8pm CST)", cfg.Notifications.War4Enabled);

    return [embed, embed2];
}

export function KickUser(user: GuildMember): RichEmbed {
    const embed = new RichEmbed()
        .setTitle("User Kicked")
        .setColor("#FF0000")
        .setDescription("User " + user + " has been kicked from this guild.");

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

