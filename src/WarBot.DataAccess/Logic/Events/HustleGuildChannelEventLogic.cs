using FluentValidation;

namespace WarBot.DataAccess.Logic.Events;
public class HustleGuildChannelEventLogic : BaseGuildChannelEventLogic<HustleGuildChannelEvent>
{
    internal HustleGuildChannelEventLogic(GuildLogic parentLogic, HustleGuildChannelEvent hustleEventSettings) : base(parentLogic, hustleEventSettings) { }

    public string? Prep_Started_Message
    {
        get => entity?.Prep_Started_Message;
        set => entity.Prep_Started_Message = value;
    }
    public string? Prep_Ending_Message
    {
        get => entity.Prep_Ending_Message;
        set => entity.Prep_Ending_Message = value;
    }
    public string? Event_Started_Message
    {
        get => entity.Event_Started_Message;
        set => entity.Event_Started_Message = value;
    }
    public string? Event_Finished_Message
    {
        get => entity.Event_Finished_Message;
        set => entity.Event_Finished_Message = value;
    }

    public override void ClearSettings()
    {
        Enabled = false;
        Channel.SetChannel(null);
        Prep_Started_Message = null;
        Prep_Ending_Message = null;
        Event_Started_Message = null;
        Event_Finished_Message = null;
    }

    public class CustomActionLogicValidator : AbstractValidator<HustleGuildChannelEventLogic>
    {
        public CustomActionLogicValidator()
        {
            When(o => o.Enabled, () =>
            {
                RuleFor(o => o.Channel.ChannelID)
                    .NotNull()
                    .WithMessage("A valid channel is required.");

                When(o => o.Event_Enabled, () =>
                {
                    RuleFor(o => o.Event_Title)
                        .NotEmpty()
                        .MinimumLength(4)
                        .WithMessage("A title is required for enabling discord events.");

                    RuleFor(o => o.Event_Description)
                        .NotEmpty()
                        .MinimumLength(4)
                        .WithMessage("A description is required for enabling discord events.");
                });

            });
        }
    }
}
