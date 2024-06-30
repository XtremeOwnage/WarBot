using WarBot.Data;

namespace WarBot.Modules.Base;
public class WarBOTInteractionContext : ShardedInteractionContext
{
    private readonly IServiceProvider serviceProvider;

    public WarBOTInteractionContext(DiscordShardedClient client, SocketInteraction interaction, IServiceProvider serviceProvider) : base(client, interaction)
    {
        this.serviceProvider = serviceProvider;
    }


    #region Use Guild Logic / Use DbContext
    public async Task UseGuildLogicAsync(Func<GuildLogic, Task> Action
#if DEBUG
      , [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
#endif
        )
    {
        if (this.Guild is null)
            throw new NullReferenceException("GetLogic() is only usable within a guild context");

        using var scope = serviceProvider.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<WarDB>();
        using var logic = await GuildLogic.GetOrCreateAsync(this.Client, db, this.Guild
#if DEBUG
            , memberName, sourceFilePath, sourceLineNumber
#endif
            );

        await Action(logic);
    }

    public async Task<T> UseGuildLogicAsync<T>(Func<GuildLogic, Task<T>> Action
#if DEBUG
      , [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
    [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
    [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
#endif
        )
    {
        if (this.Guild is null)
            throw new NullReferenceException("GetLogic() is only usable within a guild context");

        using var scope = serviceProvider.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<WarDB>();
        using var logic = await GuildLogic.GetOrCreateAsync(this.Client, db, this.Guild
#if DEBUG
            , memberName, sourceFilePath, sourceLineNumber
#endif
            );

        return await Action(logic);
    }

    public async Task UseDBContextAsync(Func<WarDB, Task> Action)
    {
        using var scope = serviceProvider.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<WarDB>();
        await Action(db);
    }

    public async Task<T> UseDBContextAsync<T>(Func<WarDB, Task<T>> Action)
    {
        using var scope = serviceProvider.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<WarDB>();
        return await Action(db);
    }
    #endregion

}
