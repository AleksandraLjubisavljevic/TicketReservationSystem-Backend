using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Models
{
    public class ReservationTicket
    {
        public int ReservationTicketId { get; set; }
        public int Quantity { get; set; }
        public int ZoneId { get; set; }
        public int ReservationId { get; set; }
        public Zone ZoneData { get; set; }
        public Reservation Reservation { get; set; }
    }
}
