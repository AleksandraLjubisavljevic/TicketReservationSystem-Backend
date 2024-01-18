using FpisNovoAPI.Data;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Repository
{
    public class PromocodeRepository : IPromocodeRepository
    {
        private readonly DataContext _dataContext;
        public PromocodeRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public bool CreatePromocode(Promocode promocode)
        {
            _dataContext.Promocodes.Add(promocode);
            return Save();
        }

        public bool DeletePromocode(Promocode promocode)
        {
            _dataContext.Promocodes.Remove(promocode);
            return Save();
        }
    

        public Promocode GetPromocode(int? promocodeId)
        {
            return _dataContext.Promocodes.Where(p => p.PromocodeId == promocodeId).FirstOrDefault();
        }

        public Promocode GetPromocodeByCode(string code)
        {
            return _dataContext.Promocodes.Where(p => p.Code == code).FirstOrDefault();
        }

        public Promocode GetPromocodeByReservationCreatedId(int? reservationId)
        {
            return _dataContext.Promocodes.Where(p => p.UsedReservationId == reservationId).FirstOrDefault();
        }
        public Promocode GetPromocodeByReservationId(int? reservationId)
        {
            return _dataContext.Promocodes.Where(p => p.ReservationId == reservationId).FirstOrDefault();
        }
        public Promocode GetPromocodeByReservationUsedId(int? reservationId)
        {
            return _dataContext.Promocodes.Where(p => p.PromocodeId == reservationId).FirstOrDefault();
        }

        public ICollection<Promocode> GetPromocodes()
        {
            return _dataContext.Promocodes.ToList();
        }

        public bool IsUsed(int promocodeId)
        {
            return _dataContext.Promocodes.Any(p=>p.PromocodeId==promocodeId);
        }
        public bool PromocodeExists(int promocodeId)
        {
            return _dataContext.Promocodes.Any(p => p.PromocodeId == promocodeId);
        }

        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdatePromocode(Promocode promocode)
        {
            _dataContext.Promocodes.Update(promocode);
            return Save();
        }
    
    }
}
