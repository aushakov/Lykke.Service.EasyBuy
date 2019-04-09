using System;

namespace Lykke.Service.EasyBuy.Domain.Entities.Settings
{
    /// <summary>
    /// Represents a settings of timers.
    /// </summary>
    public class TimersSettings
    {
        /// <summary>
        /// The timer interval of orders processing.
        /// </summary>
        public TimeSpan Orders { get; set; }
    }
}
