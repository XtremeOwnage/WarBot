using Discord;
using WarBot.DataAccess.Logic.Base;

namespace WarBot.DataAccess.Logic;
public class ChannelLogic : LogicBase<GuildChannel>
{
    internal ChannelLogic(GuildLogic parentLogic, GuildChannel Entity) : base(parentLogic, Entity) { }

    public async Task<ITextChannel?> GetChannelAsync()
    {
        if (entity.DiscordID is null)
            return null;

        return await guild.GetChannelAsync(entity.DiscordID.Value) as ITextChannel;
    }
    public void SetChannel(ITextChannel? Channel)
    {
        if (entity is null)
            entity = new GuildChannel
            {
                DiscordID = Channel?.Id,
                DiscordName = Channel?.Name,
            };
        else
        {
            //Update the existing Object.
            entity.DiscordID = Channel?.Id;
            entity.DiscordName = Channel?.Name;
        }
    }

    public ulong? ChannelID => entity.DiscordID;
    public string? ChannelName => entity.DiscordName;

    /// <summary>
    /// Returns a mention of the channel if set. If not set, returns [NOT SET]
    /// </summary>
    public string GetMention => ChannelID.HasValue ? $"<#{ChannelID}>" : "[NOT SET]";
    public override void ClearSettings()
    {
        SetChannel(null);
    }
}
