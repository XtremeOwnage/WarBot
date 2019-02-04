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

Ever need a standarized way to initiate a vote in Discord? WarBOT is here to help!

```
bot, vote 5m Question goes here
```

* The duration of the poll is adjustable from 5 minutes, up to 1 day.
* You have configurable options.
* The entire poll entry process is guided.

### Reminders
* In addition to WAR reminders for hustle castle, you can also ask WarBOT to remind you of something. This feature is available in both guilds, and via DM to the bot.
* WarBOT can also let you know when the portal opens for the week.

### Cleaning up channels
* You can use WarBOT to delete messages from a channel, with the option to filter on pinned, nonpinned, or all messages.

```
# Clear all unpinned messages.
bot, clear
# Clear ALL Messages, pinned and unpinned.
bot, clear pinned
```

### New Member Welcome Messages
* WarBOT can send a greeting message to new users.

```
# Specify greeting message and channel.
bot, enable greeting #Welcome Welcome to our guild new user!

# Specify greeting message only.
bot, enable greeting Welcome new user!

# Specify greeting channel.
bot, set channel user_join #Welcome
```

* WarBOT can also let you know when somebody leaves your server.

```
#Enable this feature, and specify the target channel.
bot, enable leave #Channel
```

# Guided Setup

To have WarBOT walk assist you with configuration, just use:

```
bot, setup
```

This command will launch a setup wizard to guide you through the majority of WarBOT's configuration.
You may run it again at any time.

## Hustle Castle - Specific Features

### WAR Prep / War Prep Ending / War Started Reminders
* Toggable notifications when war prep starts, war prep is ending, and war is started.
	* Customizable messages.

```
# Set the message when the war prep peroid starts
bot, set war prep started @here - The war prep peroid has started, Go Deploy!

# Set the message 15 minutes before the war starts
bot, set war prep ending @MyClanRole - The peroid to place your troops is almost over, go deploy if you have not already!

# Set the message when war is started
bot, set war started The war has started! @here

# Enable or Disable war prep started
bot, disable war prep started
bot, enable war prep started

# Enable or Disable war prep ending
bot, disable war prep ending
bot, enable war prep ending

# Enable or Disable war started
bot, disable war started
bot, enable war started

#Specify the channel where WAR messages are sent
bot, set channel war #War_Announcements
```

#### You also have the ability to specify a different message for EACH war
* Note- Exactly three semicolons are required.
* You must specify 4 messages, even if you don't do all 4 wars.
* This ability works for all of the types of war messages, independantly.

```
# Unique message for each of the 4 daily wars
# Note- Requires exactly 3 semicolons, seperating the messages.
# Press shift+enter to make a linebreak without sending the message

bot, set war prep started 1am War, @here, To Deploy;
7 am war, @All, Go Deploy!;
@everybody, Go deploy for the 1pm war!!!;
@here The 7pm war has started, feel free to join in!

```

#### Enabling or Disabling specific wars
You don't care about the war happening at 1am? No problem! You can disable that.
```
# War 1 = 7am UTC
bot, enable war 1
bot, disable war 1

#War 2 = 1pm UTC
bot, enable war 2
bot, disable war 2

# War 3 = 7pm UTC
bot, enable war 3
bot, disable war 3

#War 4 = 1am UTC
bot, enable war 4
bot, disable war 4
```

#### Validating the war messages are configured properly.

This command will validate you have the channels and permissions properly configured to receive war messages.
It will also send out a test message for each message, and war.

```
bot, test war messages
```

# Advanced Setup

The following commands should provision most, if not all features of WarBot

### Set WarBOT's prefix (This is what the bot listens to)
```
bot, set prefix !
	!stats
    
bot, set prefix bot,
	bot, stats
    
@WarBOT set prefix Gandolf
	Gandolf, stats
   
```

* If you ever forget the bot's prefix, you may always tag the bot, like so:

```
@WarBOT, set prefix bot,
```

### Role Management

```
# To list available roles you can set, use
bot, set role 

# Examples on setting roles
bot, set role guest @Guest
bot, set role SuperMember @SpecialMembers
bot, set role Leader @Leaders
```

Promoting and demoting users
```
# Promote a user to the next role
bot, promote @user

# Demote a user down a role
bot, demote @user

# Set a user to a specific role
bot, set role LEADER @user
bot, set role NONE @guest

# Show configured roles
bot, show config roles
```

### Channels
```
# List configurable channels (Does not set anything)
bot, set channel

# Examples
bot, set channel CH_WarBot_Updates #bot_news
bot, set channel CH_WAR_Announcements #WAR
bot, set channel CH_Officers #officers
bot, set channel CH_New_Users #welcome

# Show configured Channels
bot, show config channels
```
### Other common settings
```
# Set the clan's website
bot, set website Please visit http://my_site_here/ to see our website.

# Show the clans website - Usable by all members, including guests
bot, website
bot, show website

# Set the clan's loot directions, or message, or website
bot, set loot For directions on how to request warchest loot, please see #War_Loot_Rules
bot, set loot https://google.com/?q=Give me loot

# Show the clan's configured loot message
bot, loot
bot, show loot
```

# Validate Settings

To validate or display the current settings configured, please use the command:
```
bot, show config
```

# Invite URL
To Invite WarBot to your discord server, Please visit this url:

https://xtremeownage.com/index.php?link-forums/discord-invite-link.53/

# Usage and Further Assistance

The following commands, will direct the bot to print available commands for your role level.
```
# Display a list of all available commands to your role
bot, help
```

Warbot will reply to either bot, {command} or @WarBot {command}.

# Faqs

#### The bot is replying, but, is I cannot see any text.

* You must have link previews enabled. Discord does not distinguish between embeds (a formatted way to display information), and actual link previews.

#### The bot is collecting and selling my information

* Not at all. I have better things to do then to collect your discord transcripts! Feel free to look through the source code.

# Other

More details can be found at https://xtremeownage.com/index.php?threads/version-1-0-released.20561/

You may join the WarBot Development discord server @ https://discord.gg/uEkamH9 for support.

To submit a request for a new feature, please submit through github via https://github.com/XtremeOwnage/WarBot/issues/new/choose
