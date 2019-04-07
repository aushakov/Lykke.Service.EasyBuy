using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.AzureRepositories.Instruments;
using Lykke.Service.EasyBuy.AzureRepositories.Orders;
using Lykke.Service.EasyBuy.AzureRepositories.Prices;
using Lykke.Service.EasyBuy.AzureRepositories.Settings;
using Lykke.Service.EasyBuy.AzureRepositories.Trades;
using Lykke.Service.EasyBuy.Domain.Entities.Instruments;
using Lykke.Service.EasyBuy.Domain.Entities.Orders;
using Lykke.Service.EasyBuy.Domain.Entities.Prices;
using Lykke.Service.EasyBuy.Domain.Entities.Settings;
using Lykke.Service.EasyBuy.Domain.Entities.Trades;

namespace Lykke.Service.EasyBuy.AzureRepositories
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<InstrumentSettings, InstrumentSettingsEntity>(MemberList.Source);
            CreateMap<InstrumentSettingsEntity, InstrumentSettings>(MemberList.Destination);

            CreateMap<Order, OrderEntity>(MemberList.Source);
            CreateMap<OrderEntity, Order>(MemberList.Destination);

            CreateMap<Price, PriceEntity>(MemberList.Source);
            CreateMap<PriceEntity, Price>(MemberList.Destination);

            CreateMap<TimersSettings, TimersSettingsEntity>(MemberList.Source);
            CreateMap<TimersSettingsEntity, TimersSettings>(MemberList.Destination);

            CreateMap<Trade, TradeEntity>(MemberList.Source);
            CreateMap<TradeEntity, Trade>(MemberList.Destination);
        }
    }
}
