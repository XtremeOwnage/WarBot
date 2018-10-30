# WarBot
Warbot is a discord bot designed for the hustle-castle mobile phone game. It is intended to display a notification when wars are starting, to assist with reminding members to deploy their troops.

## What is WarBOT

Warbot is a discord bot designed around the Hustle Castle mobile game. While, many of the features are specific to this game, you do not need to play this game to utilize WarBOT.

## Basic Features

### Discord User/Role Management
* Can promote, demote, and set user to role(s).
* Keeps track of the last time a user was online in discord.
* Keeps track of the last time a user sent a message in discord.
* Kick Users. Not a huge feature, but, It does leave a nice "Kicked" message.

### Voting
* Ever need a standarized way to initiate a vote in Discord? WarBOT is here to help!
	* bot, vote (TimeSpan) Question

### Reminders
* In addition to WAR reminders for hustle castle, you can also ask WarBOT to remind you of something. This feature is available in both guilds, and via DM to the bot.
* WarBOT can also let you know when the portal opens for the week.

### Cleaning up channels
* You can use WarBOT to delete messages from a channel, with the option to filter on pinned, nonpinned, or all messages.

### New Member Welcome Messages
* WarBOT can send a greeting message to new users.
* WarBOT can also let you know when somebody leaves your server.

## Hustle Castle - Specific Features

### WAR Prep / War Prep Ending / War Started Reminders
* Toggable notifications when war prep starts, war prep is ending, and war is started.
	* Customizable messages.

### Keep track of your clan members
* Can force discord nickname to match in-game nickname.
* Keeps track of the day they joined.

# Guided Setup

To have WarBOT walk assist you with configuration, just use:

* bot, setup

# Advanced Setup

The following commands should provision most, if not all features of WarBot

If you wish to change WarBOT's prefix,
* bot, set prefix !
* bot, set prefix bot,

To list available roles you can set, use
* bot, set role 

For examples on how to set roles:
* bot, set role guest @Guest
* bot, set role SuperMember @SpecialMembers
* bot, set role Leader @Leaders

List list configurable channels, use
* bot, set channel

For Examples on how to set channels:
* bot, set channel CH_WarBot_Updates #bot_news
* bot, set channel CH_WAR_Announcements #WAR
* bot, set channel CH_Officers #officers
* bot, set channel CH_New_Users #welcome

Other common settings
* bot, set website Please visit http://my_site_here/ to see our website.
* bot, set loot For directions on how to request warchest loot, please see #War_Loot_Rules

# Validate Settings

To validate or display the current settings configured, please use the command:
* bot, show config

# Invite URL
To Invite WarBot to your discord server, Please visit this url:

https://xtremeownage.com/index.php?link-forums/discord-invite-link.53/

# Usage and Further Assistance

The following commands, will direct the bot to print available commands for your role level.
* bot, help
* @WarBot help

Warbot will reply to either bot, {command} or @WarBot {command}.


# Other

More details can be found at https://xtremeownage.com/index.php?threads/version-1-0-released.20561/

You may join the WarBot Development discord server @ https://discord.gg/uEkamH9 for support.

To submit a request for a new feature, please submit through github via https://github.com/XtremeOwnage/WarBot/issues/new/choose
