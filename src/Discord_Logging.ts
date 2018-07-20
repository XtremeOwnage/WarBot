import { Client, Channel, TextChannel, Role, Message, User, GuildMember, RichEmbed, Guild, DiscordAPIError } from 'discord.js';
import { RoleLevel } from './RoleLevel';
import { Clean_Mentions } from './Discord_Utils';
import { BotCommonConfig } from './BotCommonConfig';
import { SSL_OP_ALLOW_UNSAFE_LEGACY_RENEGOTIATION } from 'constants';
import { retry, concat } from 'async';

interface LoggingConfig {
    Chat: TextChannel;
    Errors: TextChannel;
    GuildActivity: TextChannel;
    GeneralOutput: TextChannel;
}

const enum TargetChannel {
    Chat = 1,
    Errors = 2,
    GuildActivity = 3,
    General = 4,
}

export const enum GuildActivityType {
    Deleted = 0,
    Created = 1,
}

let LogChannels: Array<string> = [];
let Channels: LoggingConfig = { Chat: null, Errors: null, GuildActivity: null, GeneralOutput: null };

//Function used to determine if this is a "Logging" channel. We don't want to log, or perform commands for these channels.
export function IsLogChannel(sourceCH: TextChannel): boolean {
    return LogChannels.indexOf(sourceCH.id) > -1;
}

export async function Initialize_Logging(BOT: Client, isDEV: boolean) {
    this.bot = BOT;

    //Channels to which information sent to the bot will be logged.
    //Used for analystics and troubleshooting.
    let CH_ID_DEVL_CHAT: string = "459028214145220618";
    let CH_ID_PROD_CHAT: string = "459028171199610881";

    //Channels to log errors to.
    let CH_ID_DEVL_ERR: string = "469890596752982026";
    let CH_ID_PROD_ERR: string = "469890564800774164";

    //Channels for general logs
    let CH_ID_DEVL_OUT: string = "469900793764380674";
    let CH_ID_PROD_OUT: string = "469900766736285696";

    //Log guild join/leaves to this channel.
    let CH_ID_GUILD_CHANGES: string = "469887501310623746";

    //Populate the log channels array.
    LogChannels.push(CH_ID_DEVL_CHAT);
    LogChannels.push(CH_ID_PROD_CHAT);
    LogChannels.push(CH_ID_DEVL_ERR);
    LogChannels.push(CH_ID_PROD_ERR);
    LogChannels.push(CH_ID_DEVL_OUT);
    LogChannels.push(CH_ID_PROD_OUT);
    LogChannels.push(CH_ID_GUILD_CHANGES);

    if (isDEV) {
        Channels.Chat = BOT.channels.get(CH_ID_DEVL_CHAT) as TextChannel;
        Channels.Errors = BOT.channels.get(CH_ID_DEVL_ERR) as TextChannel;
        Channels.GeneralOutput = BOT.channels.get(CH_ID_DEVL_OUT) as TextChannel;
    }
    else {
        Channels.Chat = BOT.channels.get(CH_ID_PROD_CHAT) as TextChannel;
        Channels.Errors = BOT.channels.get(CH_ID_PROD_ERR) as TextChannel;
        Channels.GeneralOutput = BOT.channels.get(CH_ID_PROD_OUT) as TextChannel;
    }

    //Channel stays the same across prod and devl.
    Channels.GuildActivity = BOT.channels.get(CH_ID_GUILD_CHANGES) as TextChannel;

    if (Channels.Chat == null)
        console.error("Unable to find logging channel!!");
    if (Channels.Errors == null)
        console.error("Unable to find error channel!!");
    if (Channels.GuildActivity == null)
        console.error("Unable to find guild activity logging channel!!");

}

//Log and embed to the specified channel.
async function sendEmbedToChannel(Channel: TargetChannel, Embed: RichEmbed) {
    let ch: TextChannel = null;
    switch (Channel) {
        case TargetChannel.Chat:
            ch = Channels.Chat;
            break;
        case TargetChannel.Errors:
            ch = Channels.Errors;
            break;
        case TargetChannel.GuildActivity:
            ch = Channels.GuildActivity;
            break;
        case TargetChannel.General:
            ch = Channels.GeneralOutput;
            break;
    }

    if (ch)
        await ch.send(Embed);

}

//Log an string to the specified channel.
async function sendMessageToChannel(Channel: TargetChannel, Msg: string) {
    let ch: TextChannel = null;
    switch (Channel) {
        case TargetChannel.Chat:
            ch = Channels.Chat;
            break;
        case TargetChannel.Errors:
            ch = Channels.Errors;
            break;
        case TargetChannel.GuildActivity:
            ch = Channels.GuildActivity;
            break;
        case TargetChannel.General:
            ch = Channels.GeneralOutput;
            break;
    }

    if (ch)
        await ch.send(Msg);
}

//Logs when a new process is started.
export async function Process_Started_async(tag: string) {
    const embed = new RichEmbed()
        .setTitle("Process Started")
        .setColor("#f4eb42")
        .addField("Username", tag);

    await sendEmbedToChannel(TargetChannel.General, embed);
}

//Logs guild activity to the guild activity channel.
export async function Guild_Activity_async(guild: Guild, Type: GuildActivityType) {
    const embed = new RichEmbed();
    switch (Type) {
        case GuildActivityType.Created: {
            embed
                .setTitle("Guild Created")
                .setColor("#33FF74");
        } break;
        case GuildActivityType.Deleted: {
            embed
                .setTitle("Guild Deleted")
                .setColor("#FF0000")
        } break;
    }

    embed
        .addField("Guild", guild.name)
        .addField("Members", guild.memberCount);

    await sendEmbedToChannel(TargetChannel.GuildActivity, embed);
}

//Logs a message to the general channel, when a guild's bot version is updated.
export async function Guild_Updated_async(guild: Guild, PrevVersion: number, NewVersion: number, SentUpdate: boolean) {
    const embed = new RichEmbed()
        .setTitle("Guild Updated")
        .setColor("#33FF74")
        .addField("Guild", guild.name)
        .addField("Old Version", PrevVersion, true)
        .addField("New Version", NewVersion, true)
        .addField("Sent Update", SentUpdate, true);

    await sendEmbedToChannel(TargetChannel.General, embed);
}

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

//Logs commands received by the bot to the chat channel.
export async function Chat_Logging_async(msg: Message) {
    const embed = new RichEmbed()
        .setTitle(msg.guild.name)
        .addField("From", msg.member.displayName, true);

    if (msg.channel instanceof TextChannel)
        embed.addField("Channel", msg.channel.name, true);

    if (msg.embeds.length == 0 && msg) {
        embed.addField("Message", Clean_Mentions(msg), false);
    } else if (msg.embeds.length > 0) {
        embed.addField("Message", "EMBED: " + msg.embeds[0].title, false);
    }

    await sendEmbedToChannel(TargetChannel.Chat, embed);
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