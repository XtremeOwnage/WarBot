namespace WarBot.UI.Models;
public class EditButtonHelper
{
    public string Text { get; set; } = "Save";
    public string Class { get; set; } = "disabled";

    public string Class_Ready { get; init; } = "btn-success";
    public string Class_Saving { get; init; } = "btn-primary disabled";
    public string Class_Saved { get; init; } = "btn-success disabled";
    public string Class_Error { get; init; } = "btn-danger disabled";

    /// <summary>
    /// How long will "Saved" be displayed? If null, will go straight from Saving -> Ready.
    /// </summary>
    public TimeSpan? Time_Saved { get; init; } = TimeSpan.FromMilliseconds(500);

    public void Ready()
    {
        Class = Class_Ready;
    }
    public void Saving()
    {
        Class = Class_Saving;
        Text = "Saving... Please Wait";
    }

    public Task OnSavedAsync()
    {
        Class = Class_Saved;
        Text = "Saved";

        if (Time_Saved is null)
        {
            Class = Class_Ready;
            Text = "Save";
            return Task.CompletedTask;
        }
        return Task.Delay(Time_Saved.Value).ContinueWith((t) =>
        {
            Class = Class_Ready;
            Text = "Save";
        });
    }

    public void Error()
    {
        Class = Class_Error;
        Text = "Error. Please Reload.";
    }
}
