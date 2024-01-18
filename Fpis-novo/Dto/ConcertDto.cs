using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Dto
{
    public class ConcertDto
    {
        public int ConcertId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
