using System.Threading.Tasks;

namespace Lykke.Service.EasyBuy.Domain.Services
{
    public interface IPricesPublisher
    {
        Task Publish(Price price);

        void Start();

        void Stop();
    }
}
