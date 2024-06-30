---
title: Role Management
description: Documentation warbot's role management features
---

# Role Management

Get tired of manually adding and removing roles from users? Well, no fear. Warbot has your back.

## Basics of role management.

Warbot maintains 7 different role levels. These different roles, will also grant more or less access within Warbot. Each role must be tied to a discord role for it to become active.

### Role Levels
1. None 
    * A user who does not have any other role.
2. Guest
    * If you choose to have a guest role, this is it.
3. Member
    * Generic member role.
4. Supermember
5. Officer
    * This role will grant some basic management commands
6. Leader
    * This role level will grant most management commands
7. Server Admin
    * This is a special role, not tied to discord roles. This role is automatically applied to anyone with admin level permissions in the discord server.

### How to configure roles

#### Method #1 - Via Configuration Site

The easiest way to configure roles, is by visiting the [Warbot configuration site](https://warbot.dev/Config/Roles)

![Role Management GUI](./../Assets/Role%20Management.png)

From here, click on the role you wish to manage, make your changes, and press Save.

If you wish to not use a particular role, set the "Discord Role" dropdown to "--Not Set--", and click save.

If you add a value into "Custom Name", Warbot will refer to this role as the value you specify. Feel free to customize your guild's role names.

#### Method #2 - Via Commands

Warbot has rudimentary commands for configuring roles. 

As a note, you are not able to set custom names using these commands. They are only used to associate a role with a discord role, or remove the role from your guild.

##### Set role

Using the `/set role` command, you can associate a warbot role to one of your discord guild's roles.

    /set role role-level:Member role:@Member

##### Clear Role

Clearing a role will remove it from being utilized by warbot, until you reconfigure it.

    /set role role-level:Member clear:True

##### Viewing current configuration

To view how roles are currently conofigured in your guild, use the command:

    /show config section:Roles

## How Warbot handles roles.

When warbot adds or removes a role, it includes the discord roles from all roles equal or less then the user's current role.

Lets say, you have this configuration in your server:

* Guest = @Guest
* Member = @Member
* Officer = @Officer
* Leader = @Leader

If you have "UserA", who has no roles.... 

If you promote him to "Leader", Warbot will add him to @Guest, @Member, @Officer and @Leader.

Likewise, if you demote him back to "None", all of those roles will be removed.

## Promoting and Demoting Users

!!! info
    Note- WarBOT can only add or remove users from roles, which are below his current role. If you run into permissions issues, you may need to move Warbot's role UP in the hierarchy.
!!! info
    Likewise, WarBOT cannot manage users higher then its configured hierarchy. So... You can't turn around and demote the guild's owner.

### Promote User

Promoting users is handled through either context commands, or by using slash commands.

##### Promote One Level

This will promote the user to the next CONFIGURED role. So, if you only have member, officer, and leader configured.... and the target user is a member, they will be promoted to officer.

    /promote user:@xtreme_test_user#5491 

##### Promote to specific role

You can optionally specify the target role you wish to promote a user to.

    /promote user:@xtreme_test_user#5491 role:Officer 

##### Promote Context Command

By clicking on a user, selecting apps, you will notice the option "Promote one level"

This.... will promote the user to the next configured level.

### Demote User

Like promotions, demotions are handled through either context commands, or by using slash commands.

##### Demote One Level

This will demote the user down one role Like promoting- if you only have member, officer, and leader configured.... and the target user is a officer, they will be demoeted to member.

Members can be demoted all the way down to "None"

    /demote user:@xtreme_test_user#5491 

##### Demote to specific role

You can optionally specify the target role you wish to demote a user to.

    /demote user:@xtreme_test_user#5491 role:None 

##### Demote Context Command

By clicking on a user, selecting apps, you will notice the option "Demote one level"

This.... will promote the user to the next configured level.

## Role Specific Capabilities

Some roles will gain access to more commands then others.

### Guest
* The ability to show `/loot` is unlocked at the guest role.

### Officer
* Gains the ability to show configuration items
    * `/portal config`
* Has the ability to promote and demote users BELOW the officer role.
    * Ie, they can promote somebody UP TO SuperMember.
    * They cannot promote someone to officer, or demote anyone higher then supermember.
* Does NOT have the ability to configure or change anything.

### Leader
* Will gain the ability to [Clear Messages](./../Features/ClearMessages.md)
* Ability to manage all common configurations via Commands. This includes, roles, channels, events, etc.
* Essentially, this role unlocks all commands from within discord. They will gain the ability to kick and ban users as well.

!!! warning
    Only promote people you trust to the leader role. This role will unlock a lot of responsability within WarBOT.
    
### Server Admin
* Has the ability to do anything within WarBOT, or its configuration site.
