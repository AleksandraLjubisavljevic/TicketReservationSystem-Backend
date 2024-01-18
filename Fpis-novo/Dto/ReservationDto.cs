using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Dto
{
    public class ReservationDto
    {
        public int ReservationId { get; set; }
        public DateTime ReservationDate { get; set; }
        public double TotalPrice { get; set; }
        public string Token { get; set; }
        public int CustomerId { get; set; }
        public int ConcertId { get; set; }
        public int PromocodeId { get; set; }
        public ICollection<ReservationTicketDto> Tickets { get; set; }
        public int? UsedPromocodeId { get; set; }
        public Promocode? UsedPromocode { get; set; }
        


    }
}
