using System;
using JetBrains.Annotations;

namespace Lykke.Service.EasyBuy.Client.Models.Settings
{
    /// <summary>
    /// Represents a settings of timers.
    /// </summary>
    [PublicAPI]
    public class TimersSettingsModel
    {
        /// <summary>
        /// The timer interval of orders processing.
        /// </summary>
        public TimeSpan Orders { get; set; }
    }
}
