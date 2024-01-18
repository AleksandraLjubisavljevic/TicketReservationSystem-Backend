using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Helper
{
    public class NotEnoughSeatsException : Exception
    {
        public NotEnoughSeatsException()
        {
        }

        public NotEnoughSeatsException(string message) : base(message)
        {
        }

        public NotEnoughSeatsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
