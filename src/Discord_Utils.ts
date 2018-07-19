import { Client, Channel, Guild, TextChannel, Role, Message, User, GuildMember, RichEmbed, PermissionString } from 'discord.js';
import { BotCommonConfig } from './BotCommonConfig';
import { RoleLevel } from "./RoleLevel";

//Cleans all mentions from the message, and returns a string.
export function Clean_Mentions(msg: Message): string {
    let Message: string = msg.content;

    msg.mentions.channels.forEach((mention: TextChannel) => {
        Message = Message.replace(mention.toString(), "#" + mention.name);
    });
    msg.mentions.members.forEach((mention: GuildMember) => {
        Message = Message.replace(mention.toString(), "@" + mention.displayName);
    });
    msg.mentions.roles.forEach((mention: Role) => {
        Message = Message.replace(mention.toString(), "@" + mention.name);
    });
    msg.mentions.users.forEach((mention: User) => {
        Message = Message.replace(mention.toString(), "@" + mention.username);
    });

    return Message;
}

export function GetRole(cfg: BotCommonConfig, msg: Message): RoleLevel {
    //Is Eric. Used for troubleshooting purposes, or showing bot output, which should not be available to the public..
    if (msg.author.id == '381654208073433091') {
        return RoleLevel.GlobalAdmin;
    } else if (checkPermission(msg.member, "ADMINISTRATOR")) {
        return RoleLevel.ServerAdmin;
    } else if (cfg.Role_Leaders && msg.member.roles.has(cfg.Role_Leaders.id)) {
        return RoleLevel.Leader;
    } else if (cfg.Role_Officers && msg.member.roles.has(cfg.Role_Officers.id)) {
        return RoleLevel.Officer;
    } else if (cfg.Role_Members && msg.member.roles.has(cfg.Role_Members.id)) {
        return RoleLevel.Member;
    } else {
        return RoleLevel.None;
    }
}

export function checkPermission(user: GuildMember, permission: PermissionString): boolean {
    return user.hasPermission(permission);
}