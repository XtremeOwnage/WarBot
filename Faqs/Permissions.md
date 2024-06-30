---
title: Too many permissions
---

# Too Many Permissions! I don't want to grant administrator

Well, Warbot actually doesn't require administrator. However, its easier to request administrator, as opposed to the list of permissions actually required.

If, you want to tightly control WarBot's permissions, here is the full list of permissions, along with the reason they are required:


## Create Commands in a server
This ability permission is required for WarBOT to create commands in your server. Without it, you will not be able to use any of warbot's commands.



## Permissions List

#### View Channels 
We need to see the channels in your guild in order to allow you to choose target channels for actions

#### Manage Roles 
This is needed for Warbot's [Role Management](./../Features/RoleManagement.md) feature, as well as any [Custom Commands](./../Features/CustomCommands.md) you create with an action to add or remove a user from a role.

#### Change Nickname
Gives WarBOT the ability to change his own nickname, when you tell him to.

#### Manage Nicknames
Allows you to change your own, or another user's nickname by typing `/nickname`

#### Kick Members
If you plan on using `/kick`, this permission is needed.

#### Ban Members
Likewise, if you plan on using `/ban`, this permission is required.

#### Manage Events
This permission is required for Warbot to create events. These are optionally used for [Hustle Castle Wars](./../Features/HustleFeatures.md)

#### Send Messages
If you want WarBOT to be able to actually send notifications and messages, you kind of need to grant him this permission.

#### Send Messages In Threads
[Hustle Castle](./../Features/HustleFeatures.md) can optionally announce war messages in threads, which greatly assists in cleaning up old messages.

#### Create Public Threads
Likewise, for the above, it needs to be able to create the thread.

#### Add Reactions
To enable [Voting](./../Features/Voting.md) in discord, we leverage emojis. If we cannot add reactions, we cannot create polls.

#### Mention @everyone, @here, and All Roles
If you setup a [Custom Command](./../Features/CustomCommands.md), [Hustle Event](./../Features/HustleFeatures.md), or any other action which mentions @here or @everyone, this permission is required.

#### Manage Messages
Needed for [Clear Messages](./../Features/ClearMessages.md) functionality. Cannot delete messages if you cannot manage them.

#### Read Message History
Also needed for [Clear Messages](./../Features/ClearMessages.md) functionality.

#### Use Application Commands
This is tied into [Custom Commands](./../Features/CustomCommands.md)