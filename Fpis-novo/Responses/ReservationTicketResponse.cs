using FpisNovoAPI.Dto;
using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Responses
{
    public class ReservationTicketResponse
    {
       public Reservation reservation { get; set; }
       public ICollection<ReservationTicket> tickets { get; set; }
        
    }
}
