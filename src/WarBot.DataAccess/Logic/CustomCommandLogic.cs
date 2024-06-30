using FluentValidation;
using WarBot.Core;
using WarBot.DataAccess.Logic.Base;

namespace WarBot.DataAccess.Logic;
public class CustomCommandLogic : LogicBase<CustomSlashCommand>
{
    internal CustomCommandLogic(GuildLogic parentLogic, CustomSlashCommand Entity) : base(parentLogic, Entity) { }

    public long ID => entity.ID;

    public string Name
    {
        get => entity.Name;
        set => entity.Name = value;
    }

    public string Description
    {
        get => entity.Description;
        set => entity.Description = value;
    }

    public bool Enabled
    {
        get => entity.PublishSlashCommand;
        set => entity.PublishSlashCommand = value;
    }

    public RoleLevel? MinimumRoleLevel
    {
        get => entity.MinimumRoleLevel ?? RoleLevel.None;
        set => entity.MinimumRoleLevel = value;
    }

    public async Task DeleteAsync()
    {
        //Load the related entities.
        await db.Entry(entity).Collection(o => o.Actions).LoadAsync();

        //If this record has any related actions, delete those.
        if (entity.Actions.Any())
            db.Set<CustomCommandAction>().RemoveRange(entity.Actions);

        //Finally, delete this entity.
        db.Set<CustomSlashCommand>().Remove(entity);
    }

    #region Actions
    public async Task<CustomActionLogic> CreateActionAsync()
    {
        var newAction = await db.Set<CustomCommandAction>().AddAsync(new CustomCommandAction
        {
            Name = String.Empty,
            Parent = entity,
            Type = CustomCommandActionType.NEW_ACTION
        });

        return new CustomActionLogic(this, newAction.Entity);
    }

    public List<CustomActionLogic> Actions
    {
        get
        {
            //Load the related entities.
            db.Entry(entity).Collection(o => o.Actions).Load();

            //Return a list.
            return entity.Actions
                .ToList()
                .Select(o => new CustomActionLogic(this, o))
                .ToList();
        }
    }
    public async Task<CustomActionLogic?> GetActionAsync(long ID)
    {
        //Load the related entities.
        await db.Entry(entity).Collection(o => o.Actions).LoadAsync();

        //Return a list.
        var matching = entity.Actions.FirstOrDefault(o => o.ID == ID);
        if (matching is null)
            return null;

        return new(this, matching);
    }
    #endregion


    public override void ClearSettings()
    {
        entity.Actions.Clear();
        entity.Name = String.Empty;
        entity.Description = String.Empty;
        entity.PublishSlashCommand = false;
        entity.MinimumRoleLevel = null;
    }

    public class CustomCommandLogicValidator : AbstractValidator<CustomCommandLogic>
    {
        public CustomCommandLogicValidator()
        {
            RuleFor(o => o.Name)
                .NotEmpty()
                .WithMessage("You must specify a command name.");

            RuleFor(o => o.Name)
                .Matches(@"^[a-z\d\-]{3,32}$")
                .WithMessage("Command name must be between 3 and 32 characters long, and all lower space. The only allowed symbol is '-'");

            RuleFor(o => o.Description)
                .MinimumLength(5)
                .WithMessage("Please add a description. It will help you one day.");

            RuleFor(o => o.GuildLogic)
                .Must(o => !o.CustomCommands.GroupBy(l => l.Name).Any(l => l.Count() > 1))
                .WithMessage("Command names must be unique per guild.");

            RuleFor(o => o.GuildLogic)
                .Must(o => o.CustomCommands.Count <= 20)
                .WithMessage("You are limited to 20 custom commands.");
        }
    }
}
