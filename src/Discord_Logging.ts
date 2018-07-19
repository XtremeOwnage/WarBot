import { Client, Channel, TextChannel, Role, Message, User, GuildMember, RichEmbed, Guild, DiscordAPIError } from 'discord.js';
import { RoleLevel } from './RoleLevel';
import { Clean_Mentions } from './Discord_Utils';
import { BotCommonConfig } from './BotCommonConfig';

let ch: TextChannel = null;
let bot: Client = null;

let CH_Test: string = "459028214145220618";
let CH_PROD: string = "459028171199610881";

let Server_DiscordBotList = "264445053596991498";

export async function Initialize_Logging(BOT: Client, isDEV: boolean) {
    this.bot = BOT;
    if (isDEV)
        ch = BOT.channels.get(CH_Test) as TextChannel;
    else
        ch = BOT.channels.get(CH_PROD) as TextChannel;

    if (ch == null) {
        console.error("Unable to find logging channel!!");
    } else {
        console.info("Logging Channel Set To: " + ch.name);
    }
}
export async function Process_Started_async(tag: string) {
    if (!ch) return;
    const embed = new RichEmbed()
        .setTitle("Process Started")
        .setColor("#f4eb42")
        .addField("Username", tag);

    await ch.send(embed);
}

export async function Guild_Created_async(guild: Guild) {
    if (!ch) return;
    const embed = new RichEmbed()
        .setTitle("Guild Created")
        .setColor("#33FF74")
        .addField("Guild", guild.name);
    await ch.send(embed);
}

export async function Guild_Updated_async(guild: Guild, PrevVersion: number, NewVersion: number, SentUpdate: boolean) {
    if (!ch) return;
    const embed = new RichEmbed()
        .setTitle("Guild Updated")
        .setColor("#33FF74")
        .addField("Guild", guild.name)
        .addField("Old Version", PrevVersion, true)
        .addField("New Version", NewVersion, true)
        .addField("Sent Update", SentUpdate, true);
    await ch.send(embed);
}

export async function Guild_Deleted_async(guild: Guild) {
    if (!ch) return;
    const embed = new RichEmbed()
        .setTitle("Guild Deleted")
        .setColor("#FF0000")
        .addField("Guild", guild.name);
    await ch.send(embed);
}
export async function UnHandled_MessageType_async(guild: Guild, msg: Message) {
    if (!ch) return;
    const embed = new RichEmbed()
        .setTitle("Unhandled Message")
        .setColor("#FF0000")
        .addField("Guild", guild.name)
        .addField("Message Type", msg.type)
        .addField("Message", msg.content);
    await ch.send(embed);
}

export async function Log_ReceivedMessage_async(msg: Message) {
    if (msg.channel instanceof TextChannel) {
        await Log_TextChannel_async(msg.channel, msg);
    }
}
export async function Log_Global_Error_async(error: Error) {
    if (!ch) return;
    const embed = new RichEmbed()
        .setTitle("Unhandled Message")
        .setColor("#FF0000")
        .addField("Error Name", error.name)
        .addField("Message", error.message)
        .addField("Trace", error.stack);
    await ch.send(embed);
}

export function IsLogChannel(sourceCH: TextChannel): boolean {
    return sourceCH.id == CH_PROD || sourceCH.id == CH_Test
}

export async function Error_async(cfg: BotCommonConfig | null, Method: string, error: Object) {
    if (!ch) {
        //No logging channel is set. Just return.
        return;
    }
    const embed = new RichEmbed()
        .setTitle("Unhandled Message")
        .setColor("#FF0000")
    if (cfg && cfg.Guild)
        embed.addField("Guild", cfg.Guild.name, true)

    embed.addField("Method", Method, true);

    if (error instanceof DiscordAPIError) {
        embed.addField("Type", "DiscordAPIError")
            .addField("Message", error.message);
    } else {
        embed.addField("Error", error);
    }

    await ch.send(embed);
}

export async function Log_SimpleMessage_async(Guild: Guild, Message: string) {
    if (!ch) {
        //No logging channel is set. Just return.
        return;
    }

    await ch.send(Guild.name + ": " + Message);
}
async function Log_TextChannel_async(sourceCH: TextChannel, msg: Message) {
    if (!ch || IsLogChannel(sourceCH)) {
        //No logging channel is set. Just return.
        return;
    } else if (msg.guild.id == Server_DiscordBotList) {
        //Dont log this laggy thing.
        return;
    }


    const embed = new RichEmbed()
        .setTitle(sourceCH.guild)
        .addField("From", msg.member.displayName, true)
        .addField("Channel", sourceCH.name, true)

    //If, the message is not from me, Log the role level.
    //if (msg.member.id != sourceCH.guild.me.id)
    //    embed.addField("Role Level", role.toString(), true);

    if (msg.embeds.length == 0) {
        embed.addField("Message", Clean_Mentions(msg), false);
    } else {
        embed.addField("Message", "EMBED: " + msg.embeds[0].title, false);
    }

    await ch.send(embed);
}