using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Models
{
    public class Zone
    {
        public int ZoneId { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public double Price { get; set; }
        public ICollection<ReservationTicket> Tickets { get; set; }
        public ICollection<AvailableSeats> AvailableSeats { get; set; }
    }
}
