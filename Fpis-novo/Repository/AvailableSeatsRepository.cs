using FpisNovoAPI.Data;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Repository
{
    public class AvailableSeatsRepository : IAvailableSeatsRepository
    {
        private readonly DataContext _dataContext;
        public AvailableSeatsRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public AvailableSeats GetAvailableSeats(int zoneId, int concertId)
        {
            return _dataContext.AvailableSeats.Where(a => a.ConcertId == concertId && a.ZoneId == zoneId).FirstOrDefault();
        }
        
        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }

       
    }
}
