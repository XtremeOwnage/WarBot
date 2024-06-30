namespace WarBot.DataAccess.Logic.Events;
public class GuildChannelEventLogic : BaseGuildChannelEventLogic<GuildChannelEvent>
{
    internal GuildChannelEventLogic(GuildLogic parentLogic, GuildChannelEvent ChannelEvent) : base(parentLogic, ChannelEvent) { }
}
