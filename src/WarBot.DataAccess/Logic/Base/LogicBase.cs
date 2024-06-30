using Discord;
using System.Linq.Expressions;
using WarBot.Data.Interfaces;

namespace WarBot.DataAccess.Logic.Base;
public abstract class LogicBase
{
    public GuildLogic GuildLogic { get; set; }
    protected WarDB db => GuildLogic.db;
    protected IDiscordClient discord => GuildLogic.discord;
    protected IGuild guild => GuildLogic.guild;

    protected LogicBase(GuildLogic logic)
    {
        GuildLogic = logic;
    }

    /// <summary>
    /// Clear all settings and revert to defaults.
    /// </summary>
    public abstract void ClearSettings();
}
public abstract class LogicBase<TEntity> : LogicBase where TEntity : class
{
    protected TEntity entity { get; set; }

    protected LogicBase(GuildLogic logic, TEntity entity) : base(logic)
    {
        this.entity = entity;
    }
    protected LogicBase(LogicBase logic, TEntity entity) : base(logic.GuildLogic)
    {
        this.entity = entity;
    }

    protected TLogic LoadRelatedLogic<TProperty, TLogic>(Expression<Func<TEntity, TProperty?>> propertyExpression, Func<TProperty, TLogic> Constructor)
        where TProperty : class, IRecord
    {
        GuildLogic.db.Entry(entity).Reference(propertyExpression).Load();

        var Property = propertyExpression.Compile().Invoke(entity);

        if (Property.ID == default && GuildLogic.db.Entry(Property).State == EntityState.Detached)
        {
            GuildLogic.db.Entry(Property).State = EntityState.Added;

        }


        return Constructor(Property);
    }
}
