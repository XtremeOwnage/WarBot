Version 3.0 of bot. Will be converted to use .net core.

# Major Changes
* Entire re-write to utilize .net core instead of node.js / typescript.
	* Logic was added to migrate your current configurations to the new version of the bot.

# Minor Changes
* There will no longer be seperate notifications to the officers channel. Although, this was togglable before, after revewing use of the bot, I determined the additional notification was not required.
* The "Channels" are now differant. 
	* Instead of Members, Officers channels, You have...
		* Welcome Channel - If, you configure the bot to welcome new users, it will be announced in this channel.
		* WAR Channel - This is where warbot will send the war started messages.
		* Officers Channel - This will be used for user management purposes. To come...
		* Updates Channel - If you wish to see news regarding warbot (Will not happen frequenetly) it will happen in this channel.

# New Commands / Updated Commands
* Updates a user's nickname.
	```
	bot, set nickname @User NewNickName
	```
	* alias: bot, nickname
	* bot, nickname {nickname} will update WarBOT's nickname.
* Promote and Demote Members
	* bot, promote @member, [@member2, @member3, etc....]
	* bot, demote @member, [@member2, @member3, etc....]
	* bot, set role {RoleName} @User, [@User2, @User3, etc....]
		* You can add multiple users to a provided role.
	* To See available roles, type bot, set role

	* These commands will promote or demote a user to the next configured rank.
	* Note, if there are multiple WarBot roles, with the same Guild role, the highest warbot role will be used.
		* Ex: You have Officer set to @Admins, and Leaders set to @Admins.
		* A user will be promoted from member to Leader, since, it is the same role.
	* Note, The promote command, will not promote users to above the level of leader. 
		* To promote to server admin, please use: bot, set role ServerAdmin @user, (@moreusers)
* bot, show config [All, notifications, Channels, Roles]
	* You can now display a certain config element, or multiple elements. It will default to show all config items.