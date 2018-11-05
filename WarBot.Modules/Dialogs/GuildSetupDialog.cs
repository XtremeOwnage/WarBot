using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using WarBot.Core.Dialogs;
using WarBot.Core.Helper;
using WarBot.Core.ModuleType;
using System.Linq;
using WarBot.Core;

namespace WarBot.Modules.Dialogs
{
    public class SetupDialog : SocketGuildDialogContextBase
    {
        public SetupDialog(GuildCommandContext Context)
            : base(Context) { }


        private SetupStep CurrentStep = SetupStep.Initial;
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
                        await NextStep($"When a user leaves, I will post to {CH.Mention}");
                    }
                    else
                        await SendAsync(msg_ChannelExpected);
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
                            $"\nIn channel {Config.GetGuildChannel(Core.WarBotChannelType.USER_JOIN).Mention}");
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
                case SetupStep.Portal_Started:
                    if (Bool == true)
                        await NextStep();
                    else if (Bool == false || Skip)
                    {
                        Config[Setting_Key.PORTAL_STARTED].Set(false, null);
                        await SkipStep("I will not send a notification when the portal opens.");
                    }
                    else
                        await SendAsync(msg_BoolParseFailed);
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
                        $"\nWhat prefix should I use?" +
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
                        "\nYou may 'skip' to use a default message.");
                    break;
                case SetupStep.User_Left_Channel:
                    await SendAsync("What channel would you like me to send a notification to when users leave?\r" +
                        "\nIf you do not want this feature, please say 'skip'.");
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
                        "\nPlease let me know which channel I should send war announcments to.\r" +
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
                case SetupStep.Portal_Started:
                    await SendAsync("Would you like me to send an announcement when the portal opens once per week?" +
                        "\r\nYes or No?");
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

            [Step(Initial, WarBot_Prefix)]
            Initial,

            //Setup the prefix for the bot.
            [Step(WarBot_Prefix, User_Join_Channel)]
            WarBot_Prefix,

            [Step(WarBot_Prefix, User_Join_Message, User_Left_Channel)]
            User_Join_Channel,

            [Step(User_Join_Channel, User_Left_Channel)]
            User_Join_Message,

            [Step(User_Join_Channel, Channel_Updates)]
            User_Left_Channel,

            //Configure various channels
            [Step(User_Left_Channel, Channel_Officers)]
            Channel_Updates,

            [Step(Channel_Updates, WAR_Channel)]
            Channel_Officers,

            //Hustle Castle - War Related Settings
            [Step(Channel_Officers, WAR_SendPrepStarted, Portal_Started)]
            WAR_Channel,
            [Step(WAR_Channel, WAR_PrepStartedMessage, WAR_SendPrepEnding)]
            WAR_SendPrepStarted,
            [Step(WAR_SendPrepStarted, WAR_SendPrepEnding)]
            WAR_PrepStartedMessage,
            [Step(WAR_SendPrepStarted, WAR_PrepEndingMessage, WAR_SendWarStarted)]
            WAR_SendPrepEnding,
            [Step(WAR_SendPrepEnding, WAR_SendWarStarted)]
            WAR_PrepEndingMessage,
            [Step(WAR_SendPrepEnding, WAR_WarStartedMessage, Portal_Started)]
            WAR_SendWarStarted,
            [Step(WAR_SendWarStarted, Portal_Started)]
            WAR_WarStartedMessage,

            //Portal Enabled / Portal Message
            [Step(WAR_Channel, Portal_Started_Message, Should_Set_Roles)]
            Portal_Started,
            [Step(Portal_Started, Should_Set_Roles)]
            Portal_Started_Message,

            //Roles    
            [Step(WAR_Channel, Role_Guest, Set_Website)]
            Should_Set_Roles,
            [Step(Should_Set_Roles, Role_Member)]
            Role_Guest,
            [Step(Role_Guest, Role_Officer)]
            Role_Member,
            [Step(Role_Member, Role_Leader)]
            Role_Officer,
            [Step(Role_Officer, Role_ServerAdmin)]
            Role_Leader,
            [Step(Role_Leader, Set_Website)]
            Role_ServerAdmin,

            [Step(Should_Set_Roles, Set_Loot)]
            Set_Website,

            [Step(Set_Website, Done)]
            Set_Loot,

            [Step(Set_Loot, Done)]
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
            if (Message != null)
                await SendAsync(Message);
            SetupStep NextStep = GetStep(CurrentStep).NextStep;
            await StartStep(NextStep);
        }
        private async Task SkipStep(string Message = null)
        {
            if (Message != null)
                await SendAsync(Message);
            SetupStep SkipStep = GetStep(CurrentStep).SkipStep;
            await StartStep(SkipStep);
        }
        private async Task PreviousStep()
        {
            SetupStep PrevStep = GetStep(CurrentStep).PreviousStep;
            await StartStep(PrevStep);
        }
        StepAttribute GetStep(SetupStep Value)
        {
            var type = typeof(SetupStep);
            var name = Enum.GetName(type, Value);
            return type.GetField(name).GetCustomAttributes(false).OfType<StepAttribute>().SingleOrDefault();
        }
        class StepAttribute : Attribute
        {
            public SetupStep PreviousStep { get; set; }
            public SetupStep NextStep { get; set; }
            public SetupStep SkipStep { get; set; }
            public StepAttribute(SetupStep Previous, SetupStep Next, SetupStep Skip = SetupStep.NULL)
            {
                this.PreviousStep = Previous;
                this.NextStep = Next;
                this.SkipStep = Skip == SetupStep.NULL ? Next : Skip;
            }
        }
        #endregion
    }
}
