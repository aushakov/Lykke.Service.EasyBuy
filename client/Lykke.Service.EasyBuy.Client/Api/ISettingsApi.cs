using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Client.Models.Settings;
using Refit;

namespace Lykke.Service.EasyBuy.Client.Api
{
    /// <summary>
    /// Provides methods to work with EasyBuy settings.
    /// </summary>
    [PublicAPI]
    public interface ISettingsApi
    {
        /// <summary>
        /// Returns account settings.
        /// </summary>
        /// <returns>Account settings.</returns>
        [Get("/api/Settings/account")]
        Task<AccountSettingsModel> GetAccountSettingsAsync();

        /// <summary>
        /// Returns the timers settings.
        /// </summary>
        /// <returns>The model that represents timers settings.</returns>
        [Get("/api/Settings/timers")]
        Task<TimersSettingsModel> GetTimersSettingsAsync();

        /// <summary>
        /// Updates the timers settings.
        /// </summary>
        /// <param name="model">The model that represents timers settings.</param>
        [Put("/api/Settings/timers")]
        Task UpdateTimersSettingsAsync([Body] TimersSettingsModel model);
    }
}
