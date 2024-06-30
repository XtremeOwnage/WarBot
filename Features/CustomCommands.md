---
title: Custom Commands
description: Documentation around utilizing custom commands
---

# Custom Commands

Custom commands are a very powerful addition to WarBOT. Custom commands, are slash commands specific to your guild, which you can define.

Essentially, a custom command is composed of two parts.

1. The command. This is what users will type to execute the command.
2. The actions. A single command may contain many actions.


## Creating Commands

To create a command, first, visit the [WarBOT Configuration Site](https://warbot.dev/Config/Commands), and select Commands on the left.

Next, you will see all custom commands currently configured for your guild with basic details. At the bottom, there will be a button to "Create new command". Select it.

This will bring you to a new page, with 4 options you must set.

##### Enabled
If the command is not enabled, it will not be published to your server. If you wish for users to have access to your command, it must be enabled.

##### Command Name
This is the name of the command.

ie- if you name the command..... 'dance', users would type `/dance` to execute the command

!!! info 
    Commands must be all lower case, alpha-numeric only, and between 3 to 30 characters long. These restrictions are imposed by [Discord](https://discord.com/developers/docs/interactions/application-commands#application-command-object).
    You are allowed to use -

!!! warning
    Commands must also be unique per guild. You cannot have two commands named 'dance', for example. You should also not create a command named after one of warbot's existing commands.

##### Description
This is for a friendly description to help you remember what this command is used for. Required.

##### Minimum Role Level
If you wish to only allow users with a specific role level, or higher, you may set this option.

Otherwise, leave set to 'none' to disable filtering by role level.

!!! note
    The role level filter is not yet implemented.

## Creating Actions

Once you have saved your new command, you will see a new button appear, named "Add Action". Click this button to add an action.

Once you have clicked "Add Action", a new form will appear.

Initially, you will only have one changable option, for "Action Type"

After selecting an action type, more fields will appear.

### Action Types

#### Reply With Message
This action type will reply to the user executing the command, with the message you enter in the "Message" prompt.

!!! note 
    You are only allowed to have a single "Reply" action, per command.

#### Send Message to Channel
This action type, will send the message you enter in the "Message" prompt, to the channel you select in the "Target Channel" prompt.

#### Add User To Role
This action type, will add the user to the role you specify in the target role prompt.

!!! caution
    Ensure Warbot's role is placed higher then the target role on your discord server's role list. If it is not, warbot will not be able to add users to the target role.

#### Remove User From Role
This action type, will remove the user from the role you specify in the target role prompt.

!!! caution
    Ensure Warbot's role is placed higher then the target role on your discord server's role list. If it is not, warbot will not be able to remove users from the target role.

## Loading commands in your guild

After you have created the custom commands, and you are ready to put them to use.... You will need to execute a command in your discord guild.

    /reload-commands 

Thats it. The custom commands should pop up within a few seconds afterwards. 

!!! warning
    There is a discord limit on the number of custom application command updates we can push per day. This is documented in the [Discord Documentation](https://discord.com/developers/docs/interactions/application-commands#registering-a-command)
    Every time you execute `/reload-commands`, it will reload all of your guild's custom commands. If you have... 20 commands, this counts as 20 hits. After you do this 10 times, you will no longer be able to update commands for the rest of the way.
!!! info
    You only need to `/reload-commands` after adding or removing a custom command. You do not need to execute this if you are changing actions on an existing command.