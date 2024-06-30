using Discord;

namespace WarBot.UI.Components;
public partial class TagHelper
{
    private bool Show { get; set; } = false;

    private string _value;

    private string mention;

    [Parameter]
    public string Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            ValueChanged.InvokeAsync(value);
        }
    }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public string Class { get; set; } = string.Empty;

    public void Toggle()
    {
        Show = !Show;
        StateHasChanged();
    }

    public void AddMention()
    {
        if (!string.IsNullOrEmpty(mention))
            Value += mention;

        Show = false;
        StateHasChanged();
    }
  
}
