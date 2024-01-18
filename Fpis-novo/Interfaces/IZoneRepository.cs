using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Interfaces
{
    public interface IZoneRepository
    {
        ICollection<Zone> GetZones();
        Zone GetZone(int zoneId);
        bool ZoneExists(int zoneId);
        bool CreateZone(Zone zone);
        bool UpdateZone(Zone zone);
        bool DeleteZone(Zone zone);
        bool Save();
    }
}
