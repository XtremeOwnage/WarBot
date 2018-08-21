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

# New Commands
* bot, set nickname @User NewNickName - Updates a user's nickname.
	* alias: bot, nickname
	* bot, nickname {nickname} will update WarBOT's nickname.