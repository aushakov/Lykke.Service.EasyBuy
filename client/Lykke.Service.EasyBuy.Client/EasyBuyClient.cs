using Lykke.HttpClientGenerator;

namespace Lykke.Service.EasyBuy.Client
{
    /// <inheritdoc/>
    public class EasyBuyClient : IEasyBuyClient
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EasyBuyClient"/> with <param name="httpClientGenerator"></param>.
        /// </summary> 
        public EasyBuyClient(IHttpClientGenerator httpClientGenerator)
        {
        }
    }
}
