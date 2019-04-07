using System;
using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Client.Models.Settings;

namespace Lykke.Service.EasyBuy.Validators
{
    [UsedImplicitly]
    public class TimersSettingsModelValidator : AbstractValidator<TimersSettingsModel>
    {
        public TimersSettingsModelValidator()
        {
            RuleFor(o => o.Orders)
                .GreaterThanOrEqualTo(TimeSpan.FromSeconds(1))
                .WithMessage("Orders timer interval should be greater than or equal to one second.");
        }
    }
}
