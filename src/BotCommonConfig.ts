import { Client, Guild, TextChannel, Role, } from 'discord.js';
import { LocalStorage } from 'node-localstorage';

export class BotCommonConfig {
    public Guild: Guild;
    public Bot: Client;

    private Storage: persistantStorage;
    private configVersion: number = 2;
    private fileStorage: Storage = new LocalStorage('./guilds');

    //#region Channels
    private ch_Members: TextChannel;
    get CH_Members(): TextChannel {
        return this.ch_Members;
    }
    set CH_Members(ch: TextChannel) {
        if (ch)
            this.Storage.Channel_Member = new KeyValuePair(ch.id, ch.name);
        else
            this.Storage.Channel_Member = null;
        this.saveChanges();
        this.ch_Members = ch;
    }

    private ch_Officers: TextChannel;
    get CH_Officers(): TextChannel {
        return this.ch_Officers;
    }
    set CH_Officers(ch: TextChannel) {
        this.ch_Officers = ch;
        if (ch)
            this.Storage.Channel_Officer = new KeyValuePair(ch.id, ch.name);
        else
            this.Storage.Channel_Officer = null;
        this.saveChanges();
    }
    //#endregion


    //#region  Roles
    private role_Members: Role;
    get Role_Members(): Role {
        return this.role_Members;
    }
    set Role_Members(role: Role) {
        this.role_Members = role;
        if (role)
            this.Storage.Role_Member = new KeyValuePair(role.id, role.name);
        else
            this.Storage.Role_Member = null;
        this.saveChanges();
    }

    private role_Officers: Role;
    get Role_Officers(): Role {
        return this.role_Officers;
    }
    set Role_Officers(role: Role) {
        this.role_Officers = role;
        if (role)
            this.Storage.Role_Officer = new KeyValuePair(role.id, role.name);
        else
            this.Storage.Role_Officer = null;
        this.saveChanges();
    }

    private role_Leaders: Role;
    get Role_Leaders(): Role {
        return this.role_Leaders;
    }
    set Role_Leaders(role: Role) {
        this.role_Leaders = role;
        if (role)
            this.Storage.Role_Leader = new KeyValuePair(role.id, role.name);
        else
            this.Storage.Role_Leader = null;
        this.saveChanges();
    }

    private role_Admins: Role;
    get Role_Admins(): Role {
        return this.role_Admins;
    }
    set Role_Admins(role: Role) {
        this.role_Admins = role;
        if (role)
            this.Storage.Role_Admin = new KeyValuePair(role.id, role.name);
        else
            this.Storage.Role_Admin = null;
        this.saveChanges();
    }
    //#endregion

    //#region Other Properties
    get Website_URL(): string {
        return this.Storage.WebsiteURL;
    }
    set Website_URL(website: string) {
        this.Storage.WebsiteURL = website;
        this.saveChanges();
    }

    get Loot_URL(): string {
        return this.Storage.LootURL;
    }
    set Loot_URL(lootURL: string) {
        this.Storage.LootURL = lootURL;
        this.saveChanges();
    }

    get Nickname(): string {
        return this.Storage.NickName;
    }
    set Nickname(nickName: string) {
        this.Storage.NickName = nickName;
        this.saveChanges();
    }

    get BotVersion(): number {
        return this.Storage.BotVersion;
    }
    set BotVersion(version: number) {
        this.Storage.BotVersion = version;
        this.saveChanges();
    }

    get Notifications(): NotificationSettings {
        return this.Storage.Notifications;
    }
    //#endregion

    public constructor(bot: Client, guild: Guild) {
        this.Bot = bot;
        this.Guild = guild;
        this.LoadSettings();
    }

