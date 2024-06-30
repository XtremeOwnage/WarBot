---
title: Hustle Castle - Allow Member Opt-In
description: How to configure warbot to allow members to opt into each war.
---


# Summary

This is a  question I get asked a lot, and its been a requested feature for a few years. 

The ability for members to opt-in for specific wars.... Because lets face it. People need to sleep.

Well, WarBOT has your back. 

## How this will work?

For this to work properly, we will need to create four roles within discord. One for each of the wars.

For the purpose of this article, I will refer to these roles as:

1. @war-1
2. @war-2
3. @war-3
4. @war-4

We will configure a custom command in warbot, which will allow users to be added, or removed from the above roles.

We will then configure the war notifications, to tag the specific roles, for each war.

This way, only the users included in the roles, will be tagged.

## Making it happen

### Step 1. Create the discord roles

You will need to create the four roles within your discord server.

!!! info
    Ensure Warbot's role is ABOVE these four roles. Otherwise, the bot will not be able to manage members in your new roles.

### Step 2. Create Custom Commands

You will need to create 8 [Custom Commands](./../Features/CustomCommands.md)

You will need ideally need a command to add a user to the role, and a command to remove the user from the role.

Create these 8 commands, We will associate the actions in the next step:

1. war-1
2. war-2
3. war-3
4. war-4
5. war-1-optout
6. war-2-optout
7. war-3-optout
8. war-4-optout

### Step 3. Add actions to add users to roles.

For the first four commands, we will add two actions.

#### Action 1. [Add User To Role](./../../Features/CustomCommands/#add-user-to-role)

For each of the war commands, you will want to add this action, to add the user to the @war-* group.

ex- for the command `/war-1`, you will want to add an action to add the user to the @war1 role.

#### Action 2. [Reply with message](./../../Features/CustomCommands/#reply-with-message)

Message: You will now receive notifications for War x. (Replace x, with the war number, or friendly name of your choosing.)

### Step 4. Add actions to remove users from roles.

For the last four commands, we will add two actions, to remove the users from the role, in the event they wish to opt-out.

#### Action 1. [Remove User From Role](./../../Features/CustomCommands/#remove-user-from-role)

For each of the war commands, you will want to add this action, to remove the user from the @war-* group.

ex- for the command `/war-1`, you will want to add an action to remove the user from the @war1 role.

#### Action 2. [Reply with message](./../../Features/CustomCommands/#reply-with-message)

Message: You will no longer receive notifications for War x. (Replace x, with the war number, or friendly name of your choosing.)

### Step 5. Add notifications to each of the wars

For this step, you will modify the notifications for each of the wars.

See [Hustle Castle War Configuration](./../../Features/HustleFeatures/#war-events)

It may be easier to leverage the command line for adding roles, since the roles are strongly-typed within discord. 

However, if you wish to leverage the configuration website, This is the format to tag a role: `<@&ROLE_ID>` where ROLE_ID corresponds to the ID of the role within discord.

To configure these events via discord, here is a simple method of doing so.

    /war message war:WAR_1 event:Event_Prep_Starting message: @war-1 Preperation has started, deploy your troops!
    /war message war:WAR_1 event:Event_Prep_Ending message: @war-1 War starts in 15 minutes, deploy if you have not already done so!!
    /war message war:WAR_1 event:Event_Started message: @war-1 The war has started

    /war message war:WAR_2 event:Event_Prep_Starting message: @war-2 Preperation has started, deploy your troops!
    /war message war:WAR_2 event:Event_Prep_Ending message: @war-2 War starts in 15 minutes, deploy if you have not already done so!!
    /war message war:WAR_2 event:Event_Started message: @war-2 The war has started

    /war message war:WAR_3 event:Event_Prep_Starting message: @war-3 Preperation has started, deploy your troops!
    /war message war:WAR_3 event:Event_Prep_Ending message: @war-3 War starts in 15 minutes, deploy if you have not already done so!!
    /war message war:WAR_3 event:Event_Started message: @war-3 The war has started

    /war message war:WAR_4 event:Event_Prep_Starting message: @war-4 Preperation has started, deploy your troops!
    /war message war:WAR_4 event:Event_Prep_Ending message: @war-4 War starts in 15 minutes, deploy if you have not already done so!!
    /war message war:WAR_4 event:Event_Started message: @war-4 The war has started