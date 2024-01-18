using FpisNovoAPI.Dto;
using FpisNovoAPI.Models;
using FpisNovoAPI.Requests;
using FpisNovoAPI.Responces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Interfaces
{
    public interface IReservationService
    {
        ReservationResponse CreateReservation(CreateReservationRequest reservationNew);
        Reservation GetReservationByEmailAndToken(string email, string token);
        ICollection<ReservationDto> GetReservationsWithTickets();
        bool UpdateReservation(UpdateReservationRequest reservationUpdate);
        bool DeleteReservation(int reservationId);
    }
}
