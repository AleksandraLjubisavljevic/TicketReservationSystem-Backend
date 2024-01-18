using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Interfaces
{
    public interface IReservationTicketRepository
    {
        ICollection<ReservationTicket> GetTickets();
        ICollection<ReservationTicket> GetTicketsForReservation(int reservationId);
        ReservationTicket GetReservationTicket(int ticketId);
        bool ReservationTicketExists(int ticketId);
        bool CreateReservationTicket(ReservationTicket ticket);
        bool UpdateReservationTicket(ReservationTicket ticket);
        bool DeleteReservationTicket(ReservationTicket ticket);
        bool Save();

    }
}
