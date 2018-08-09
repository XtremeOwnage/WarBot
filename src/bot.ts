import { Client, Channel, Guild, TextChannel, DMChannel, User, DiscordAPIError } from "discord.js";
import { BotCommonConfig } from './BotCommonConfig';
import * as Discord_Actions from './DiscordBot_Actions';
import { HandleCommand_async } from './Discord_HandleChatMessage';
import { RecurrenceRule, scheduleJob } from 'node-schedule';
import * as LOG from './Discord_Logging';
import * as async from 'async';
import { error } from "util";
import { fail } from "assert";
import { retry } from "async";

let DiscordSetup = {
    Token: "",
    UserName: "WarBot"
};

//Holds an array of Config objects, to guilds.
let ConfigDictionary: Map<string, BotCommonConfig> = new Map<string, BotCommonConfig>();

//Reference to the bot class.
let bot: Client = new Client;

//Determines if this bot is running in debug mode. This will seperate the logs and actions from regular production activities.
let debug: boolean = false;

async function UncaughtExceptionHandler(err: Error) {
    await LOG.Error_async(null, "UncaughtExceptionHandler", null, err);
}

function DiscordStartUp() {
    bot.login(DiscordSetup.Token);
    bot.on('guildCreate', async (guild: Guild) => {
        if (shouldReturn_TestCheck(guild)) { return; }
        let cfg: BotCommonConfig = await new BotCommonConfig(bot, guild);
        await Discord_Actions.Bot_JoinedServer(cfg);
        setConfig(guild, cfg);
    });
    bot.on('guildDelete', async (guild: Guild) => {
        if (shouldReturn_TestCheck(guild)) { return; }
        await Discord_Actions.Bot_LeftServer(getConfig(guild));
        ConfigDictionary.delete(guild.id);
    });
    bot.on('roleDelete', async (role) => {
        if (shouldReturn_TestCheck(role.guild)) { return; }
        await Discord_Actions.HandleDeletedRole_async(role, getConfig(role.guild));
    });
    bot.on('channelDelete', async (channel) => {
        if (channel instanceof TextChannel) {
            if (shouldReturn_TestCheck(channel.guild)) { return; }
            await Discord_Actions.HandleDeletedChannel_async(channel, getConfig(channel.guild));
        }
    });
    bot.on('error', async function (error: Error) {
        await LOG.Error_async(null, "bot.on.Error", null, error);
    });
    bot.on('ready', async () => {
        await LOG.Initialize_Logging(bot, debug);

        //Set the exception handler, AFTER the logging library is initialized.
        process.on('uncaughtException', UncaughtExceptionHandler);

        //Send a log message. Useful to know if the process is crashing over and over.
        await LOG.Process_Started_async(bot.user.tag);

        //Set Presence to "Online".
        await Discord_Actions.ComeOnline(bot);

        //Initalize the config repository for each guild.
        async.each(bot.guilds.values(), async function (guild: Guild) {
            if (shouldReturn_TestCheck(guild)) { return; }
            let cfg: BotCommonConfig = new BotCommonConfig(bot, guild);
            await Discord_Actions.ConnectedToServer_async(cfg);
            setConfig(guild, cfg);
        });

        //Schedule the notification events.
        ScheduleEvents();
    });
    bot.on('message', async function (msg) {
        let cfg: BotCommonConfig = null;
        try {
            //Ignore anything from a private channel, or anything without a guild.
            if (!msg.guild || !msg.guild.id || msg.channel instanceof DMChannel) return;

            //Prevents test from running in prod, and vise-versa.
            else if (shouldReturn_TestCheck(msg.guild)) return;

            //Ignore other bots... per bot-etiquette
            else if (msg.author.bot == true) return;

            //Fetch this guild's config object.
            cfg = getConfig(msg.guild);

            //Throw exception if there is no defined config... 
            if (!cfg) throw new Error("CFG is not defined.");

            if (msg.channel instanceof TextChannel) {
                //If the message came from one of the logging channels, abort.
                if (LOG.IsLogChannel(msg.channel)) return;

                let ToMe: Boolean = false;
                let Message: string = msg.content.trim();

                if (msg.isMemberMentioned(cfg.Bot.user) && Message.startsWith(cfg.Guild.me.toString())) {
                    let Me: number = cfg.Guild.me.toString().length;
                    Message = Message.substr(Me, Message.length - Me).trim();
                    if (Message.startsWith(',')) {
                        Message = Message.substr(1, Message.length - 1);
                    }
                    ToMe = true;
                } else if (msg.content.toLowerCase().startsWith("bot,")) {
                    Message = Message.substr(4, Message.length - 4);
                    ToMe = true;
                } else {
                    return;
                }

                if (ToMe) {
                    //Log the message for diagnostics.
                    await LOG.Chat_Logging_async(msg);

                    //Execute the command handler.
                    await HandleCommand_async(Message.trim(), msg, cfg);
                }

            } else {
                await LOG.DebugOutput_async(msg.guild, "Encountered unhandled message type.");
            }
        } catch (err) {
            await LOG.Error_async(cfg, "bot.on.Message", null, err);
        }
    });
}


