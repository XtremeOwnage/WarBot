using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using WarBot.Core.Dialogs;
using WarBot.Core.Helper;
using WarBot.Core.ModuleType;
using System.Linq;
using WarBot.Core;
using System.Collections.Generic;

namespace WarBot.Modules.Dialogs
{
    public class SetupDialog : SocketGuildDialogContextBase
    {
        public SetupDialog(GuildCommandContext Context)
            : base(Context) { }


        private SetupStep CurrentStep = SetupStep.Initial;
        private Stack<SetupStep> History = new Stack<SetupStep>(15);

        public async override Task ProcessMessage(SocketUserMessage input)
        {
            //Add the user's message to be cleaned up after this poll is completed.
            CleanupList.Add(input);
            string Message = input.Content.Trim();
            string rawMsg = input.Content.Trim().ToLowerInvariant();

            if (rawMsg == "stop" || rawMsg == "cancel")
            {
                await SendAsync("Setup aborted.");
                await this.Bot.CloseDialog(this);
                return;
            }
            else if (rawMsg == "back" || rawMsg == "go back")
            {
                //If the user said back, return to the previous step.
                await PreviousStep();
                return;
            }

            //Parse out the differant options we will need.
            bool Skip = rawMsg == "skip";
            bool? Bool = rawMsg.ParseBool();
            SocketTextChannel CH = input.MentionedChannels.OfType<SocketTextChannel>().FirstOrDefault();
            SocketRole ROLE = input.MentionedRoles.FirstOrDefault();


            //ToDo - Submitted a issue for dehumanizer, to see if this "logic" can be offloaded to their library.
            const string msg_BoolParseFailed = "I did not reconize that message. You may say yes, no, true, false, y or n.\r\n";
            string msg_ChannelExpected = $"I expected a channel. Please tag the channel like this: {this.Channel.Mention}\r\n";
            string msg_RoleExpected = $"I expected a role. Please tag the role like this:\r\n {this.Guild.CurrentUser.Roles.FirstOrDefault(o => o.IsMentionable)?.Mention ?? "@MyRole"}";

            switch (CurrentStep)
            {
                case SetupStep.Initial:
                    await NextStep();
                    break;
                case SetupStep.WarBot_Prefix:
                    if (Skip)
                        Config.Prefix = "bot,";
                    else
                        Config.Prefix = Message;

                    await NextStep($"My prefix has been set to '{Config.Prefix}'");
                    break;
                case SetupStep.User_Left_Channel:
                    if (Skip)
                    {
                        Config.SetGuildChannel(Core.WarBotChannelType.USER_LEFT, null);
                        await SkipStep("I will not send notifications when a user leaves.");
                    }
                    else if (CH != null)
                    {
                        Config.SetGuildChannel(Core.WarBotChannelType.USER_LEFT, CH);
                        await NextStep($"User leave notifications will go to {CH.Mention}");
                    }
                    else
                        await SendAsync(msg_ChannelExpected);
                    break;
                case SetupStep.User_Left_Message:
                    if (Skip)
                    {
                        Config[Setting_Key.USER_LEFT].Set(true, null);
                        await SkipStep("I will send a default message.");
                    }
                    else
                    {
                        Config[Setting_Key.USER_LEFT].Set(true, Message);
                        await NextStep("When users leave the server, this message will be sent:\r" +
                            $"\n{Message}\r" +
                            $"\nIn channel {Config.GetGuildChannel(WarBotChannelType.USER_LEFT).Mention}");
                    }
                    break;
                case SetupStep.User_Join_Channel:
                    if (Skip)
                    {
                        Config.SetGuildChannel(Core.WarBotChannelType.USER_JOIN, null);
                        Config[Setting_Key.USER_JOIN].Enabled = false;
                        await SkipStep("I will not send new user greetings.");
                    }
                    else if (CH != null)
                    {
                        Config.SetGuildChannel(Core.WarBotChannelType.USER_JOIN, CH);
                        await NextStep($"New user greetings will go to {CH.Mention}");
                    }
                    else
                        await SendAsync(msg_ChannelExpected);
                    break;
                case SetupStep.User_Join_Message:
                    if (Skip)
                    {
                        Config[Setting_Key.USER_JOIN].Set(true, null);
                        await SkipStep("I will send a default message.");
                    }
                    else
                    {
                        Config[Setting_Key.USER_JOIN].Set(true, Message);
                        await NextStep("New users joining the server will receive this message:\r" +
                            $"\n{User.Mention}, {Message}\r" +
                            $"\nIn channel {Config.GetGuildChannel(WarBotChannelType.USER_JOIN).Mention}");
                    }
                    break;
                case SetupStep.Channel_Updates:
                    if (Skip)
                    {
                        Config.SetGuildChannel(Core.WarBotChannelType.WARBOT_UPDATES, null);
                        Config[Setting_Key.WARBOT_UPDATES].Enabled = false;
                        await SkipStep("I will not send notifications when I am updated.");
                    }
                    else if (CH != null)
                    {
                        Config.SetGuildChannel(Core.WarBotChannelType.WARBOT_UPDATES, CH);
                        Config[Setting_Key.WARBOT_UPDATES].Enabled = true;
                        await NextStep($"My update notifications will be sent to {CH.Mention}.");
                    }
                    else
                        await SendAsync(msg_ChannelExpected);

                    break;
                case SetupStep.Channel_Officers:
                    if (Skip)
                    {
                        Config.SetGuildChannel(Core.WarBotChannelType.CH_Officers, null);
                        await SkipStep("I will not send any officer related messages or errors.");
                    }
                    else if (CH != null)
                    {
                        Config.SetGuildChannel(Core.WarBotChannelType.CH_Officers, CH);
                        await NextStep($"My officer-related messages and errors will be sent to {CH.Mention}.");
                    }
                    else
                        await SendAsync(msg_ChannelExpected);
                    break;
                case SetupStep.WAR_Channel:
                    if (Skip || Bool == false)
                    {
                        Config.SetGuildChannel(Core.WarBotChannelType.WAR, null);
                        Config[Setting_Key.WAR_PREP_STARTED].Enabled = false;
                        Config[Setting_Key.WAR_PREP_ENDING].Enabled = false;
                        Config[Setting_Key.WAR_STARTED].Enabled = false;
                        await SkipStep("I will not send any type of announcements for hustle castle clan wars.\r" +
                            "\nYou may enable this feature later if you wish.");
                    }
                    else if (CH != null)
                    {
                        Config.SetGuildChannel(Core.WarBotChannelType.WAR, CH);
                        await NextStep($"My war announcements will be sent to {CH.Mention}.");
                    }
                    else
                        await SendAsync(msg_ChannelExpected);
                    break;
                case SetupStep.WAR_SendPrepStarted:
                    if (Bool == true)
                    {
                        await NextStep();
                    }
                    else if (Bool == false || Skip)
                    {
                        Config[Setting_Key.WAR_PREP_STARTED].Set(false, null);
                        await SkipStep("I will not send notifications when war prep starts.");
                    }
                    else
                        await SendAsync(msg_BoolParseFailed);
                    break;
                case SetupStep.WAR_PrepStartedMessage:
                    if (Skip)
                    {
                        Config[Setting_Key.WAR_PREP_STARTED].Set(true, null);
                        await SkipStep("I will use my default notification when war prep starts.");
                    }
                    else
                    {
                        Config[Setting_Key.WAR_PREP_STARTED].Set(true, Message);
                        await NextStep("The war prep started message has been set to:\r" +
                            $"\n{Message}");
                    }
                    break;
                //War prep ending 
                case SetupStep.WAR_SendPrepEnding:
                    if (Bool == true)
                    {
                        await NextStep();
                    }
                    else if (Bool == false || Skip)
                    {
                        Config[Setting_Key.WAR_PREP_ENDING].Set(false, null);
                        await SkipStep("I will not send notifications for war prep ending.");
                    }
                    //Failed to parse a boolean.
                    else
                        await SendAsync(msg_BoolParseFailed);
                    break;
                case SetupStep.WAR_PrepEndingMessage:
                    if (Skip)
                    {
                        Config[Setting_Key.WAR_PREP_ENDING].Set(true, null);
                        await NextStep("I will use my default notification when war prep is ending.");
                    }
                    else
                    {
                        Config[Setting_Key.WAR_PREP_ENDING].Set(true, Message);
                        await NextStep("The war prep ending message has been set to:\r" +
                            $"\n{Message}");
                    }
                    break;
                //War Started
                case SetupStep.WAR_SendWarStarted:
                    if (Bool == true)
                    {
                        await NextStep();
                    }
                    else if (Bool == false || Skip)
                    {
                        Config[Setting_Key.WAR_STARTED].Set(false, null);
                        await SkipStep("I will not send a notification when clan wars start.");
                    }
                    else
                        await SendAsync(msg_BoolParseFailed);
                    break;
                case SetupStep.WAR_WarStartedMessage:
                    if (Skip)
                    {
                        Config[Setting_Key.WAR_STARTED].Set(true, null);
                        await SkipStep("I will use my default notification after war has started.");
                    }
                    else
                    {
                        Config[Setting_Key.WAR_STARTED].Set(true, Message);
                        await NextStep("The war started message has been set to:\r" +
                            $"\n{Message}");
                    }
                    break;

                //Enable specific wars
                case SetupStep.Enable_Specific_Wars:
                    if (Bool == true)
                    {
                        await NextStep();
                    }
                    else if (Bool == false || Skip)
                    {
                        Config[Setting_Key.WAR_1].Enable();
                        Config[Setting_Key.WAR_2].Enable();
                        Config[Setting_Key.WAR_3].Enable();
                        Config[Setting_Key.WAR_4].Enable();
                        await SkipStep("I will send notifications for all 4 war cycles.");
                    }
                    else
                        await SendAsync(msg_BoolParseFailed);
                    break;

                case SetupStep.WAR_1_Enabled:
                    if (Bool == true)
                    {
                        Config[Setting_Key.WAR_1].Enable();                        
                        await NextStep("I will notify for war 1.");
                    }
                    else if (Bool == false || Skip)
                    {
                        Config[Setting_Key.WAR_1].Disable();
                        await SkipStep("I will not notify for war 1.");
                    }
                    else
                        await SendAsync(msg_BoolParseFailed);
                    break;
                case SetupStep.WAR_2_Enabled:
                    if (Bool == true)
                    {
                        Config[Setting_Key.WAR_2].Enable();
                        await NextStep("I will notify for war 2.");
                    }
                    else if (Bool == false || Skip)
                    {
                        Config[Setting_Key.WAR_2].Disable();
                        await SkipStep("I will not notify for war 2.");
                    }
                    else
                        await SendAsync(msg_BoolParseFailed);
                    break;
                case SetupStep.WAR_3_Enabled:
                    if (Bool == true)
                    {
                        Config[Setting_Key.WAR_3].Enable();
                        await NextStep("I will notify for war 3.");
                    }
                    else if (Bool == false || Skip)
                    {
                        Config[Setting_Key.WAR_3].Disable();
                        await SkipStep("I will not notify for war 3.");
                    }
                    else
                        await SendAsync(msg_BoolParseFailed);
                    break;
                case SetupStep.WAR_4_Enabled:
                    if (Bool == true)
                    {
                        Config[Setting_Key.WAR_4].Enable();
                        await NextStep("I will notify for war 4.");
                    }
                    else if (Bool == false || Skip)
                    {
                        Config[Setting_Key.WAR_4].Disable();
                        await SkipStep("I will not notify for war 4.");
                    }
                    else
                        await SendAsync(msg_BoolParseFailed);
                    break;
                case SetupStep.Should_Set_Roles:
                    if (Bool == true)
                        await NextStep();
                    else if (Bool == false || Skip)
                    {
                        this.Config.ClearAllRoles();
                        await SkipStep("I will not perform any role-based management or tagging.");
                    }
                    else
                        await SendAsync(msg_BoolParseFailed);
                    break;
                case SetupStep.WAR_Clear_Channel:
                    if (Bool == true)
                    {
                        this.Config[Setting_Key.CLEAR_WAR_CHANNEL_ON_WAR_START].Enable();
                        var warch = this.Config.GetGuildChannel(WarBotChannelType.WAR);
                        await NextStep($"I will automatically purge all non-pinned messages from {warch.Mention} when the preperation peroid starts.");
                    }
                    else if (Bool == false || Skip)
                    {
                        this.Config[Setting_Key.CLEAR_WAR_CHANNEL_ON_WAR_START].Disable();
                        await SkipStep("I will not automatically empty the war channel.");
                    }
                    else
                        await SendAsync(msg_BoolParseFailed);
                    break;
                case SetupStep.Portal_Channel:
                    if (Skip || Bool == false)
                    {
                        Config.SetGuildChannel(WarBotChannelType.PORTAL, null);
                        Config[Setting_Key.PORTAL_STARTED].Disable();
                        await SkipStep("I will not send portal notifications.\r" +
                            "\nYou may enable this feature later if you wish.");
                    }
                    else if (CH != null)
                    {
                        Config.SetGuildChannel(WarBotChannelType.PORTAL, CH);
                        await NextStep($"My portal notifications will be sent to {CH.Mention}.");
                    }
                    else
                        await SendAsync(msg_ChannelExpected);
                    break;
                case SetupStep.Portal_Started_Message:
                    if (Skip)
                    {
                        Config[Setting_Key.PORTAL_STARTED].Set(true, null);
                        await SkipStep("I will use my default notification when the portal opens.");
                    }
                    else
                    {
                        Config[Setting_Key.PORTAL_STARTED].Set(true, Message);
                        await NextStep("The war started message has been set to:\r" +
                            $"\n{Message}");
                    }
                    break;
                //All of these steps share the same logic.
                case SetupStep.Role_Guest:
                case SetupStep.Role_Member:
                case SetupStep.Role_Officer:
                case SetupStep.Role_Leader:
                case SetupStep.Role_ServerAdmin:
                    var role = GetRoleFromStep(CurrentStep);
                    if (Skip)
                    {
                        Config.SetGuildRole(role, null);
                        await SkipStep();
                    }
                    else if (ROLE != null)
                    {
                        Config.SetGuildRole(role, ROLE);
                        await NextStep($"Role {role.ToString()} has been assigned to discord role {(ROLE.IsMentionable ? ROLE.Mention : ROLE.Name)}");
                    }
                    else
                        await SendAsync(msg_RoleExpected);
                    break;
                case SetupStep.Set_Website:
                    if (Skip)
                    {
                        Config.Website = null;
                        await SkipStep("No website will be configured.");
                    }
                    else
                    {
                        Config.Website = Message;
                        await NextStep("The website message has been set to:\r" +
                            $"\n{Message}");
                    }
                    break;
                case SetupStep.Set_Loot:
                    if (Skip)
                    {
                        Config.Loot = null;
                        await SkipStep("No loot message will be configured.");
                    }
                    else
                    {
                        Config.Loot = Message;
                        await NextStep("The Loot message has been set to:\r" +
                            $"\n{Message}");
                    }
                    break;
                default:
                    throw new Exception("Unhandled case.");
            }
        }


