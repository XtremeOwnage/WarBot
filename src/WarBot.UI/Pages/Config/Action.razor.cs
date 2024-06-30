namespace WarBot.UI.Pages.Config;
public partial class Action
{
    [Parameter]
    public long CommandID { get; set; }

    [Parameter]
    public long? ActionID { get; set; }

    [CascadingParameter]
    public GuildLogic? Logic { get; set; }

    [Inject]
    NavigationManager navigationManager { get; set; }

    private CustomActionLogic Data { get; set; }

    [Inject]
    public ILogger<Commands> log { get; set; }

    private Task AfterSaveAsync()
    {
        return Task.Run(() => navigationManager.NavigateTo($"/Config/Command/{CommandID}"));
    }

    protected async override Task OnParametersSetAsync()
    {
        if (Data is null)
        {
            var Command = await Logic.GetSlashCommandAsync(CommandID);
            if (Command is null)
                navigationManager.NavigateTo("/Config/Commands");

            if (ActionID.HasValue)
            {
                Data = await Command.GetActionAsync(ActionID.Value);
                if (Data is null)
                {
                    navigationManager.NavigateTo("/Config/Commands");
                    return;
                }
            }
            else
            {
                //New Action
                Data = await Command.CreateActionAsync();
            }

            StateHasChanged();
        }
    }
}
