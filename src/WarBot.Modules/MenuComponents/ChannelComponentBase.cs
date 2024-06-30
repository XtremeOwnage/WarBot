namespace WarBot.Modules.MenuComponents;


public abstract class ChannelComponentBase
{
    public bool Disabled { get; set; } = false;

    /// <summary>
    /// Reference to the message containing this component.
    /// </summary>
    public IUserMessage? Message { get; protected set; }

    public abstract Task Disable(SocketMessageComponent arg);

    public virtual Task Delete() => Message?.DeleteAsync() ?? Task.CompletedTask;

    /// <summary>
    /// Processes incoming message with arguements.
    /// </summary>
    /// <param name="msg"></param>
    /// <returns><see cref="bool"/> indicating if this message was handled.</returns>
    public abstract Task<bool> ProcessMessage(SocketMessageComponent msg);

    /// <summary>
    /// Returns <see cref="bool"/> indicating if provided <paramref name="CustomID"/> is applicable to this component.
    /// </summary>
    /// <param name="CustomID"></param>
    /// <returns></returns>
    public abstract bool ContainsID(string CustomID);
}
