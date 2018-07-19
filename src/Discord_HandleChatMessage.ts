import { Client, Channel, Guild, TextChannel, Role, Message, User, GuildMember, RichEmbed } from 'discord.js';
import { BotCommonConfig } from './BotCommonConfig';
import * as Actions from './DiscordBot_Actions';
import * as Messages from './Discord_FormattedMessages';
import { RoleLevel } from './RoleLevel'
import * as LOG from './Discord_Logging'
import * as async from 'async';
import { Dictionary } from 'typescript-collections';
import { GetRole } from './Discord_Utils';

type CommandHandlerCallBack =
    (Parameters: string, msg: Message, cfg: BotCommonConfig) => Promise<void>;

let CMD: Dictionary<RoleLevel, Dictionary<string, CommandHandlerCallBack>>
    = new Dictionary<RoleLevel, Dictionary<string, CommandHandlerCallBack>>();

SetCommandHandler(RoleLevel.GlobalAdmin, "ping", cmd_AdminPing);
SetCommandHandler(RoleLevel.GlobalAdmin, "mass message", cmd_MassMessage);
SetCommandHandler(RoleLevel.GlobalAdmin, "go die", cmd_GoDie);
SetCommandHandler(RoleLevel.GlobalAdmin, "help", cmd_Help_GlobalAdmin);
SetCommandHandler(RoleLevel.GlobalAdmin, "commands", cmd_Help_GlobalAdmin);
SetCommandHandler(RoleLevel.GlobalAdmin, "show help", cmd_Help_GlobalAdmin);
SetCommandHandler(RoleLevel.GlobalAdmin, "?", cmd_Help_GlobalAdmin);
SetCommandHandler(RoleLevel.GlobalAdmin, "show guilds", cmd_ShowGuilds);
SetCommandHandler(RoleLevel.GlobalAdmin, "list guilds", cmd_ShowGuilds);

SetCommandHandler(RoleLevel.ServerAdmin, "help", cmd_Help_ServerAdmin);
SetCommandHandler(RoleLevel.ServerAdmin, "commands", cmd_Help_ServerAdmin);
SetCommandHandler(RoleLevel.ServerAdmin, "show help", cmd_Help_ServerAdmin);
SetCommandHandler(RoleLevel.ServerAdmin, "?", cmd_Help_ServerAdmin);
SetCommandHandler(RoleLevel.ServerAdmin, "set admin role", cmd_Set_Role_Admin);
SetCommandHandler(RoleLevel.ServerAdmin, "reset config", cmd_ResetConfig);
SetCommandHandler(RoleLevel.ServerAdmin, "go away", cmd_LeaveGuild);
SetCommandHandler(RoleLevel.ServerAdmin, "leave", cmd_LeaveGuild);
SetCommandHandler(RoleLevel.ServerAdmin, "test messages", cmd_TestMessages);
SetCommandHandler(RoleLevel.ServerAdmin, "clear messages", cmd_ClearMessages);

