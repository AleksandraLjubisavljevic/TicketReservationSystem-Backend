using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Interfaces
{
    public interface IConcertRepository
    {
        ICollection<Concert> GetConcerts();
        Concert GetConcert(int concertId);
        bool ConcertExist(int concertId);
        bool CreateConcert(Concert concert);
        bool UpdateConcert(Concert concert);
        bool DeleteConcert(Concert concert);
        bool Save();
    }
}
