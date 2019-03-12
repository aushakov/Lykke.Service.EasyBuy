using System.Threading.Tasks;

namespace Lykke.Service.EasyBuy.Domain.Services
{
    public interface IPricesGenerator
    {
        Task StartActives();
        Task Start(string assetPair);
        Task StopAll();
        Task Stop(string assetPair);
    }
}