SetCommandHandler(RoleLevel.Leader, "help", cmd_Help_Leader);
SetCommandHandler(RoleLevel.Leader, "commands", cmd_Help_Leader);
SetCommandHandler(RoleLevel.Leader, "show help", cmd_Help_Leader);
SetCommandHandler(RoleLevel.Leader, "?", cmd_Help_Leader);
SetCommandHandler(RoleLevel.Leader, "show config", cmd_ShowConfig);
SetCommandHandler(RoleLevel.Leader, "disable war prep started", cmd_Notification_WarPrepStarted_Disable);
SetCommandHandler(RoleLevel.Leader, "disable notifications", cmd_Notifications_ALL_Disable);
SetCommandHandler(RoleLevel.Leader, "enable notifications", cmd_Notifications_ALL_Enable);
SetCommandHandler(RoleLevel.Leader, "enable war prep started", cmd_Notification_WarPrepStarted_Enable);
SetCommandHandler(RoleLevel.Leader, "disable war prep started", cmd_Notification_WarPrepStarted_Disable);
SetCommandHandler(RoleLevel.Leader, "enable war prep ending", cmd_Notification_WarPrepEnding_Enable);
SetCommandHandler(RoleLevel.Leader, "disable war prep ending", cmd_Notification_WarPrepEnding_Disable);
SetCommandHandler(RoleLevel.Leader, "enable war started", cmd_Notification_WarStarted_Enable);
SetCommandHandler(RoleLevel.Leader, "disable war started", cmd_Notification_WarStarted_Disable);
SetCommandHandler(RoleLevel.Leader, "enable war reminder", cmd_Notification_SpecificWar_Enable);
SetCommandHandler(RoleLevel.Leader, "disable war reminder", cmd_Notification_SpecificWar_Disable);
SetCommandHandler(RoleLevel.Leader, "enable update notification", cmd_Notification_Updates_Enable);
SetCommandHandler(RoleLevel.Leader, "disable update notification", cmd_Notification_Updates_Disable);
SetCommandHandler(RoleLevel.Leader, "kick", cmd_Kick);
SetCommandHandler(RoleLevel.Leader, "set member channel", cmd_Set_Channel_Member);
SetCommandHandler(RoleLevel.Leader, "set officer channel", cmd_Set_Channel_Officer);
SetCommandHandler(RoleLevel.Leader, "set leader channel", cmd_Set_Channel_Officer);
SetCommandHandler(RoleLevel.Leader, "set member role", cmd_Set_Role_Member);
SetCommandHandler(RoleLevel.Leader, "set leader role", cmd_Set_Role_Leader);
SetCommandHandler(RoleLevel.Leader, "set officer role", cmd_Set_Role_Officer);
SetCommandHandler(RoleLevel.Leader, "set war prep started", cmd_Set_Message_WarPrepStarted);
SetCommandHandler(RoleLevel.Leader, "set war prep ending", cmd_set_Message_WarPrepEnding);
SetCommandHandler(RoleLevel.Leader, "set war started", cmd_set_Message_WarStarted);
SetCommandHandler(RoleLevel.Leader, "set member", cmd_SetRole_Member);
SetCommandHandler(RoleLevel.Leader, "set officer", cmd_SetRole_Officer);
SetCommandHandler(RoleLevel.Leader, "set leader", cmd_SetRole_Leader);
SetCommandHandler(RoleLevel.Leader, "set name", cmd_Change_My_NickName);
SetCommandHandler(RoleLevel.Leader, "set nickname", cmd_Change_My_NickName);
SetCommandHandler(RoleLevel.Leader, "set website url", cmd_Set_Website_URL);
SetCommandHandler(RoleLevel.Leader, "set loot url", cmd_Set_LootPage_URL);
SetCommandHandler(RoleLevel.Leader, "set website", cmd_Set_Website_URL);
SetCommandHandler(RoleLevel.Leader, "set loot", cmd_Set_LootPage_URL);

SetCommandHandler(RoleLevel.Officer, "help", cmd_Help_Officer);
SetCommandHandler(RoleLevel.Officer, "commands", cmd_Help_Officer);
SetCommandHandler(RoleLevel.Officer, "show help", cmd_Help_Officer);
SetCommandHandler(RoleLevel.Officer, "?", cmd_Help_Officer);

SetCommandHandler(RoleLevel.Member, "help", cmd_Help_Member);
SetCommandHandler(RoleLevel.Member, "commands", cmd_Help_Member);
SetCommandHandler(RoleLevel.Member, "show help", cmd_Help_Member);
SetCommandHandler(RoleLevel.Member, "?", cmd_Help_Member);
SetCommandHandler(RoleLevel.Member, "who is awesome", cmd_WhoIsAwesome_Hidden);
SetCommandHandler(RoleLevel.Member, "loot", cmd_Show_Loot_URL);

SetCommandHandler(RoleLevel.None, "help", cmd_Help_Everyone);
SetCommandHandler(RoleLevel.None, "commands", cmd_Help_Everyone);
SetCommandHandler(RoleLevel.None, "show help", cmd_Help_Everyone);
SetCommandHandler(RoleLevel.None, "?", cmd_Help_Everyone);
SetCommandHandler(RoleLevel.None, "ping", cmd_PING);
SetCommandHandler(RoleLevel.None, "website", cmd_Show_Website_URL);
SetCommandHandler(RoleLevel.None, "thanks", cmd_Say_Thanks);
SetCommandHandler(RoleLevel.None, "stats", cmd_Show_My_Stats);
SetCommandHandler(RoleLevel.None, "show stats", cmd_Show_My_Stats);
SetCommandHandler(RoleLevel.None, "uptime", cmd_Show_My_Stats);


