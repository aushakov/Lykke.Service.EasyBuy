using System;

namespace Lykke.Service.EasyBuy.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
            : base("Entity not found.")
        {
        }
    }
}
