using FpisNovoAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Requests
{
    public class UpdateReservationRequest
    {
        public int ReservationId { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public ICollection<ReservationTicketDto> Tickets { get; set; }
        public double TotalPrice { get; set; }
    }
}
