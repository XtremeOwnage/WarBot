namespace WarBot.Modules.MenuComponents;
internal class DialogInput
{
    public bool Skip { get; set; } = false;
    public bool Stop { get; set; } = false;
    public bool GoBack { get; set; } = false;

    public bool? Boolean { get; set; }
    public ITextChannel? TextChannel { get; set; }
    public IRole? Role { get; set; }
    public string? Message { get; set; }
}

