using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Dto
{
    public class AvailableSeatsDto
    {
        public int Count { get; set; }
        public int ZoneId { get; set; }
        public int ConcertId { get; set; }
    }
}
