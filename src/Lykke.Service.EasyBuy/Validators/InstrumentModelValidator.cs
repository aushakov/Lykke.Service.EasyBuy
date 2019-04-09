using System;
using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Client.Models;
using Lykke.Service.EasyBuy.Client.Models.Instruments;
using Lykke.Service.EasyBuy.Domain.Services;

namespace Lykke.Service.EasyBuy.Validators
{
    [UsedImplicitly]
    public class InstrumentSettingsModelValidator : AbstractValidator<InstrumentSettingsModel>
    {
        public InstrumentSettingsModelValidator(ISettingsService settingsService)
        {
            RuleFor(o => o.AssetPair)
                .NotEmpty()
                .WithMessage("Asset pair required.");

            RuleFor(o => o.Exchange)
                .NotEmpty()
                .WithMessage("Exchange required.");

            RuleFor(o => o.PriceLifetime)
                .GreaterThan(TimeSpan.Zero)
                .WithMessage("Price lifetime should be greater than zero.");

            RuleFor(o => o.OverlapTime)
                .GreaterThanOrEqualTo(TimeSpan.Zero)
                .WithMessage("Overlap time should be greater than or equal to zero.");

            RuleFor(o => o.Markup)
                .InclusiveBetween(0, 1)
                .WithMessage("Markup should be between 0 and 1.");

            RuleFor(o => o.Volume)
                .GreaterThan(0m)
                .WithMessage("Volume should be greater than zero.");

            RuleFor(o => o.Status)
                .NotEqual(InstrumentStatus.None)
                .WithMessage("Instrument status required.");
        }
    }
}
