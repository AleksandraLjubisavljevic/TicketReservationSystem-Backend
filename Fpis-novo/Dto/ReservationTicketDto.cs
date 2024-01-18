using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Dto
{
    public class ReservationTicketDto
    {
        public int ReservationTicketId { get; set; }
        public int Quantity { get; set; }
        public int ZoneId { get; set; }
    }
}