function setConfig(guild: Guild, cfg: BotCommonConfig) {
    ConfigDictionary.set(guild.id, cfg);
}
function getConfig(guild: Guild): BotCommonConfig {
    let a: BotCommonConfig = ConfigDictionary.get(guild.id);

    if (!a) {
        //Config was not found in the local cache. Re-add config to local cache.
        let cfg: BotCommonConfig = new BotCommonConfig(bot, guild);
        setConfig(guild, cfg);

        let b: BotCommonConfig = ConfigDictionary.get(guild.id);
        if (!b) {
            LOG.DebugOutput_async(guild, "Unable to locate config.");
            return null;
        }
        return b;
    } else {
        return a;
    }
}
function shouldReturn_TestCheck(guild: Guild): Boolean {
    return (debug && !isTestGuild(guild)) || (!debug && isTestGuild(guild));
}
function isTestGuild(guild: Guild): Boolean {
    //Statically defined... but, hey. it works. Might change one day.
    return guild.id == "458992709718245377" || guild.id == "461975072563789824";
}
function ScheduleEvents() {
    var rule = new RecurrenceRule();
    rule.minute = 0;
    rule.hour = [2, 14, 8, 20];

    var rule2 = new RecurrenceRule();
    rule2.minute = 45;
    rule2.hour = [3, 15, 9, 21];

    var rule3 = new RecurrenceRule();
    rule3.minute = 0;
    rule3.hour = [4, 16, 10, 22];

    var rule4 = new RecurrenceRule();
    rule4.minute = 15;
    rule4.hour = [4, 16, 10, 22];

    scheduleJob(rule, async () => await SendWarPrepStartedToAll_async());
    scheduleJob(rule2, async () => await SendWarPrepEndingToAll_async());
    scheduleJob(rule3, async () => await SendWarStartedToAll_async());
    scheduleJob(rule4, async () => await Discord_Actions.GoAway(bot));
}

async function SendMessageToAll_async(msg: string, from: User) {
    async.each(ConfigDictionary.values(), async (cfg: BotCommonConfig) => {
        try {
            await Discord_Actions.AdminMessage_async(cfg, from.toString(), msg);
        } catch (err) {
            LOG.Error_async(cfg, "SendMessageToAll_async", null, err);
        }
    });
}

async function SendWarPrepStartedToAll_async() {
    await Discord_Actions.ComeOnline(bot);

    async.each(ConfigDictionary.values(), async (cfg: BotCommonConfig) => {
        try {
            if (SendMessagesForThisWar(cfg)) {
                await Discord_Actions.WarPrepStarted_Members_async(cfg);
            }
        } catch (err) {
            LOG.Error_async(cfg, "SendWarPrepStartedToAll_async", null, err);
        }
    });
}
async function SendWarPrepEndingToAll_async() {
    async.each(ConfigDictionary.values(), async (cfg: BotCommonConfig) => {
        try {
            if (SendMessagesForThisWar(cfg)) {
                await Discord_Actions.WarPrepAlmostOver_Members_async(cfg);
                await Discord_Actions.WarPrepAlmostOver_Officers_async(cfg);
            };
        } catch (err) {
            LOG.Error_async(cfg, "SendWarPrepEndingToAll_async", null, err);
        }
    });
}
async function SendWarStartedToAll_async() {

    async.each(ConfigDictionary.values(), async (cfg: BotCommonConfig) => {
        try {
            if (SendMessagesForThisWar(cfg)) {
                await Discord_Actions.WarStarted_Members_async(cfg);
                await Discord_Actions.WarStarted_Officers_async(cfg);
            }
        } catch (err) {
            LOG.Error_async(cfg, "SendWarPrepEndingToAll_async", null, err);
        }
    });
}

function SendMessagesForThisWar(cfg: BotCommonConfig) {
    var date = new Date();
    var current_hour = date.getHours();

    return (cfg.Notifications.War1Enabled && current_hour >= 2 && current_hour < 8)
        || (cfg.Notifications.War2Enabled && current_hour >= 8 && current_hour < 14)
        || (cfg.Notifications.War3Enabled && current_hour >= 14 && current_hour < 20)
        || (cfg.Notifications.War4Enabled && current_hour >= 20 && current_hour < 24);

}


process.argv.forEach(function (val, index, array) {
    console.info("Args: " + index + ': ' + val);
    if (val.startsWith('--')) {
        switch (val) {
            case "--debug":
                LOG.DebugOutput_async(null, "Debug mode specified via args.");
                debug = true;
                break;

            case "--token":
                let tok: string = array[index + 1];
                if (tok && tok != "") {
                    DiscordSetup.Token = tok;
                } else {
                    LOG.DebugOutput_async(null, "Token switch was specified, but, unable to find token.");
                }
                break;

        }
    }
});

if (process.env.debug && process.env.debug.toLocaleLowerCase() == "true") {
    LOG.DebugOutput_async(null, "Debug mode specified via environment variable.");
    debug = true;
}
//Set the token from ENV variable.
if (process.env.Token && process.env.Token.length > 5) {
    DiscordSetup.Token = process.env.Token;
}

if (DiscordSetup.Token && DiscordSetup.Token != "") {
    DiscordStartUp();
} else {
    throw new Error("no token provided. Process aborting.");
}


