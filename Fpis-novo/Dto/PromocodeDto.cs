using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Dto
{
    public class PromocodeDto
    {
        public int PromocodeId { get; set; }
        public string Code { get; set; }
        public bool IsUsed { get; set; }
    }
}
