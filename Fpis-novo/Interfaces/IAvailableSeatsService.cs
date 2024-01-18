using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Interfaces
{
    public interface IAvailableSeatsService
    {
        bool AreEnoughSeatsAvailable(Reservation reservation);
        void ReduceAvailableSeats(Reservation reservation);
        void ReturnAvailableSeats(Reservation reservation);
        void ReduceAvailableSeatsForTicket(ReservationTicket ticket);
        void ReturnAvailableSeatsForTicket(ReservationTicket ticket);
        void UpdateAvailableSeatsForTicket(ReservationTicket existingTicket, ReservationTicket updatedTicket);
    }
}
