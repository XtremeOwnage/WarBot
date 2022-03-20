---
title: Greetings & Farewells
description: Documentation regarding greeting users and sending farewell messages
---

# User greeting and farewells

Warbot has the ability to greet users joining your guild, and let you know when someone has left your guild.


## New User Greetings

A new user greeting, is a message Warbot will send to your desired channel when a user joins your server.

You can enable and configure greetings either via commands, or by the configuration website.

### Configure Using Configuration Site.

First, visit the [Warbot configuration site](https://warbot.dev/Config/Events) and navigate to "Configure Events".

Next, select "User Joined" in the tab menu.

From here, you have three options you can configure.

1. Event Enabled = If this is unchecked, notifications will not be delivered. You can change this at any time.
2. Message - This is the message to send to the user. You may add {User} in the message, which will be replaced with a mention of the user.
3. Channel - This is the channel you wish for greetings to be delivered in.

Once configured, click "Save" and you are finished.

### Configure using commands.

To configure greetings using commands, just enter:
    
    /set greeting channel:#welcome message:Hello {User}! 

You can optionally change the message OR the channel as well.

    /set greeting message: Welcome to the server {User}!
    /set greeting channel:#welcome

You can enable or disable the greetings using a single command as well.

    /enable greeting
    /disable greeting

## Farewell Messages

A farewell message, is a message Warbot will send to your desired channel when a user leaves your server.

You can enable and configure farewell notifications either via commands, or by the configuration website.

### Configure Using Configuration Site.

First, visit the [Warbot configuration site](https://warbot.dev/Config/Events) and navigate to "Configure Events".

Next, select "User Leaving" in the tab menu.

From here, you have three options you can configure.

1. Event Enabled = If this is unchecked, notifications will not be delivered. You can change this at any time.
2. Message - This is the message to send to the user. You may add {User} in the message, which will be replaced with the name of the user. We cannot mention the user, because, they no longer exist in your server.
3. Channel - This is the channel you wish for farewell notifications to be delivered in.

Once configured, click "Save" and you are finished.

### Configure using commands.

To configure farewells using commands, just enter:
    
    /set farewell channel:#welcome message:Goodbye {User}! 

You can optionally change the message OR the channel as well.

    /set farewell message: Welcome to the server {User}!
    /set farewell channel:#welcome

You can enable or disable the greetings using a single command as well.

    /enable farewell
    /disable farewell