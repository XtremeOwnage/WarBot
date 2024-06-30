namespace WarBot.UI.Pages.Config;
public partial class Command
{
    [Parameter]
    public long? ID { get; set; }

    [CascadingParameter]
    public GuildLogic? Logic { get; set; }

    private CustomCommandLogic Data { get; set; }

    [Inject]
    public ILogger<Commands> log { get; set; }

    [Inject]
    NavigationManager navigationManager { get; set; }

    private List<CustomActionLogic> Actions { get; set; } = new();

    private void roleSelected(RoleLevel? NewRole)
    {
        Data.MinimumRoleLevel = NewRole;
    }


    private async Task DeleteAction(CustomActionLogic action)
    {
        await action.DeleteAsync();
        this.Actions.Remove(action);
    }

    private async Task OnSavedAsync()
    {
        if (!ID.HasValue)
            navigationManager.NavigateTo($"/Config/Command/{Data.ID}");
    }

    protected async override Task OnParametersSetAsync()
    {
        if (Data is null)
        {
            if (ID.HasValue)
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                Data = Logic.CustomCommands.FirstOrDefault(o => o.ID == ID.Value);
#pragma warning restore CS8601 // Possible null reference assignment.
                if (Data is null)
                    navigationManager.NavigateTo("/Config/Commands");
                else
                {
                    Actions = Data.Actions;
                }
            }
            else
            {
                Data = await Logic.CreateSlashCommandAsync();
                Actions = new();
            }

            StateHasChanged();
        }

    }
}