        private async Task StartStep(SetupStep step)
        {
            CurrentStep = step;

            const string suffix = "\n\r\n*You may always say 'skip' to disable this feature, 'back' to return to the previous step, or 'stop' to cancel this dialog.*\r";
            switch (step)
            {
                case SetupStep.Initial:
                    await SendAsync("Welcome to the guild setup dialog.\r" +
                        "\nI will walk you through the process of configuring me, by asking simple questions.");
                    return;

                case SetupStep.WarBot_Prefix:
                    await SendAsync("Lets start by asking what my prefix should be.\r" +
                        $"\nYou also may always address me by tagging me, like this:\r" +
                        $"\n**{this.Bot.Client.CurrentUser.Mention}, help**\r" +
                        $"\nWhat prefix should I use?\r" +
                        $"\nMy default is 'bot,'" +
                        suffix);
                    return;
                case SetupStep.User_Join_Channel:
                    await SendAsync("Which channel would you like me to use for greeting new users?\r" +
                        "\nIt is recommend the new users can see the new channel, but, you can set this to a private channel to notify your officers.\r" +
                        $"\nPlease tag the channel like this: {this.Channel.Mention}"
                        + suffix);
                    break;
                case SetupStep.User_Join_Message:
                    await SendAsync("What message would you like me to send to new users?\r" +
                        "You may use {User} in your message, which will automatically tag the user." +
                        "\nYou may 'skip' to use a default message.");
                    break;
                case SetupStep.User_Left_Channel:
                    await SendAsync("What channel would you like me to send a notification to when users leave?\r" +
                        $"\nPlease tag the channel like this: {this.Channel.Mention}"
                         + suffix);
                    break;
                case SetupStep.User_Left_Message:
                    await SendAsync("What message would you like me to send when somebody leaves your server?\r" +
                        "You may use {User} in your message, which will be replaced with the user's name" +
                        "\nYou may 'skip' to use a default message.");
                    break;
                case SetupStep.Channel_Updates:
                    await SendAsync("Occasionally, my developer will add significant new features to me.\r" +
                        "\nWould you like to receive those updates in a channel?\r" +
                        "\nIf so, please tell me which channel to send update notices to. If you do not want this, say 'No'.\r" +
                        suffix);
                    break;
                case SetupStep.Channel_Officers:
                    await SendAsync("Occasionally, I need to communicate with the clan's leadership.\r" +
                        "\nWould you like to receive those updates in a channel? (They are not very frequent.)\r" +
                        "\nIf so, please tell me which channel to use. If you do not want this, say 'No'.\r" +
                        suffix);
                    break;
                case SetupStep.WAR_Channel:
                    await SendAsync("I assume you invited me to your server, for the purpose of alerting for Hustle Castle War events.\r" +
                        "\nPlease let me know which channel I should send war announcements to.\r" +
                        "\nIf you say 'No' or 'Skip', I will disable all war-related announcements.");
                    break;
                case SetupStep.WAR_SendPrepStarted:
                    await SendAsync("Would you like me to send an announcement when the war preperation peroid starts?" +
                        "\r\nYes or No?");
                    break;
                case SetupStep.WAR_PrepStartedMessage:
                    await SendAsync("What message would you like for me to send to members when the war preperation peroid starts?" +
                        "\r\nYou may 'skip' to use a default message.");
                    break;
                case SetupStep.WAR_SendPrepEnding:
                    await SendAsync("Would you like me to send an announcement 15 minutes before the war starts?" +
                        "\r\nYes or No?");
                    break;
                case SetupStep.WAR_PrepEndingMessage:
                    await SendAsync("What message would you like for me to send before the war starts?" +
                        "\r\nYou may 'skip' to use a default message.");
                    break;
                case SetupStep.WAR_SendWarStarted:
                    await SendAsync("Would you like me to send an announcement when the war starts?" +
                        "\r\nYes or No?");
                    break;
                case SetupStep.WAR_WarStartedMessage:
                    await SendAsync("What message would you like for me to send when the war starts?" +
                        "\r\nYou may 'skip' to use a default message.");
                    break;
                case SetupStep.Enable_Specific_Wars:
                    await SendAsync("Would you like to only enable specific wars?" +
                        "\r\nYou may 'skip' or 'no' to enable all 4 war notifications.");
                    break;
                case SetupStep.WAR_1_Enabled:
                    await SendAsync("Would you like notifications for War 1? It occurs at 7am UTC\r" +
                        "\nPlease be sure to convert to your current time zone.");                  
                    break;
                case SetupStep.WAR_2_Enabled:
                    await SendAsync("Would you like notifications for War 2? It occurs at 1pm UTC\r" +
                        "\nPlease be sure to convert to your current time zone.");
                    break;
                case SetupStep.WAR_3_Enabled:
                    await SendAsync("Would you like notifications for War 3? It occurs at 7pm UTC\r" +
                        "\nPlease be sure to convert to your current time zone.");
                    break;
                case SetupStep.WAR_4_Enabled:
                    await SendAsync("Would you like notifications for War 4? It occurs at 1am UTC\r" +
                        "\nPlease be sure to convert to your current time zone.");
                    break;
                case SetupStep.WAR_Clear_Channel:
                    await SendAsync("I can automatically delete all non-pinned messages in the WAR channel when a new war preperation peroid is started.\r" +
                        "\nWould you like to enable this feature?.");
                    break;
                case SetupStep.Portal_Channel:
                    await SendAsync("If you would like a reminder every week when the portal opens, " +
                        "Please let me know which channel I should send portal messages to.\r" +
                        "\nIf you say 'No' or 'Skip', I will not send portal open notifications");
                    break;
                case SetupStep.Portal_Started_Message:
                    await SendAsync("What message would you like for me to send when the portal opens?" +
                        "\r\nYou may 'skip' to use a default message.");
                    break;

                case SetupStep.Should_Set_Roles:
                    await SendAsync("Would you like me to assist you with managing the roles of your discord server?\r" +
                        "\nI can help by promoting users, demoting users, and setting users to specific roles\r" +
                        "\nAs well, many of my functions requires a user to have a specific role\r" +
                        "\nPlease say yes or no.");
                    break;
                case SetupStep.Role_Guest:
                    await SendAsync("Which role would you like to utilize for guests?\r" +
                        "\nTypically, users in this role will not have access to many of the protected channels, or features of mine.\r" +
                        "\nPlease tag a role, like so: @Guests, or say 'skip'");
                    break;
                case SetupStep.Role_Member:
                    await SendAsync("Which role would you like to use for members?\r" +
                        "\nThese users will have access to basic commands.\r" +
                         "\nPlease tag a role, like so: @Members, or say 'skip'");
                    break;
                case SetupStep.Role_Officer:
                    await SendAsync("Which role would you like to use for officers?\r" +
                        "\nThese users will have access to a few user management commands, and will be able to set users to roles less then officer\r" +
                         "\nPlease tag a role, like so: @Officers, or say 'skip'");
                    break;
                case SetupStep.Role_Leader:
                    await SendAsync("Which role would you like to use for Leaders?\r" +
                        "\nThese users will have access to nearly all of my features around user and clan management.\r" +
                         "\nPlease tag a role, like so: @Leaders, or say 'skip'");
                    break;
                case SetupStep.Role_ServerAdmin:
                    await SendAsync("Which role would you like to use for Server Admins?\r" +
                        "\nThese users will have access to all of my commands. It is not required that you set this role, as I can detect users who have administrative permissions.\r" +
                         "\nPlease tag a role, like so: @Admins, or say 'skip'");
                    break;
                case SetupStep.Set_Website:
                    await SendAsync("I can assist in directing users to a website, or message you specify when somebody says, 'bot, website'\r" +
                        "If you would like to use this feature, please tell me the message to send. Else, say 'skip'.");
                    break;
                case SetupStep.Set_Loot:
                    await SendAsync("I can assist in pointing users to how your loot is managed when somebody says, 'bot, loot'.\r" +
                        "If you would like to use this feature, please tell me the message to send. Else, say 'skip'.");
                    break;
                case SetupStep.Done:
                    {
                        await SendAsync("You have successfully completed my setup wizard.\r" +
                            "\nYou may always type 'bot, setup' to re-run this wizard, or 'bot, help' to show your available commands.\r" +
                            "\nIf you run into any issues, you may submit an issue at https://github.com/XtremeOwnage/WarBot or, join the support server for me.\r" +
                            "\nThanks for trusting WarBOT with all of your needs!");

                        await Config.SaveConfig();

                        await Bot.CloseDialog(this);
                    }
                    break;
            }
        }
        public async override Task OnCreated()
        {
            await StartStep(SetupStep.Initial);
            await StartStep(SetupStep.WarBot_Prefix);
        }
        public async override Task OnClosed()
        {
            await base.OnClosed();
            await this.Channel.SendMessageAsync("The guild setup dialog has been closed.");
        }

