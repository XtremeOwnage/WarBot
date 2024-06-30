---
title: Clear Messages
description: Documentation around /clear
---

# Clear Messages

Warbot has the ability to clear messages from a channel.

By default, executing `/clear` will remove all non-pinned messages from the target channel, for the last 2 weeks. 

2 weeks is a limitation in the [Discord API](https://discord.com/developers/docs/resources/channel#bulk-delete-messages)

### Deleting Pinned Messages

You may execute `/clear action:Pinned` to remove pinned messages as well.

## Context Command

There is also a context command which can be access by clicking on a message, selecting apps, and clicking "Clear Messages After" 

This command, will remove all messages AFTER the specified message.