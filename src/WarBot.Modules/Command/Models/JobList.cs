namespace WarBot.Modules.Command.Models
{
    public enum JobList
    {
        expedition1_discord_event,
        expedition2_discord_event,
        expedition3_discord_event,
        expedition4_discord_event,

        expedition1_prep_started,
        expedition2_prep_started,
        expedition3_prep_started,
        expedition4_prep_started,

        expedition1_prep_ending,
        expedition2_prep_ending,
        expedition3_prep_ending,
        expedition4_prep_ending,

        expedition1_started,
        expedition2_started,
        expedition3_started,
        expedition4_started,

        war1_discord_event,
        war2_discord_event,
        war3_discord_event,
        war4_discord_event,

        war1_prep_started,
        war2_prep_started,
        war3_prep_started,
        war4_prep_started,

        war1_prep_ending,
        war2_prep_ending,
        war3_prep_ending,
        war4_prep_ending,

        war1_started,
        war2_started,
        war3_started,
        war4_started,
        portal_opened,

        //Updates the discord bot list api.
        update_dbl,

        //Updates bot's current status and game.
        update_status,

        //Register's commands to discord.
        discord_register_commands,

        //Check discord scopes
        discord_check_scopes
    }
}