        /// <summary>
        /// Defines the current step of this dialog. This enum also contains attributes to manage the flow.
        /// </summary>
        enum SetupStep
        {
            NULL,
            Initial,

            //Setup the prefix for the bot.            
            WarBot_Prefix,

            [Skip(User_Left_Channel)]
            User_Join_Channel,
            User_Join_Message,

            [Skip(Channel_Updates)]
            User_Left_Channel,
            User_Left_Message,

            //Configure various channels            
            Channel_Updates,
            Channel_Officers,

            //Hustle Castle - War Related Settings
            [Skip(Portal_Channel)]
            WAR_Channel,

            [Skip(WAR_SendPrepEnding)]
            WAR_SendPrepStarted,
            WAR_PrepStartedMessage,
            [Skip(WAR_SendWarStarted)]
            WAR_SendPrepEnding,
            WAR_PrepEndingMessage,
            [Skip(Enable_Specific_Wars)]
            WAR_SendWarStarted,
            WAR_WarStartedMessage,

            [Skip(WAR_Clear_Channel)]
            Enable_Specific_Wars,
            WAR_1_Enabled,
            WAR_2_Enabled,
            WAR_3_Enabled,
            WAR_4_Enabled,

            WAR_Clear_Channel,

            //Portal Enabled / Portal Message
            [Skip(Should_Set_Roles)]
            Portal_Channel,
            Portal_Started_Message,