    public LoadSettings() {
        var obj = JSON.parse(this.fileStorage.getItem(this.Guild.id)) as persistantStorage;

        if (obj)
            this.Storage = obj;
        else
            this.SetDefaultSettings();

        if (!this.Storage.Notifications)
            this.Storage.Notifications = new NotificationSettings();

        this.LoadReferences();

        //If no member's channel is set, lets finds one.
        if (!this.CH_Members)
            this.CH_Members = this.FindDefaultMembersChannel();

        //If no officers channel is set, set it to the members channel.
        if (!this.CH_Officers)
            this.CH_Officers = this.CH_Members;

        //Set the guild info.
        if (!this.Storage.Guild)
            this.Storage.Guild = new KeyValuePair(this.Guild.id, this.Guild.name);

        //Update the config version, if it is behind.
        if (this.Storage.ConfigVersion < 2) {
            this.Notifications.War1Enabled = true;
            this.Notifications.War2Enabled = true;
            this.Notifications.War3Enabled = true;
            this.Notifications.War4Enabled = true;
            
            this.Notifications.WarPrepStartedMessage = null;
            this.Notifications.WarPrepEndingMessage = null;
            this.Notifications.WarStartedMessage = null;
            this.Notifications.SendUpdateMessage = true;
            this.saveChanges();
        }
    }
    private LoadReferences() {
        if (this.Storage.Role_Member) this.role_Members = this.Guild.roles.get(this.Storage.Role_Member.ID);
        if (this.Storage.Role_Officer) this.role_Officers = this.Guild.roles.get(this.Storage.Role_Officer.ID);
        if (this.Storage.Role_Leader) this.role_Leaders = this.Guild.roles.get(this.Storage.Role_Leader.ID);
        if (this.Storage.Role_Admin) this.role_Admins = this.Guild.roles.get(this.Storage.Role_Admin.ID);

        if (this.Storage.Channel_Member) this.ch_Members = this.Guild.channels.get(this.Storage.Channel_Member.ID) as TextChannel;
        if (this.Storage.Channel_Officer) this.ch_Officers = this.Guild.channels.get(this.Storage.Channel_Officer.ID) as TextChannel;
    }
    public SetDefaultSettings() {
        this.Storage = new persistantStorage();

        let firstAdminRole: Role = this.Guild.roles.find(function (role) {
            return role.hasPermission("ADMINISTRATOR");
        });

        this.Storage.ConfigVersion = this.configVersion;

        //Find the default admin role.
        if (firstAdminRole) {
            this.Storage.Role_Admin = new KeyValuePair(firstAdminRole.id, firstAdminRole.name);
            this.Storage.Role_Leader = new KeyValuePair(firstAdminRole.id, firstAdminRole.name);
            this.Storage.Role_Officer = new KeyValuePair(firstAdminRole.id, firstAdminRole.name);
        }

        //Default member role.
        if (this.Guild.defaultRole) {
            this.Storage.Role_Member = new KeyValuePair(this.Guild.defaultRole.id, this.Guild.defaultRole.name);
        }

        //Find Default Member Channel
        let MemChannel: TextChannel = this.FindDefaultMembersChannel();
        if (MemChannel) {
            this.Storage.Channel_Member = new KeyValuePair(MemChannel.id, MemChannel.name);
            this.Storage.Channel_Officer = this.Storage.Channel_Member;
        }

        this.Storage.NickName = 'WarBOT';

        //Set default notification settings
        this.Storage.Notifications = new NotificationSettings();

        this.saveChanges();
    }
    public FindDefaultMembersChannel(): TextChannel {
        let ch: TextChannel = null;
        if (this.Guild.defaultChannel) {
            ch = this.Guild.defaultChannel;
        } else {
            //Find the first text channel.
            ch = this.Guild.channels.find(ch1 => { if (ch1 instanceof TextChannel) return true; }) as TextChannel;
        }
        return ch;
    }
    public saveChanges() {
        this.fileStorage.setItem(this.Guild.id, JSON.stringify(this.Storage, null, 2));
        this.LoadReferences();
    }
}

class persistantStorage {
    public Guild: KeyValuePair;

    public Role_Member: KeyValuePair;
    public Role_Officer: KeyValuePair;
    public Role_Leader: KeyValuePair;
    public Role_Admin: KeyValuePair;

    public Channel_Member: KeyValuePair;
    public Channel_Officer: KeyValuePair;

    public NickName: string;

    public LootURL: string;
    public WebsiteURL: string;

    public BotVersion: number;
    public ConfigVersion: number;

    public Notifications: NotificationSettings;

}

export class NotificationSettings {
    constructor() {
        this.WarPrepAlmostOver = true;
        this.WarPrepStarted = true;
        this.WarStarted = true;
    }
    public WarPrepStarted: boolean = true;
    public WarPrepAlmostOver: boolean = true;
    public WarStarted: Boolean = true;

    public SendUpdateMessage: Boolean = true;

    public War1Enabled: boolean = true;
    public War2Enabled: boolean = true;
    public War3Enabled: boolean = true;
    public War4Enabled: boolean = true;

    public WarStartedMessage: string = null;
    public WarPrepStartedMessage: string = null;
    public WarPrepEndingMessage: string = null;
}
class KeyValuePair {
    constructor(id: string, name: string) {
        this.ID = id;
        this.Name = name;
    }

    public ID: string;
    public Name: string;
}