using WarBot.Data;

namespace WarBot.Modules.Base;

/// <summary>
/// A base module to inherit everything from.
/// </summary>
public class WarBOTModule : InteractionModuleBase<WarBOTInteractionContext>
{
    bool hasResponded = false;

    protected override Task DeferAsync(bool UserOnly = false, RequestOptions options = null)
    {
        hasResponded = true;
        return base.DeferAsync(UserOnly);
    }

    protected override async Task RespondAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null, Embed embed = null)
    {
        if (!hasResponded)
        {
            await base.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, components, embed);
            hasResponded = true;
        }
        else
        {
            await Context.Interaction.ModifyOriginalResponseAsync(o =>
            {
                o.Content = text;
                o.Embed = embed;
                o.Embeds = embeds;
                o.AllowedMentions = allowedMentions;
                o.Components = components ?? new ComponentBuilder().Build();


                if (o.Flags.IsSpecified)
                    o.Flags = null;
                if (ephemeral)
                    o.Flags = new Optional<MessageFlags?>(MessageFlags.Ephemeral);
            });
        }
    }

    #region Use GuildLogic / DbContext
    public Task UseGuildLogicAsync(Func<GuildLogic, Task> Action
#if DEBUG
      , [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
#endif
        ) => Context.UseGuildLogicAsync(Action
#if DEBUG
            , memberName, sourceFilePath, sourceLineNumber
#endif
            );
    public Task<T> UseGuildLogicAsync<T>(Func<GuildLogic, Task<T>> Action
#if DEBUG
      , [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
    [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
    [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
#endif
        ) => Context.UseGuildLogicAsync(Action
#if DEBUG
            , memberName, sourceFilePath, sourceLineNumber
#endif
            );
    public Task UseDbContextAsync(Func<WarDB, Task> Action) => Context.UseDBContextAsync(Action);

    #endregion

    #region Interaction Utility
    public Task<bool> ConfirmAsync(TimeSpan timeout, string message = null, CancellationToken cancellationToken = default)
        => InteractionUtility.ConfirmAsync(this.Context.Client, this.Context.Channel, timeout, message, cancellationToken);
    //
    // Summary:
    //     Wait for an Interaction event for a given amount of time as an asynchronous opration.
    //
    // Parameters:
    //   client:
    //     Client that should be listened to for the Discord.WebSocket.BaseSocketClient.InteractionCreated
    //     event.
    //
    //   timeout:
    //     Timeout duration for this operation.
    //
    //   predicate:
    //     Delegate for cheking whether an Interaction meets the requirements.
    //
    //   cancellationToken:
    //     Token for canceling the wait operation.
    //
    // Returns:
    //     A Task representing the asyncronous waiting operation. If the user responded
    //     in the given amount of time, Task result contains the user response, otherwise
    //     the Task result is null.
    public Task<SocketInteraction> WaitForInteractionAsync(TimeSpan timeout, Predicate<SocketInteraction> predicate, CancellationToken cancellationToken = default)
        => InteractionUtility.WaitForInteractionAsync(this.Context.Client, timeout, predicate, cancellationToken);
    //
    // Summary:
    //     Wait for an Message Component Interaction event for a given amount of time as
    //     an asynchronous opration .
    //
    // Parameters:
    //   client:
    //     Client that should be listened to for the Discord.WebSocket.BaseSocketClient.InteractionCreated
    //     event.
    //
    //   fromMessage:
    //     The message that Discord.WebSocket.BaseSocketClient.ButtonExecuted or Discord.WebSocket.BaseSocketClient.SelectMenuExecuted
    //     should originate from.
    //
    //   timeout:
    //     Timeout duration for this operation.
    //
    //   cancellationToken:
    //     Token for canceling the wait operation.
    //
    // Returns:
    //     A Task representing the asyncronous waiting operation with a Discord.IDiscordInteraction
    //     result, the result is null if the process timed out before receiving a valid
    //     Interaction.
    public Task<SocketInteraction> WaitForMessageComponentAsync(IUserMessage fromMessage, TimeSpan timeout, CancellationToken cancellationToken = default)
        => InteractionUtility.WaitForMessageComponentAsync(this.Context.Client, fromMessage, timeout, cancellationToken);
    #endregion

    #region Random Interaction Helpers

    public async Task<SocketTextChannel?> PromptForTextChannel(string Message, IEnumerable<SocketTextChannel>? Choices = null, TimeSpan? Timeout = null, CancellationToken cancellationToken = default)
    {
        //Defer- querying could take a minute.
        await DeferAsync(true);

        SelectMenuBuilder builder = new SelectMenuBuilder()
            .WithPlaceholder("Please select a channel")
            .WithCustomId(Guid.NewGuid().ToString());

        if (Choices is null)
            Choices = Context.Guild.Channels.OfType<SocketTextChannel>();

        Choices = Choices.OrderBy(o => o.Name);

        foreach (var choice in Choices)
            builder.AddOption(choice.Name, choice.Id.ToString(), choice.Topic);

        ComponentBuilder compBuilder = new ComponentBuilder()
            .WithSelectMenu(builder);


        var msg = await FollowupAsync(Message, components: compBuilder.Build(), ephemeral: true);

        var SelectInteraction = await this.WaitForMessageComponentAsync(msg, Timeout ?? TimeSpan.FromMinutes(15), cancellationToken);

        var x = SelectInteraction as SocketMessageComponent;

        var channel = Choices.First(o => o.Id.ToString() == x.Data.Values.First());


        return channel;
    }


    #endregion

}

