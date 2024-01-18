using FpisNovoAPI.Data;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Repository
{
    public class ZoneRepository : IZoneRepository
    {
        private readonly DataContext _dataContext;
        public ZoneRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public bool CreateZone(Zone zone)
        {
            _dataContext.Zones.Add(zone);
            return Save();
        }

        public bool DeleteZone(Zone zone)
        {
            _dataContext.Zones.Remove(zone);
            return Save();
        }


        public Zone GetZone(int zoneId)
        {
            return _dataContext.Zones.Where(z => z.ZoneId == zoneId).FirstOrDefault();
        }

        public ICollection<Zone> GetZones()
        {
            return _dataContext.Zones.ToList();        }

        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateZone(Zone zone)
        {
            _dataContext.Zones.Update(zone);
            return Save();
        }

        public bool ZoneExists(int zoneId)
        {
            return _dataContext.Zones.Any(z => z.ZoneId == zoneId);
        }
    }
}
