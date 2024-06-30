using Discord;
using Discord.Rest;

namespace WarBot.UI.Components;
public partial class ChannelPicker
{
    [CascadingParameter]
    public Models.Discord.UserGuild? SelectedGuild { get; set; }

    [Inject]
    public DiscordObjectCache discordCache { get; set; }

    [Parameter]
    public Action<ITextChannel?> OnChannelSelected { get; set; }

    [Parameter]
    public ulong? SelectedChannelID { get; set; }

    public ITextChannel? GetSelectedChannel() => Channels.FirstOrDefault(o => o.Id == SelectedChannelID);

    public List<ITextChannel> Channels { get; set; } = new List<ITextChannel>();

    private void setChannel(ChangeEventArgs changeArgs)
    {
        if (ulong.TryParse(changeArgs.Value.ToString(), out ulong u))
        {
            var tch = Channels.FirstOrDefault(o => o.Id == u);
            this.SelectedChannelID = tch.Id;
            OnChannelSelected?.Invoke(tch);
            this.StateHasChanged();
        }
        else
        {
            this.SelectedChannelID = null;
            OnChannelSelected?.Invoke(null);
            this.StateHasChanged();
        }

    }
    private async Task refreshChannels()
    {
        this.Channels.Clear();
        this.StateHasChanged();
        this.Channels = await discordCache.RefreshChannels(SelectedGuild.ID_NUM.Value);
        this.StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.Channels = await discordCache.GetGuildChannels(SelectedGuild.ID_NUM.Value);
            this.StateHasChanged();
        }
    }
}
