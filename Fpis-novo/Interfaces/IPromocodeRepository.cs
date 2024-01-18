using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Interfaces
{
    public interface IPromocodeRepository
    {
        ICollection<Promocode> GetPromocodes();
        Promocode GetPromocode(int? promocodeId);
        Promocode GetPromocodeByCode(string code);
        Promocode GetPromocodeByReservationCreatedId(int? reservationId);
        Promocode GetPromocodeByReservationUsedId(int? reservationId);
        Promocode GetPromocodeByReservationId(int? reservationId);
        bool IsUsed(int promocodeId);
        bool PromocodeExists(int promocodeId);
        bool CreatePromocode(Promocode promocode);
        bool UpdatePromocode(Promocode promocode);
        bool DeletePromocode(Promocode promocode);
        bool Save();
    }
}
