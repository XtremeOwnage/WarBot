using Discord;
using FluentValidation;
using WarBot.Core;
using WarBot.DataAccess.Logic.Base;
using WarBot.DataAccess.Logic.Events;

namespace WarBot.DataAccess.Logic;
public class GuildLogic : GuildLogicBase, IDisposable
{
    private GuildLogic(IDiscordClient client, WarDB Database, IGuild Guild, GuildSettings Entity) : base(Database, Guild, client, Entity) { }

    /// <summary>
    /// Creates instance of GuildLogic for the sepcified <paramref name="Guild"/>
    /// If Guild does not exist in the underlying datastore, it will be created automatically.
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="Guild"></param>
    /// <param name="NoTracking"></param>
    /// <returns></returns>
    public static async Task<GuildLogic> GetOrCreateAsync(IDiscordClient client, WarDB db, IGuild Guild
#if DEBUG
        , [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
#endif
        )
    {
#if DEBUG
        Console.WriteLine($"Creating GuildLogic For {memberName}, {sourceFilePath.Split('\\').LastOrDefault()} lineno: {sourceLineNumber}");
#endif

        var e = await db
            .GuildSettings
            .AsTracking()
            .FirstOrDefaultAsync(o => o.DiscordID == Guild.Id);

        if (e is not null)
            return new GuildLogic(client, db, Guild, e);
        else
        {
            var settings = new GuildSettings
            {
                DiscordID = Guild.Id,
                DiscordName = Guild.Name,
                BotVersion = BotConfig.BOT_VERSION
            };

            await db.AddAsync(settings);
            await db.SaveChangesAsync();
            return new GuildLogic(client, db, Guild, settings);
        }
    }


    /// <inheritdoc cref="GuildSettings.Website"/>
    public string? Website
    {
        get => entity.Website;
        set => entity.Website = value;
    }

    public TimeZoneInfo TimeZone
    {
        get
        {
            if (!string.IsNullOrEmpty(entity.TimeZone))
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(entity.TimeZone);
                }
                catch { }
            }

            return TimeZoneInfo.Utc;
        }
        set
        {
            entity.TimeZone = value.Id;
        }

    }

    /// <summary>
    /// Retreives buisness logic for utilizing hustle-castle specific guild settings.
    /// </summary>
    /// <returns></returns>
    public HustleSettingLogic HustleSettings => LoadRelatedLogic(o => o.HustleCastleSettings, o => new HustleSettingLogic(this, o));
    public GuildRolesLogic Roles => LoadRelatedLogic(o => o.Roles, o => new GuildRolesLogic(this, o));
    public GuildChannelEventLogic Event_UserJoin => LoadRelatedLogic(o => o.Event_UserJoin, o => new GuildChannelEventLogic(this, o));
    public GuildChannelEventLogic Event_UserLeft => LoadRelatedLogic(o => o.Event_UserLeft, o => new GuildChannelEventLogic(this, o));
    public ChannelLogic Channel_Admins => LoadRelatedLogic(o => o.Channel_Admins, o => new ChannelLogic(this, o));
    public GuildChannelEventLogic Event_Updates => LoadRelatedLogic(o => o.Event_Updates, o => new GuildChannelEventLogic(this, o));

    #region Custom Commands
    public IReadOnlyList<CustomCommandLogic> CustomCommands
    {
        get
        {
            db.Entry(entity).Collection(o => o.CustomCommands).Load();
            return entity.CustomCommands
                .ToList()
                .Select(o => new CustomCommandLogic(this, o))
                .ToList();
        }
    }

    public async Task<CustomCommandLogic> CreateSlashCommandAsync()
    {
        var newAction = await db.Set<CustomSlashCommand>().AddAsync(new CustomSlashCommand
        {
            Parent = entity
        });

        return new CustomCommandLogic(this, newAction.Entity);
    }

    public async Task<CustomCommandLogic?> GetSlashCommandAsync(string name)
    {
        //Load the related entities.
        await db.Entry(entity).Collection(o => o.CustomCommands).LoadAsync();

        var matching = entity.CustomCommands.FirstOrDefault(o => o.Name == name);
        if (matching == null)
            return null;

        return new CustomCommandLogic(this, matching);
    }

    public async Task<CustomCommandLogic?> GetSlashCommandAsync(long ID)
    {
        //Load the related entities.
        await db.Entry(entity).Collection(o => o.CustomCommands).LoadAsync();

        var matching = entity.CustomCommands.FirstOrDefault(o => o.ID == ID);
        if (matching == null)
            return null;

        return new CustomCommandLogic(this, matching);
    }

    #endregion


    /// <summary>
    /// Resets ALL configured settings to defaults.
    /// </summary>
    public override void ClearSettings()
    {
        this.Website = null;

        this.HustleSettings.ClearSettings();
        this.Roles.ClearSettings();
        this.Event_UserJoin.ClearSettings();
        this.Event_UserLeft.ClearSettings();
        this.Channel_Admins.ClearSettings();
        this.Event_Updates.ClearSettings();

        entity.CustomCommands.Clear();
    }

    public void Dispose()
    {
        Console.WriteLine("Disposing of GuildLogic");
        this.db.Dispose();
    }

    public class GuildLogicValidator : AbstractValidator<GuildLogic>
    {
        public GuildLogicValidator(IServiceProvider sp)
        {
            RuleFor(o => o.CustomCommands)
                .Must(o => !o.GroupBy(o => o.Name).Any(o => o.Count() > 1))
                .WithMessage("Command names must be unique per guild.");

            RuleFor(o => o.CustomCommands)
                .Must(o => o.Count <= 20)
                .WithMessage("You are not allowed more then 20 custom commands.");

            RuleForEach(o => o.CustomCommands)
                .SetInheritanceValidator(o =>
                {
                    o.Add(new CustomCommandLogic.CustomCommandLogicValidator());
                });
        }
    }
}
