using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Models
{
    public class AvailableSeats
    {
        public int Count { get; set; }
        public int ZoneId { get; set; }
        public int ConcertId { get; set; }
        public Zone Zone { get; set; }
        public Concert Concert { get; set; }
    }
}