            //Roles    
            [Skip(Set_Website)]
            Should_Set_Roles,
            Role_Guest,
            Role_Member,
            Role_Officer,
            Role_Leader,
            Role_ServerAdmin,

            Set_Website,
            Set_Loot,

            Done,
        }

        #region Various other related pieces of code, helpers and shortcuts.
        private RoleLevel GetRoleFromStep(SetupStep Step)
        {
            switch (Step)
            {
                case SetupStep.Role_Guest:
                    return RoleLevel.Guest;
                case SetupStep.Role_Member:
                    return RoleLevel.Member;
                case SetupStep.Role_Officer:
                    return RoleLevel.Officer;
                case SetupStep.Role_Leader:
                    return RoleLevel.Leader;
                case SetupStep.Role_ServerAdmin:
                    return RoleLevel.ServerAdmin;
                default:
                    throw new Exception("Bad input. That step does not have an associated role.");
            }

        }
        private async Task NextStep(string Message = null)
        {
            //add the current step to the history buffer.
            History.Push(CurrentStep);

            if (Message != null)
                await SendAsync(Message);

            SetupStep NextStep = CurrentStep + 1;
            await StartStep(NextStep);
        }
        private async Task SkipStep(string Message = null)
        {
            //add the current step to the history buffer.
            History.Push(CurrentStep);

            if (Message != null)
                await SendAsync(Message);

            //Determine if we can locate a SkipAttribute.
            var type = typeof(SetupStep);
            var name = Enum.GetName(type, CurrentStep);
            var skipStepAttribute = type.GetField(name).GetCustomAttributes(false).OfType<SkipAttribute>().SingleOrDefault();

            //If the skip attribute exists, skip to the defined step.
            if (skipStepAttribute != null)
                await StartStep(skipStepAttribute.SkipStep);
            else
                //Else, go to the next step.
                await StartStep(CurrentStep + 1);
        }
        private async Task PreviousStep()
        {
            //First check the history queue.
            if (History.TryPop(out var Step))
                await StartStep(Step);
            else
                //If we fail to check the history queue, start at the beginning.
                await StartStep(SetupStep.Initial + 1);

        }

        class SkipAttribute : Attribute
        {
            public SetupStep SkipStep { get; set; }
            public SkipAttribute(SetupStep Skip)
            {
                this.SkipStep = Skip;
            }
        }
        #endregion
    }
}
