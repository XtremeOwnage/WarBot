﻿namespace WarBot.Data.Models;
public enum CustomCommandActionType
{
    //New action - doesn't do anything yet.
    NEW_ACTION = 0,


    REPLY_WITH_MESSAGE = 1,
    BROADCAST_MESSAGE_TARGET_CHANNEL = 2,


    ADD_ROLE_CALLING_USER = 4,
    REMOVE_ROLE_CALLING_USER = 8,



    FLAGS_HAS_TARGET_ROLE = ADD_ROLE_CALLING_USER | REMOVE_ROLE_CALLING_USER,
    FLAGS_HAS_TARGET_CHANNEL = BROADCAST_MESSAGE_TARGET_CHANNEL,
    FLAGS_HAS_CUSTOM_MESSAGE = REPLY_WITH_MESSAGE | BROADCAST_MESSAGE_TARGET_CHANNEL,

}

