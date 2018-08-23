Version 3.0 of bot. Will be converted to use .net core.

# Major Changes
* Entire re-write to utilize .net core instead of node.js / typescript.
	* Logic was added to migrate your current configurations to the new version of the bot.
	* For me as a developer, it is MUCH easier to add new functionality.

# Minor Changes
* There will no longer be seperate notifications to the officers channel. Although, this was togglable before, after revewing use of the bot, I determined the additional notification was not required.
* The "Channels" are now differant. 
	* Instead of Members, Officers channels, You have...
		* Welcome Channel - If, you configure the bot to welcome new users, it will be announced in this channel.
		* WAR Channel - This is where warbot will send the war started messages.
		* Officers Channel - This will be used for user management purposes. To come...
		* Updates Channel - If you wish to see news regarding warbot (Will not happen frequenetly) it will happen in this channel.

# New Commands / Updated Commands
* Remind Me
	- Ever want a reminder for something? WARBot can assist with that now!
	```
	bot, remind me 1d Do Something
	bot, remind me 1m do something in a minute.
	bot, remind me 10:02:12 Remind me in 10 hours, 2 minutes, and 12 seconds.
	alias: bot, remind {span} {message}
	```
	- If WARBot is unable to send messages in the channel where the command was entered, WARBOT will DM you instead.
	- As well, this command can be used in a DM to warbot.
* Update a user's nickname.
	```
	bot, set nickname @User NewNickName
	alias: bot, nickname @User NewNickName
	```
* Update WarBot's nickname
	```
	bot, nickname NewNickName
	alias: bot, set nickname NewNickName
	```
* Promote and Demote Members
	```
	bot, promote @member, [@member2, @member3, etc....]
	bot, demote @member, [@member2, @member3, etc....]
	bot, set role {RoleName} @User, [@User2, @User3, etc....]
		alias: bot, add role
		alias: bot, remove role
	```
	* You can promote one or more users at a time. Just tag multiple users. No commas required.
	* To See available roles:
		```
		bot, set role
			alias: bot, set role ?
		```
	* These commands will promote or demote a user to the next configured rank.
	* Note, if there are multiple WarBot roles, with the same Guild role, the highest warbot role will be used.
		* Ex: You have Officer set to @Admins, and Leaders set to @Admins.
		* A user will be promoted from member to Leader, since, it is the same role.
	* Note, The promote command, will not promote users to above the level of leader. 
		* To promote to server admin, please use: bot, set role ServerAdmin @user, (@moreusers)
* Show current configuration
	```
	- Show the selected configuration sections
		bot, show config [All, notifications, Channels, Roles]
	- Print a list of available options. Multiple options may be selected.
		bot, show config ?
		
	```
	* You can now display a certain config element, or multiple elements. It will default to show all config items.