function SetCommandHandler(Role: RoleLevel, Command: string, Callback: CommandHandlerCallBack) {
    if (!CMD.containsKey(Role))
        CMD.setValue(Role, new Dictionary<string, CommandHandlerCallBack>());

    var RoleCommands = CMD.getValue(Role);

    RoleCommands.setValue(Command, Callback);

}
export async function HandleCommand_async(cmd: string, msg: Message, cfg: BotCommonConfig): Promise<void> {
    if (!cmd || !msg)
        return;
    let role: RoleLevel = GetRole(cfg, msg);
    for (var lvl of CMD.keys()) {
        if (role >= lvl) {
            let CommandsToFunctions: Dictionary<string, CommandHandlerCallBack> = CMD.getValue(lvl);
            for (var cmdText of CommandsToFunctions.keys()) {
                if (cmd.toLowerCase().startsWith(cmdText)) {
                    var Msg = cmd.substr(cmdText.length, cmd.length - cmdText.length).trim();
                    return await CommandsToFunctions.getValue(cmdText)(Msg, msg, cfg);
                }
            }
        }
    }
};


//All commands go below.


async function cmd_Kick(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await Actions.KickMember(cfg, msg);
}
async function cmd_Set_Channel_Member(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (msg.mentions.channels.first() != null) {
        cfg.CH_Members = msg.mentions.channels.first();
        await msg.reply("Members channel has been set to: " + cfg.CH_Members);
    } else {
        await msg.reply("In order to set the members channel, you must mention a channel name.");
    }
}
async function cmd_Set_Channel_Officer(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (msg.mentions.channels.first() != null) {
        cfg.CH_Officers = msg.mentions.channels.first();
        await msg.reply("Officers channel has been set to: " + cfg.CH_Officers);
    } else {
        await msg.reply("In order to set the officers channel, you must mention a channel name.");
    }
}
async function cmd_Set_Role_Admin(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (msg.mentions.roles.first() != null) {
        cfg.Role_Admins = msg.mentions.roles.first();
        await msg.reply("Admin role has been set to: " + cfg.Role_Admins);
    } else {
        await msg.reply("You must mention the role, to configure as the admin role.");
    }
}
async function cmd_Set_Role_Leader(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (msg.mentions.roles.first() != null) {
        cfg.Role_Leaders = msg.mentions.roles.first();
        await msg.reply("Leader role has been set to: " + cfg.Role_Leaders);
    } else {
        await msg.reply("In order to set the Leader role, you must mention the role.");
    }
}
async function cmd_Set_Role_Officer(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (msg.mentions.roles.first() != null) {
        cfg.Role_Officers = msg.mentions.roles.first();
        await msg.reply("Officer role has been set to: " + cfg.Role_Officers);
    } else {
        await msg.reply("In order to set the Officer's role, you must mention the role.");
    }
}
async function cmd_Set_Role_Member(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (msg.mentions.roles.first() != null) {
        cfg.Role_Members = msg.mentions.roles.first();
        await msg.reply("Member role has been set to: " + cfg.Role_Members);
    } else {
        await msg.reply("In order to set the member's role, you must mention the role.");
    }
}
async function cmd_Set_Message_WarPrepStarted(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (Parameters.length > 1) {
        cfg.Notifications.WarPrepStartedMessage = Parameters;
        await msg.reply("The 'War Prep Started' notification has been updated.");
    } else {
        cfg.Notifications.WarPrepStartedMessage = null;
        await msg.reply("The 'War Prep Started' notification has been set to default, because no content was provided.");
    }
    cfg.saveChanges();
}
async function cmd_set_Message_WarPrepEnding(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (Parameters.length > 1) {
        cfg.Notifications.WarPrepEndingMessage = Parameters;
        await msg.reply("The 'War Prep Ending' notification has been updated.");
    } else {
        cfg.Notifications.WarPrepEndingMessage = null;
        await msg.reply("The 'War Prep Ending' notification has been set to default, because no content was provided.");
    }
    cfg.saveChanges();

}
async function cmd_set_Message_WarStarted(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (Parameters.length > 1) {
        cfg.Notifications.WarStartedMessage = Parameters;
        await msg.reply("The 'War Started' notification has been updated.");
    } else {
        cfg.Notifications.WarStartedMessage = null;
        await msg.reply("The 'War Started' notification has been set to default, because no content was provided.");
    }
    cfg.saveChanges();

}
async function cmd_Notification_SpecificWar_Enable(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    switch (Parameters) {
        case "1":
            cfg.Notifications.War1Enabled = true;
            cfg.saveChanges();
            await msg.reply("Done.");
            break;
        case "2":
            cfg.Notifications.War2Enabled = true;
            cfg.saveChanges();
            await msg.reply("Done.");
            break;
        case "3":
            cfg.Notifications.War3Enabled = true;
            cfg.saveChanges();
            await msg.reply("Done.");
            break;
        case "4":
            cfg.Notifications.War4Enabled = true;
            cfg.saveChanges();
            await msg.reply("Done.");
            break;
        default:
            await msg.reply("Please specify 1,2,3 or 4");
    }
}
async function cmd_Notification_SpecificWar_Disable(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    switch (Parameters) {
        case "1":
            cfg.Notifications.War1Enabled = false;
            cfg.saveChanges();
            await msg.reply("Done.");
            break;
        case "2":
            cfg.Notifications.War2Enabled = false;
            cfg.saveChanges();
            await msg.reply("Done.");
            break;
        case "3":
            cfg.Notifications.War3Enabled = false;
            cfg.saveChanges();
            await msg.reply("Done.");
            break;
        case "4":
            cfg.Notifications.War4Enabled = false;
            cfg.saveChanges();
            await msg.reply("Done.");
            break;
        default:
            await msg.reply("Please specify 1,2,3 or 4");
    }
}
async function cmd_SetRole_Member(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await Actions.SetRoleLevel(RoleLevel.Member, cfg, msg);
}
async function cmd_SetRole_Officer(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await Actions.SetRoleLevel(RoleLevel.Officer, cfg, msg);
}
async function cmd_SetRole_Leader(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await Actions.SetRoleLevel(RoleLevel.Leader, cfg, msg);
}
async function cmd_Change_My_NickName(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await Actions.SetNickName(cfg, msg, Parameters);
}
async function cmd_Set_Website_URL(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (Parameters.length > 5) {
        cfg.Website_URL = Parameters;
        await msg.reply("The new website URL has been set.");
    } else {
        cfg.Website_URL = null;
        await msg.reply("A valid URL was not provided. The value has been set to 'Blank'.");
    }
}
async function cmd_Set_LootPage_URL(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (Parameters.length > 5) {
        cfg.Loot_URL = Parameters;
        await msg.reply("The new loot URL has been set.");
    } else {
        cfg.Loot_URL = null;
        await msg.reply("A valid URL was not provided. The value has been set to 'Blank'.");
    }
}
async function cmd_WhoIsAwesome_Hidden(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.reply('<@381654208073433091> of course. He is the greatest. He created me.');
}
async function cmd_Show_Loot_URL(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (cfg.Loot_URL) {
        await msg.reply(cfg.Loot_URL);
    } else {
        await msg.reply("Sorry, there is no message set for this command.");
    }
}
async function cmd_Show_Website_URL(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    if (cfg.Website_URL) {
        await msg.reply(cfg.Website_URL);
    } else {
        await msg.reply("Sorry, your leader has not set a valid message for this command.");
    }
}
async function cmd_PING(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.reply('Pong!');
}
async function cmd_Say_Thanks(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.reply('No problem. I am always glad to help.');
}
async function cmd_Show_My_Stats(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.channel.send(Messages.Statistics(cfg));
}

