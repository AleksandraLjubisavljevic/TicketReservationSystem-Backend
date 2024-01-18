using FpisNovoAPI.Dto;
using FpisNovoAPI.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Interfaces
{
    public interface IReservationRepository
    {
        bool ReservationExists(int reservationId);
        ICollection<Reservation> GetReservations();
        Reservation GetReservation(int reservationId);
        ICollection<Reservation> GetReservationsByCustomerId(int customerId);
        Reservation GetReservationByEmailAndToken(string email, string token);
        bool CreateReservation(Reservation reservation);
        bool UpdateReservation(Reservation reservation);
        bool DeleteReservation(Reservation reservation);
        bool Save();
        IDbContextTransaction BeginTransaction();

    }
}
