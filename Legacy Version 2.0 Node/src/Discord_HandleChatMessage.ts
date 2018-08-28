SetCommandHandler(RoleLevel.GlobalAdmin, "ping", cmd_AdminPing);
//SetCommandHandler(RoleLevel.GlobalAdmin, "mass message", cmd_MassMessage);
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

async function cmd_WhoIsAwesome_Hidden(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.reply('<@381654208073433091> of course. He is the greatest. He created me.');
}
async function cmd_PING(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.reply('Pong!');
}
async function cmd_Say_Thanks(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await msg.reply('No problem. I am always glad to help.');
}

async function cmd_ResetConfig(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    cfg.SetDefaultSettings();
    await msg.channel.send(Messages.ShowConfig(cfg)).then(() =>
        msg.reply("All settings have been reverted to default.")
    );
}

async function cmd_TestMessages(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    await Actions.WarPrepStarted_Members_async(cfg);
    await Actions.WarPrepAlmostOver_Members_async(cfg);
    await Actions.WarPrepAlmostOver_Officers_async(cfg);
    await Actions.WarStarted_Members_async(cfg);
    await Actions.WarStarted_Officers_async(cfg);
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


async function cmd_ShowGuilds(Parameters: string, msg: Message, cfg: BotCommonConfig) {
    let embeds: RichEmbed[] = Messages.ShowGuilds(cfg.Bot);
    await async.each(embeds, async function (embed: RichEmbed) {
        await msg.channel.send(embed);
    });
}
// async function cmd_DisableNotifications(Parameters: string, msg: Message, cfg: BotCommonConfig) 
