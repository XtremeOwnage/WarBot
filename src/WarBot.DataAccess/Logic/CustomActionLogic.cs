using FluentValidation;
using WarBot.DataAccess.Logic.Base;

namespace WarBot.DataAccess.Logic;
public class CustomActionLogic : LogicBase<CustomCommandAction>
{
    public CustomCommandLogic Parent { get; init; }

    internal CustomActionLogic(CustomCommandLogic parentLogic, CustomCommandAction Entity) : base(parentLogic, Entity)
    {
        this.Parent = parentLogic;
    }

    public long ID => entity.ID;


    public string Description
    {
        get => Type switch
        {
            CustomCommandActionType.NEW_ACTION => "Unknown",
            CustomCommandActionType.REPLY_WITH_MESSAGE => "Reply with message",
            CustomCommandActionType.BROADCAST_MESSAGE_TARGET_CHANNEL => $"Send message to channel '{TargetChannel?.ChannelName ?? "Unknown"}'",
            CustomCommandActionType.ADD_ROLE_CALLING_USER => $"Add user to role '{TargetRole?.RoleName ?? "Unknown"}'",
            CustomCommandActionType.REMOVE_ROLE_CALLING_USER => $"Remove user from role '{TargetRole?.RoleName ?? "Unknown"}'",

            _ => "Unknown"
        };
    }

    public string? Message
    {
        get => entity.Message;
        set => entity.Message = value;
    }

    /// <summary>
    /// ItemId may refer to an ID of a user, role, channel, guild... etc... depending on <see cref="Type"/>
    /// </summary>
    public ulong? ItemId
    {
        get => entity.ItemId;
        set => entity.ItemId = value;
    }

    public CustomCommandActionType Type
    {
        get => entity.Type;
        set
        {
            //If the action type is changed, empty all of the related fields.
            if (entity.Type != value)
            {
                this.ItemId = null;
                this.TargetChannel.ClearSettings();
                this.Message = null;
            }
            entity.Type = value;

        }
    }

    public ulong? RoleID
    {
        get => TargetRole.RoleID;
    }

    public ChannelLogic TargetChannel
    {
        get
        {
            db.Entry(entity).Reference(o => o.TargetChannel).Load();
            return new(GuildLogic, entity.TargetChannel);
        }
    }

    public RoleLogic TargetRole
    {
        get
        {
            db.Entry(entity).Reference(o => o.TargetRole).Load();
            return new(GuildLogic, entity.TargetRole);
        }
    }

    /// <summary>
    /// Deletes this entity. 
    /// </summary>
    /// <returns></returns>
    public async Task DeleteAsync()
    {
        //Finally, delete this entity.
        db.Set<CustomCommandAction>().Remove(entity);
    }

    public override void ClearSettings()
    {
        //Clear settings involves all actions being removed.
    }

    public class CustomActionLogicValidator : AbstractValidator<CustomActionLogic>
    {
        public CustomActionLogicValidator()
        {
            RuleFor(o => o.Parent)
                .Must(o => o.Actions.Where(action => action.Type == CustomCommandActionType.REPLY_WITH_MESSAGE).Count() <= 1)
                .WithMessage("Only one action of type 'Reply' is allowed");

            RuleFor(o => o.Type)
                .NotEqual(CustomCommandActionType.NEW_ACTION)
                .WithMessage("Please select an action type.");

            When(o => CustomCommandActionType.FLAGS_HAS_CUSTOM_MESSAGE.HasFlag(o.Type), () =>
            {
                RuleFor(o => o.Message)
                    .NotEmpty()
                    .WithMessage("A message is required.");
            });

            When(o => CustomCommandActionType.FLAGS_HAS_TARGET_CHANNEL.HasFlag(o.Type), () =>
            {
                RuleFor(o => o.TargetChannel)
                    .Must(o => o.ChannelID.HasValue)
                    .WithMessage("A channel is required.");

                //Validate we can send messages to this channel.
                RuleFor(o => o.TargetChannel)
                    .MustAsync(async (o, ct) =>
                    {
                        Discord.ITextChannel ch = await o.GetChannelAsync();
                        var hasPerm = await ch.TestBotPermissionAsync(Discord.ChannelPermission.SendMessages);
                        return hasPerm;
                    })
                    .WithMessage("Warbot cannot write to the specified channel.");
            });

            When(o => CustomCommandActionType.FLAGS_HAS_TARGET_ROLE.HasFlag(o.Type), () =>
            {
                RuleFor(o => o.RoleID)
                    .NotNull()
                    .WithMessage("A target role is required.");
            });
        }
    }
}
