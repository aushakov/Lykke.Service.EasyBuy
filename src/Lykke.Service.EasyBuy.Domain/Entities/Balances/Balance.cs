namespace Lykke.Service.EasyBuy.Domain.Entities.Balances
{
    public class Balance
    {
        public string AssetId { set; get; }
        
        public decimal Available { set; get; }
        
        public decimal Reserved { set; get; }
    }
}
