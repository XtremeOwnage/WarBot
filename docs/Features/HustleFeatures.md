---
title: Hustle Castle
description: Documentation around various hustle-castle specific features
---

# Hustle Castle

Being Warbot was orignally founded around assisting with discord management around the game hustle-castle.... Warbot has a lot of functionality to assist with this task.

For basics, it can manage your portal and war events.

## Portal Events

The portal event in hustle castle, opens once a week, at 09:00 UTC.

WarBOT can optionally let you know when this happens.

### To configure and enable portal events

#### Via [Warbot Configuration Site](https://warbot.dev/)

First, navigate to "Hustle Castle Settings"
Next, select "Portal" in the tab menu.

You may enable or disable the notifications by selecting "Event Enabled"

Set the message to send, and select a target channel in the dropdown menu.

Click save.

#### Via Commands (Requires Leader Role.)

##### Set Target Channel (Required)
    /portal channel channel:#portal 

##### Set the message to send.
You may optionally tag a role... or link a channel.
    /portal message message:The portal has opened @Role 

##### Enable portal
    /portal enable 

##### Disable portal
    /portal disable


## War Events

##### War Times
There are four wars every day in hustle castle.

* War 1 - 09:00 UTC
* War 2 - 15:00 UTC
* War 3 - 21:00 UTC
* War 4 - 03:00 UTC

For each of these wars, you can optionally enable events and notifications.

### To configure and enable war events

#### Via [Warbot Configuration Site](https://warbot.dev/)

First, navigate to [Hustle Castle Settings](https://warbot.dev/Config/Hustle)

Select the desired war to configure from the menu at the top, and... configure the desired options.

Each war has its own unique configuration. Each one can be optionally enabled or disabled independantly.

You can fully customize the messages for each phase, of each war.

##### Enabled

If the event is not enabled, you will not receive any notifications or events for it.

As well, the other options will not show until you click enabled.

##### War Prep Started Message

This message is sent when the war prep peroid starts, two hours before the war.

If you do not wish to receive a message when the prep peroid starts, leave this field empty.

##### War Prep Ending Message

This message is sent 15 minutes before the war starts.

If you do not wish to receive this notification, leave the field empty.

##### War Started Message

This message is sent when the war starts.

As before, if you do not want this message, leave this field empty.

##### Channel

This is the channel the above notifications will be delivered to. This field is not optional.

Even if you only wish to send the discord events, you still need to fill out the channel field. Needs to be a valid text channel.

##### Discord Event Enabled

This is a new feature introduced with Warbot v4.0

[Discord events](https://support.discord.com/hc/en-us/articles/4409494125719-Scheduled-Events) are a special new feature in discord.

By enable this option, a discord event will be created two hours before the preperation period starts, giving players plenty of time to "opt-in" to the event.

##### Discord Event Title

This is the title of the created event. Required.

##### Discord Event Description

This will be the description of the generated event. Required.

#### Via Commands

Guild members with the leader role, and/or server admins can configure war events via commands.

The war times are [posted above](#war-times)

##### Set War Channel

Setting the channel is performed through the `/war channel` command.

To set the channel for a single war:

    /war channel war:WAR_1 channel:#general 

To set the channel for ALL of the war events, use `war:ALL`

    /war channel war:ALL channel:#general 

##### Setting War Messages

Setting the messages is performed via the `/war messages` command.

    /war message war:WAR_1 event:Event_Prep_Starting message:Prep has started, deploy those troops! 
    /war message war:WAR_1 event:Event_Prep_Starting message:Prep period is ending. 
    /war message war:WAR_1 event:Event_Started  message:War has started! 

Like before, if you wish to set the messages for ALL wars, you can use `war:ALL`

    /war message war:ALL event:Event_Started message:The war has started!

##### Enabling or Disabling War

You can enable or disable a war by simpally typing

    /war enable war:WAR_1
    /war disable war:WAR_1

Or, enable or disable all wars

    /war enable war:ALL
    /war disable war:ALL