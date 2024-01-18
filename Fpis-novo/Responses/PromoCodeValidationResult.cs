using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Responses
{
    public class PromoCodeValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        
    }
}
