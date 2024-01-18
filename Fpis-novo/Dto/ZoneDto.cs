using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Dto
{
    public class ZoneDto
    {
        public int ZoneId { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public double Price { get; set; }
    }
}
