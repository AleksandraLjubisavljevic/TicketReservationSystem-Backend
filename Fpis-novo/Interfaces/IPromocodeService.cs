using FpisNovoAPI.Models;
using FpisNovoAPI.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Interfaces
{
    public interface IPromocodeService
    {
        PromoCodeValidationResult ValidatePromoCode(int? promoCodeId);
        string GeneratePromoCode(Reservation reservation);
        int CreatePromoCode(string generatedPromoCode, Reservation reservation);
        bool MarkPromoCodeAsUsed(int promocodeId, int reservationId);
    }
}
