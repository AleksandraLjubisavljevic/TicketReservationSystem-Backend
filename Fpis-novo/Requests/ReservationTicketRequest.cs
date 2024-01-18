using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Requests
{
    public class ReservationTicketRequest
    {
        public Reservation reservation;
        public ReservationTicket[] tickets;
        public Zone zone;
    }
}
