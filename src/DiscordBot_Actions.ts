import { Client, Channel, Guild, TextChannel, Role, RichEmbed, Message, GuildMember, PermissionString, MessageMentions, Collection, DiscordAPIError } from 'discord.js';
import { BotCommonConfig } from './BotCommonConfig';
import * as Updates from './Updates';
import { RoleLevel } from './RoleLevel';
import * as LOG from './Discord_Logging';
import * as Messages from './Discord_FormattedMessages';
import { checkPermission } from './Discord_Utils';


export async function Bot_JoinedServer(cfg: BotCommonConfig) {
    await LOG.Guild_Created_async(cfg.Guild);
    await ConnectedToServer(cfg);
    if (cfg.CH_Members) {
        await cfg.CH_Members.send(await Messages.Bot_Joining());
    }
}
export async function Bot_LeftServer(cfg: BotCommonConfig) {
    //Don't log a message to the guild, because... we are no longer in it!!
    await LOG.Guild_Deleted_async(cfg.Guild);
}
export function ConnectedToServer(cfg: BotCommonConfig) {
    console.info("Connected to Guild " + cfg.Guild.name);

    Updates.UpdateCheck(cfg);

    if (cfg.Nickname) {
        if (checkPermission(cfg.Guild.me, "MANAGE_NICKNAMES") && checkPermission(cfg.Guild.me, "CHANGE_NICKNAME")) {
            cfg.Guild.me.setNickname(cfg.Nickname);
        }
    }
}
export async function ClearMessages_async(ch: Channel) {
    if (ch instanceof TextChannel) {
        if (!checkPermission(ch.guild.me, "ADMINISTRATOR")) {
            return ch.send("I am missing the 'ADMINISTRATOR' permission to perform this action.");
        }
        while (true) {
            let Messages: Collection<string, Message> = await ch.fetchMessages({ limit: 100 });

            //Messages are empty.
            if (Messages.array().length == 0)
                break;

            await ch.bulkDelete(Messages);
        }
    } else {
        //Can't handle this type of channel yet.
    }
}
export async function MassNotify(cfg: BotCommonConfig, message: string) {
    cfg.Bot.guilds.forEach(async function (g: Guild) {
        //Determine logic to send mass notification.
    });

}
export async function HandleDeletedRole_async(Role: Role, cfg: BotCommonConfig) {
    if (cfg.Role_Admins && Role.id == cfg.Role_Admins.id) {
        if (cfg.CH_Members)
            await cfg.CH_Members.send(`Note, Role ${Role.name} was deleted from this guild. It was configured as the admins role for this bot. Please set a new role using 'bot, set admins role @RoleName.`);
        cfg.Role_Admins = null;
    }
    if (cfg.Role_Leaders && Role.id == cfg.Role_Leaders.id) {
        if (cfg.CH_Members)
            await cfg.CH_Members.send(`Note, Role ${Role.name} was deleted from this guild. It was configured as the leaders role for this bot. Please set a new role using 'bot, set leaders role @RoleName'`);
        cfg.Role_Leaders = null;
    }
    if (cfg.Role_Members && Role.id == cfg.Role_Members.id) {
        if (cfg.CH_Members)
            await cfg.CH_Members.send(`Note, Role ${Role.name} was deleted from this guild. It was configured as the members role for this bot. Please set a new role using 'bot, set members role @RoleName'`);
        cfg.Role_Members = null;
    }
    if (cfg.Role_Officers && Role.id == cfg.Role_Officers.id) {
        if (cfg.CH_Members)
            await cfg.CH_Members.send(`Note, Role ${Role.name} was deleted from this guild. It was configured as the officers role for this bot. Please set a new role using 'bot, set officers role @RoleName'`);
        cfg.Role_Officers = null;
    }
}
export async function HandleDeletedChannel_async(Channel: TextChannel, cfg: BotCommonConfig) {
    if (cfg.CH_Members && Channel.id == cfg.CH_Members.id) {
        //Set the default channel, to the officers channel.
        if (cfg.CH_Officers) {
            cfg.CH_Members = cfg.CH_Officers;
        } else {
            cfg.CH_Members = cfg.FindDefaultMembersChannel();
        }
        if (cfg.CH_Members) {
            await cfg.CH_Members.send(Messages.DefaultChannelSet(cfg.CH_Members));
        }
    }
    if (cfg.CH_Officers && Channel.id == cfg.CH_Officers.id) {
        if (cfg.CH_Members) {
            cfg.CH_Officers = cfg.CH_Members;
            await cfg.CH_Members.send(Messages.OfficerChannelSet(cfg.CH_Members));
        }
    }
}


