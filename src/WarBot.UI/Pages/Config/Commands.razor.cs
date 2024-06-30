namespace WarBot.UI.Pages.Config;
public partial class Commands
{
    [CascadingParameter]
    public GuildLogic? Logic { get; set; }

    [Inject]
    public ILogger<Commands> log { get; set; }

    private List<CustomCommandLogic> Data { get; set; } = new List<CustomCommandLogic>();

    private bool hasChanges = false;

    private async Task Delete(CustomCommandLogic Cmd)
    {
        await Cmd.DeleteAsync();
        Data.Remove(Cmd);
        hasChanges = true;
        StateHasChanged();
    }

    private async Task Save()
    {
        try
        {
            await Logic.SaveChangesAsync();
            hasChanges = false;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error while saving changes");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.Data = Logic.CustomCommands.ToList();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}