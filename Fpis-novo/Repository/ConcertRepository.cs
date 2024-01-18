using FpisNovoAPI.Data;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Repository
{
    public class ConcertRepository : IConcertRepository
    {
        private readonly DataContext _context;
        
        public ConcertRepository(DataContext context)
        {
            _context = context;
            
        }
        public bool ConcertExist(int concertId)
        {
            return _context.Concerts.Any(c => c.ConcertId == concertId);
        }

       
        public bool CreateConcert(Concert concert)
        {
            _context.Concerts.Add(concert);
            return Save();
        }

        public bool DeleteConcert(Concert concert)
        {
            _context.Concerts.Remove(concert);
            return Save();
        }


        public Concert GetConcert(int concertId)
        {
            return _context.Concerts.Where(c => c.ConcertId == concertId).FirstOrDefault();
        }

        public ICollection<Concert> GetConcerts()
        {
            return _context.Concerts.ToList();
        }


        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
        
        public bool UpdateConcert(Concert concert)
        {
            _context.Concerts.Update(concert);
            return Save();
        }
        
    }
}
