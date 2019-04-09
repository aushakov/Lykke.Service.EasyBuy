using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Client.Models.Balances;
using Lykke.Service.EasyBuy.Client.Models.Instruments;
using Lykke.Service.EasyBuy.Client.Models.Orders;
using Lykke.Service.EasyBuy.Client.Models.Prices;
using Lykke.Service.EasyBuy.Client.Models.Settings;
using Lykke.Service.EasyBuy.Client.Models.Trades;
using Lykke.Service.EasyBuy.Domain.Entities.Balances;
using Lykke.Service.EasyBuy.Domain.Entities.Instruments;
using Lykke.Service.EasyBuy.Domain.Entities.Orders;
using Lykke.Service.EasyBuy.Domain.Entities.Prices;
using Lykke.Service.EasyBuy.Domain.Entities.Settings;
using Lykke.Service.EasyBuy.Domain.Entities.Trades;

namespace Lykke.Service.EasyBuy
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Balance, BalanceModel>(MemberList.Source);

            CreateMap<InstrumentSettings, InstrumentSettingsModel>(MemberList.Source);
            CreateMap<InstrumentSettingsModel, InstrumentSettings>(MemberList.Destination);

            CreateMap<Order, OrderModel>(MemberList.Source)
                .ForMember(o => o.Volume, opt => opt.MapFrom(x => x.BaseVolume))
                .ForSourceMember(src => src.QuotingVolume, opt => opt.DoNotValidate())
                .ForSourceMember(src => src.ReserveTransferId, opt => opt.DoNotValidate())
                .ForSourceMember(src => src.SettlementTransferId, opt => opt.DoNotValidate());

            CreateMap<Price, PriceModel>(MemberList.Source)
                .ForSourceMember(src => src.AllowedOverlap, opt => opt.DoNotValidate());

            CreateMap<TimersSettings, TimersSettingsModel>(MemberList.Source);
            CreateMap<TimersSettingsModel, TimersSettings>(MemberList.Destination);

            CreateMap<Trade, TradeModel>(MemberList.Source);
        }
    }
}
