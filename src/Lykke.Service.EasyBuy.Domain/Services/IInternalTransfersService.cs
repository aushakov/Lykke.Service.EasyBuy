using System.Threading.Tasks;

namespace Lykke.Service.EasyBuy.Domain.Services
{
    public interface IInternalTransfersService
    {
        Task TransferAsync(
            string transferId,
            string sourceWalletId,
            string destinationWalletId,
            string assetId,
            decimal amount);
    }
}