export function KickMember(cfg: BotCommonConfig, msg: Message) {
    let user: GuildMember = msg.mentions.members.last();
    if (!user) {
        msg.reply("Did you forget to mention a user?");
        return;
    } else if (cfg.Bot.user.id == user.id) {
        msg.reply("Sorry, I do not wish to kick myself.");
        return;
    }

    if (checkPermission(cfg.Guild.me, "KICK_MEMBERS")) {
        if (cfg.Guild.me.highestRole > user.highestRole) {
            user.kick("An admin determined your services were no longer required.").then(() => {
                msg.channel.send(Messages.KickUser(user));
            });
        }
        else {
            //The user is in a higher role then I am.
            msg.reply("I cannot kick that user. He is in a equal or higher role then I am.");
        }

    } else {
        msg.reply("I do not have the KICK_MEMBERS permission.");
    }
}
export function SetNickName(cfg: BotCommonConfig, msg: Message, nickname: string) {
    if (nickname.length <= 2) {
        msg.reply("The provided nickname is not long enough.");
    }

    if (checkPermission(cfg.Guild.me, "MANAGE_NICKNAMES") && checkPermission(cfg.Guild.me, "CHANGE_NICKNAME")) {
        cfg.Guild.me.setNickname(nickname);
        cfg.Nickname = nickname;
    } else {
        msg.reply("I dont have the permissons to change my nickname in this server. I require MANGE_NICKNAMES and CHANGE_NICKNAME");
    }
}
export async function WarPrepStarted_Members_async(cfg: BotCommonConfig) {
    if (!cfg.Notifications.WarPrepStarted) {
        return;
    } else if (!cfg.CH_Members) {
        return await LOG.Log_SimpleMessage_async(cfg.Guild, "No configured channels to send war prep started to.");
    }

    console.info(cfg.Guild.name + ": Sending war prep started.");
    if (cfg.Notifications.WarPrepStartedMessage) {
        const embed = new RichEmbed()
            .setTitle("WAR Prep Started")
            .setDescription(cfg.Notifications.WarPrepStartedMessage);
        await cfg.CH_Members.send(embed);
    } else if (cfg.Role_Members) {
        const embed = new RichEmbed()
            .setTitle("WAR Prep Started")
            .setDescription(`${cfg.Role_Members} WAR placement has started! Please place your troops in the next two hours!`);
        await cfg.CH_Members.send(embed);
    } else {
        const embed = new RichEmbed()
            .setTitle("WAR Prep Started")
            .setDescription(`WAR placement has started! Please place your troops in the next two hours!`);
        await cfg.CH_Members.send(embed);
    }
}
export async function AdminMessage_async(cfg: BotCommonConfig, from: string, msg: string) {
    if (!cfg.CH_Officers) {
        return console.info(cfg.Guild.name + ": No configured member channel to send war prep ending to.");;
    }
    const embed = new RichEmbed()
        .setTitle("WarBot Global Message")
        .setColor("#ad42f4")
        .addField("From", from)
        .setDescription(msg)
        .setFooter("Note- We cannot see responses to this message. To respond, please join the WARBOT test server @ https://discord.gg/ywSDGCf");
    try {
        cfg.CH_Officers.send(embed);
        await LOG.Log_SimpleMessage_async(cfg.Guild, "Sent global message successfully.");
    } catch (err) {
        await LOG.Error_async(cfg, "AdminMessage", err);
    }
}
export async function WarPrepAlmostOver_Members_async(cfg: BotCommonConfig) {
    if (!cfg.Notifications.WarPrepAlmostOver) {
        return;
    } else if (!cfg.CH_Members) {
        return console.info(cfg.Guild.name + ": No configured member channel to send war prep ending to.");;
    }
    console.info(cfg.Guild.name + ": Sending war prep ending.");
    if (cfg.Notifications.WarPrepEndingMessage) {
        const embed = new RichEmbed()
            .setTitle("WAR Prep Ending")
            .setDescription(cfg.Notifications.WarPrepEndingMessage);
        await cfg.CH_Members.send(embed);
    } else if (cfg.Role_Members) {
        const embed = new RichEmbed()
            .setTitle("WAR Prep Ending")
            .setDescription(`${cfg.Role_Members} 15 minutes before war starts! Please place your troops if you have not done so already!!!`);
        await cfg.CH_Members.send(embed);
    } else {
        const embed = new RichEmbed()
            .setTitle("WAR Prep Ending")
            .setDescription(`15 minutes before war starts! Please place your troops if you have not done so already!!!`);
        await cfg.CH_Members.send(embed);
    }
}
export async function WarPrepAlmostOver_Officers_async(cfg: BotCommonConfig) {
    if (!cfg.Notifications.WarPrepAlmostOver) {
        return;
    } else if (!cfg.CH_Officers) {
        return console.info(cfg.Guild.name + ": No configured officer channel to send war prep ending to.");;
    } else if (cfg.CH_Members && cfg.CH_Officers.id == cfg.CH_Members.id) {
        return console.info(cfg.Guild.name + ": Member channel and officer channel are the same. Skipping Officer prep ending message");
    }

    console.info(cfg.Guild.name + ": Sending officers war prep ending.");

    if (cfg.Notifications.WarPrepEndingMessage) {
        //Do Nothing.
    } else if (cfg.Role_Leaders) {
        const embed = new RichEmbed()
            .setTitle("WAR Prep Ending")
            .setDescription(`${cfg.Role_Leaders} The War will begin in 20 minutes. Please make sure everybody is properly placed!`);
        await cfg.CH_Officers.send(embed);
    } else {
        const embed = new RichEmbed()
            .setTitle("WAR Prep Ending")
            .setDescription(`The War will begin in 20 minutes. Please make sure everybody is properly placed!`);
        await cfg.CH_Officers.send(embed);
    }
}
export async function WarStarted_Members_async(cfg: BotCommonConfig) {
    if (!cfg.Notifications.WarStarted) {
        return;
    } else if (!cfg.CH_Members) {
        return console.info(cfg.Guild.name + ": No configured member channel to send war started to.");;
    }

    console.info(cfg.Guild.name + ": Sending war started.");

    if (cfg.Notifications.WarStartedMessage) {
        const embed = new RichEmbed()
            .setTitle("WAR Started")
            .setDescription(cfg.Notifications.WarStartedMessage);
        await cfg.CH_Members.send(embed);
    } else if (cfg.Role_Members) {
        const embed = new RichEmbed()
            .setTitle("WAR Started")
            .setDescription(`${cfg.Role_Members} WAR has started!`);
        await cfg.CH_Members.send(embed);
    } else {
        const embed = new RichEmbed()
            .setTitle("WAR Started")
            .setDescription(`WAR has started!`);
        await cfg.CH_Members.send(embed);
    }
}

