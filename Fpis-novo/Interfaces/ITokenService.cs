using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Interfaces
{
    public interface ITokenService
    {
         //string GenerateReservationToken(int reservationId, int userTokenVersion);
        string GenerateReservationToken(int reservationId);
    }
}
