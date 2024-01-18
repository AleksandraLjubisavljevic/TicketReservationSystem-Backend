using FpisNovoAPI.Data;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Repository
{
    public class ReservationTicketRepository : IReservationTicketRepository
    {
        private readonly DataContext _dataContext;
        public ReservationTicketRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public bool CreateReservationTicket(ReservationTicket ticket)
        {
            _dataContext.ReservationTickets.Add(ticket);
            return Save();
        }

        public bool DeleteReservationTicket(ReservationTicket ticket)
        {
            _dataContext.ReservationTickets.Remove(ticket);
            return Save();
        }

        public ReservationTicket GetReservationTicket(int ticketId)
        {
            return _dataContext.ReservationTickets.Where(c => c.ReservationTicketId == ticketId).FirstOrDefault();
        }

        public ICollection<ReservationTicket> GetTickets()
        {
            return _dataContext.ReservationTickets.ToList();
        }
        public ICollection<ReservationTicket> GetTicketsForReservation(int reservationId)
        {

            return _dataContext.ReservationTickets.Where(c => c.ReservationId == reservationId).ToList();
        }

        public bool ReservationTicketExists(int ticketId)
        {
            return _dataContext.ReservationTickets.Any(c => c.ReservationTicketId == ticketId);
        }

        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateReservationTicket(ReservationTicket ticket)
        {
            _dataContext.ReservationTickets.Update(ticket);
            return Save();
        }
    }
}