export async function WarStarted_Officers_async(cfg: BotCommonConfig) {
    if (!cfg.Notifications.WarStarted) {
        return;
    } else if (!cfg.CH_Officers) {
        return console.info(cfg.Guild.name + ": No configured channels to send war started to.");;
    } else if (cfg.CH_Members && cfg.CH_Officers.id == cfg.CH_Members.id) {
        return console.info(cfg.Guild.name + ": Member channel and officer channel are the same. Skipping Officer war started.");
    }

    console.info(cfg.Guild.name + ": Sending war started.");
    if (cfg.Notifications.WarStartedMessage) {
        //Do Nothing
    } else if (cfg.Role_Officers) {
        const embed = new RichEmbed()
            .setTitle("WAR Started")
            .setDescription(`${cfg.Role_Officers} WAR has started!`);
        await cfg.CH_Officers.send(embed);
    } else {
        const embed = new RichEmbed()
            .setTitle("WAR Started")
            .setDescription(`WAR has started!`);
        await cfg.CH_Officers.send(embed);
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


export function SetRoleLevel(level: RoleLevel, cfg: BotCommonConfig, msg: Message) {
    //Check permissions first.
    if (!checkPermission(cfg.Guild.me, "MANAGE_ROLES")) {
        msg.reply("I do not have the MANAGE_ROLES permission.");
        return;
    }

    let user: GuildMember = msg.mentions.members.last();
    if (!msg.mentions.users.last()) {
        msg.reply("Did... You forget to mention somebody?");
        return;
    }
    else if (msg.mentions.users.last().id == cfg.Bot.user.id) {
        msg.reply("I cannot execute this command on myself.");
        return;
    }

    let reply: string = "";
    if (level == RoleLevel.None) {
        reply += "\n\tSetting " + user + " to no role.";
        if (cfg.Role_Members && user.roles.has(cfg.Role_Members.id)) {
            user.removeRole(cfg.Role_Members);
            reply += "\n\tRemoved role " + cfg.Role_Members;
        } if (cfg.Role_Officers && user.roles.has(cfg.Role_Officers.id)) {
            user.removeRole(cfg.Role_Officers);
            reply += "\n\tRemoved role " + cfg.Role_Officers;
        } if (cfg.Role_Leaders && user.roles.has(cfg.Role_Leaders.id)) {
            user.removeRole(cfg.Role_Leaders);
            reply += "\n\tRemoved role " + cfg.Role_Leaders;
        }
    } else if (level == RoleLevel.Member) {
        reply += "\n\tSetting " + user + " to Member.";
        if (cfg.Role_Members && !user.roles.has(cfg.Role_Members.id)) {
            user.addRole(cfg.Role_Members);
            reply += "\n\tAdded role " + cfg.Role_Members;
        } if (cfg.Role_Officers && user.roles.has(cfg.Role_Officers.id)) {
            user.removeRole(cfg.Role_Officers);
            reply += "\n\tRemoved role " + cfg.Role_Officers;
        } if (cfg.Role_Leaders && user.roles.has(cfg.Role_Leaders.id)) {
            user.removeRole(cfg.Role_Leaders);
            reply += "\n\tRemoved role " + cfg.Role_Leaders;
        }
    } else if (level == RoleLevel.Officer) {
        reply += "\n\tSetting " + user + " to Officer.";
        if (cfg.Role_Members && !user.roles.has(cfg.Role_Members.id)) {
            user.addRole(cfg.Role_Members);
            reply += "\n\tAdded role " + cfg.Role_Members;
        } if (cfg.Role_Officers && !user.roles.has(cfg.Role_Officers.id)) {
            user.addRole(cfg.Role_Officers);
            reply += "\n\tAdded role " + cfg.Role_Officers;
        } if (cfg.Role_Leaders && user.roles.has(cfg.Role_Leaders.id)) {
            user.removeRole(cfg.Role_Leaders);
            reply += "\n\tRemoved role " + cfg.Role_Leaders;
        }
    } else if (level == RoleLevel.Leader) {
        reply += "\n\tSetting " + user + " to Leader.";
        if (cfg.Role_Members && !user.roles.has(cfg.Role_Members.id)) {
            user.addRole(cfg.Role_Members);
            reply += "\n\tAdded role " + cfg.Role_Members;
        } if (cfg.Role_Officers && !user.roles.has(cfg.Role_Officers.id)) {
            user.addRole(cfg.Role_Officers);
            reply += "\n\tAdded role " + cfg.Role_Officers;
        } if (cfg.Role_Leaders && !user.roles.has(cfg.Role_Leaders.id)) {
            user.addRole(cfg.Role_Leaders);
            reply += "\n\tAdded role " + cfg.Role_Leaders;
        }
    }
    msg.reply(reply);
}