async function cmd_ResetConfig(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.SetDefaultSettings();
    await msg.channel.send(Messages.ShowConfig(cfg)).then(() =>
        msg.reply("All settings have been reverted to default.")
    );
}
async function cmd_LeaveGuild(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.SetDefaultSettings();
    await msg.channel.send(Messages.Bot_Leaving())
        .then(() => cfg.Guild.leave());
}

async function cmd_TestMessages(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await Actions.WarPrepStarted_Members_async(cfg);
    await Actions.WarPrepAlmostOver_Members_async(cfg);
    await Actions.WarPrepAlmostOver_Officers_async(cfg);
    await Actions.WarStarted_Members_async(cfg);
    await Actions.WarStarted_Officers_async(cfg);
}
async function cmd_ClearMessages(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await Actions.ClearMessages_async(msg.channel);
}
async function cmd_Notifications_ALL_Disable(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.Notifications.WarPrepStarted = false;
    cfg.Notifications.WarPrepAlmostOver = false;
    cfg.Notifications.WarStarted = false;
    cfg.Notifications.SendUpdateMessage = false;
    cfg.saveChanges();
    msg.reply("Done.");
}
async function cmd_Notifications_ALL_Enable(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.Notifications.SendUpdateMessage = false;
    cfg.Notifications.WarPrepStarted = true;
    cfg.Notifications.WarPrepAlmostOver = true;
    cfg.Notifications.WarStarted = true;
    cfg.saveChanges();
    msg.reply("Done.");
}
async function cmd_Notification_Updates_Enable(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.Notifications.SendUpdateMessage = true;
    cfg.saveChanges();
    msg.reply("Done.");
}
async function cmd_Notification_Updates_Disable(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.Notifications.SendUpdateMessage = false;
    cfg.saveChanges();
    msg.reply("Done.");
}
async function cmd_Notification_WarPrepStarted_Enable(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.Notifications.WarPrepStarted = true;
    cfg.saveChanges();
    msg.reply("Done.");
}
async function cmd_Notification_WarPrepStarted_Disable(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.Notifications.WarPrepStarted = false;
    cfg.saveChanges();
    await msg.reply("Done.");
}
async function cmd_Notification_WarPrepEnding_Enable(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.Notifications.WarPrepAlmostOver = true;
    cfg.saveChanges();
    msg.reply("Done.");
}
async function cmd_Notification_WarPrepEnding_Disable(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.Notifications.WarPrepAlmostOver = false;
    cfg.saveChanges();
    msg.reply("Done.");
}
async function cmd_Notification_WarStarted_Enable(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.Notifications.WarStarted = true;
    cfg.saveChanges();
    msg.reply("Done.");
}
async function cmd_Notification_WarStarted_Disable(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.Notifications.WarStarted = false;
    cfg.saveChanges();
    msg.reply("Done.");
}
async function cmd_ShowConfig(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    return Messages.ShowConfig(cfg).forEach(async function DisplayMessage(embed: RichEmbed) {
        await msg.channel.send(embed);
    });
}
async function cmd_AdminPing(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.reply('ADMIN Pong!');
}
async function cmd_MassMessage(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await Actions.MassNotify(cfg, Parameters);
}
async function cmd_GoDie(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.reply("I am sorry I did not live up to your expectations. Goodbye world.").then(() => process.exit());
}
async function cmd_Help_GlobalAdmin(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.channel.send(Messages.Commands_GlobalAdmin());
    await cmd_Help_ServerAdmin(Parameters, msg, cfg);
}
async function cmd_Help_ServerAdmin(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.channel.send(Messages.Commands_ServerAdmin());
    await cmd_Help_Leader(Parameters, msg, cfg);
}
async function cmd_Help_Leader(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.channel.send(Messages.Commands_Leader());
    await cmd_Help_Officer(Parameters, msg, cfg);
}
async function cmd_Help_Officer(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await cmd_Help_Member(Parameters, msg, cfg);
}
async function cmd_Help_Member(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.channel.send(Messages.Commands_Member());
    await cmd_Help_Everyone(Parameters, msg, cfg);
}
async function cmd_Help_Everyone(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.channel.send(Messages.Commands_Everybody());
}
async function cmd_ShowGuilds(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    let embeds: RichEmbed[] = Messages.ShowGuilds(cfg.Bot);
    await async.each(embeds, async function (embed: RichEmbed) {
        await msg.channel.send(embed);
    });
}
// async function cmd_DisableNotifications(Parameters: string, msg: Message, cfg: BotCommonConfig) 
