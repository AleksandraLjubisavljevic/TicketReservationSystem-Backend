using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Models
{
    public class Promocode
    {
        public int PromocodeId { get; set; }
        public string Code { get; set; }
        public bool IsUsed { get; set; }
        public int ReservationId { get; set; }
        public Reservation ReservationCreated { get; set; }
        public int? UsedReservationId { get; set; }
        public Reservation ReservationUsed { get; set; }
    }
}
