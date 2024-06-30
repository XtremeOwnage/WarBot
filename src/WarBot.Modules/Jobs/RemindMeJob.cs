namespace WarBot.Modules.Jobs;
/// <summary>
/// Note- This service must be registered with the IServiceProvider for it to be properly 
/// </summary>
public class RemindMeJob
{
    private readonly IDiscordClient client;

    public RemindMeJob(IDiscordClient client)
    {
        this.client = client;
    }

    public async Task SendReminder_DM(ulong UserId, string Message)
    {
        var User = await client.GetUserAsync(UserId);
        var Channel = await User.CreateDMChannelAsync();

        await Channel.SendMessageAsync($"{Message}");
    }
    public async Task SendReminder_GuildChannel_Me(ulong UserId, ulong ChannelId, string Message)
    {
        var User = await client.GetUserAsync(UserId);
        var Channel = await client.GetChannelAsync(ChannelId);
        var GuildChannel = Channel as SocketTextChannel;

        await GuildChannel.SendMessageAsync($"{User.Mention}, {Message}");
    }

    public async Task SendReminder_GuildChannel_Here(ulong ChannelId, string Message)
    {
        var Channel = await client.GetChannelAsync(ChannelId, CacheMode.AllowDownload);
        var GuildChannel = Channel as SocketTextChannel;

        await GuildChannel.SendMessageAsync(Message);
    }
}
