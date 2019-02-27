using System;

namespace Lykke.Service.EasyBuy.Domain.Exceptions
{
    public class FailedOperationException : Exception
    {
        public FailedOperationException(string message)
            : base(message)
        {
        }
        
        public FailedOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
