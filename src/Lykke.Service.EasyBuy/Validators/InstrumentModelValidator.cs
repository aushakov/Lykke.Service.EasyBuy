using System;
using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Client.Models;
using Lykke.Service.EasyBuy.Domain.Services;

namespace Lykke.Service.EasyBuy.Validators
{
    [UsedImplicitly]
    public class InstrumentModelValidator : AbstractValidator<InstrumentModel>
    {
        public InstrumentModelValidator(ISettingsService settingsService)
        {
            var defaultSettings = settingsService.GetDefaultSettingsAsync().GetAwaiter().GetResult();
            
            RuleFor(o => o.AssetPair)
                .NotEmpty()
                .WithMessage("Asset pair required.");
            
            RuleFor(o => o.Exchange)
                .NotEmpty()
                .WithMessage("Exchange required.");
            
            RuleFor(o => o.State)
                .Must(o => o != InstrumentState.None)
                .WithMessage("Instrument state required.");
            
            RuleFor(o => o.OverlapTime)
                .Must(o => !o.HasValue || o.Value >= TimeSpan.Zero)
                .WithMessage("Allowed overlap can not be negative.");
            
            RuleFor(o => o.Markup)
                .Must(o => !o.HasValue || o.Value < 1m && o.Value > 0m)
                .WithMessage("Markup has to be between 0 and 1 (non inclusive).");

            RuleFor(o => o.Volume)
                .GreaterThan(0m)
                .WithMessage("Volume has to be positive number.");

            RuleFor(o => o.RecalculationInterval)
                .Must((model, b) =>
                    ((!b.HasValue && defaultSettings.RecalculationInterval < (model.PriceLifetime ?? defaultSettings.PriceLifetime)) ||
                    (b.HasValue && b.Value < (model.PriceLifetime ?? defaultSettings.PriceLifetime))))
                .WithMessage("Invalid Recalculation Interval.");
        }
    }
}
