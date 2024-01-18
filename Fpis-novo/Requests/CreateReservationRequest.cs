using FpisNovoAPI.Dto;
using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Requests
{
    public class CreateReservationRequest
    {
        public int ConcertId { get; set; }
        public DateTime ReservationDate { get; set; }
        public ICollection<ReservationTicketDto> Tickets { get; set; }
        public int? UsedPromocodeId { get; set; }
        public CustomerDto Customer { get; set; }
        public double TotalPrice { get; set; }
    }

}
