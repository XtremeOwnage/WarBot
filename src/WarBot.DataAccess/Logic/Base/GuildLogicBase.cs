using Discord;
using System.Linq.Expressions;

namespace WarBot.DataAccess.Logic.Base;
public abstract class GuildLogicBase
{
    protected internal WarDB db { get; init; }
    protected internal IDiscordClient discord { get; }
    protected GuildSettings entity { get; set; }
    public IGuild guild { get; set; }

    internal GuildLogicBase(WarDB db, IGuild Guild, IDiscordClient client, GuildSettings rootEntity)
    {
        this.db = db;
        this.discord = client;
        this.guild = Guild;
        this.entity = rootEntity;
    }

    public virtual async Task SaveChangesAsync()
    {
        await db.SaveChangesAsync();
    }

    public TLogic LoadRelatedLogic<TProperty, TLogic>(Expression<Func<GuildSettings, TProperty?>> propertyExpression, Func<TProperty, TLogic> Constructor)
    where TProperty : class
    {
        db.Entry(entity).Reference(propertyExpression).Load();

        var Property = propertyExpression.Compile().Invoke(entity);

        return Constructor(Property);
    }

    /// <summary>
    /// Clear all settings and revert to defaults.
    /// </summary>
    public abstract void ClearSettings();
}
