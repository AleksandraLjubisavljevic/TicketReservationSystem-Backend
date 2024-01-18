using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Interfaces
{
    public interface IAvailableSeatsRepository 
    {
        AvailableSeats GetAvailableSeats(int zoneId, int concertId);
        bool Save();
    }
}
