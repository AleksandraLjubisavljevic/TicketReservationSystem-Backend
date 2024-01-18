using FpisNovoAPI.Helper;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Services
{
    public class AvailableSeatsService : IAvailableSeatsService
    {
        private readonly IAvailableSeatsRepository _availableSeatsRepository;
        private readonly IReservationTicketRepository _reservationTicketRepository;
        private readonly ILogger<AvailableSeatsService> _logger;
        public AvailableSeatsService(IAvailableSeatsRepository availableSeatsRepository,
                                    IReservationTicketRepository reservationTicketRepository,
                                    ILogger<AvailableSeatsService> logger)
        {
            _availableSeatsRepository = availableSeatsRepository;
            _reservationTicketRepository = reservationTicketRepository;
            _logger = logger;
        }
        public bool AreEnoughSeatsAvailable(Reservation reservation)
        {
            try
            {
                if (reservation == null || reservation.Tickets == null)
                {
                    _logger.LogError("Reservation or its tickets are null.");
                    return false;
                }
                var tickets = reservation.Tickets;

                foreach (ReservationTicket ticket in tickets)
                {
                    var availableSeats = _availableSeatsRepository.GetAvailableSeats(ticket.ZoneId, reservation.ConcertId);
                    if (availableSeats == null || availableSeats.Count < ticket.Quantity)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occurred while checking seats availability: {ex.Message}");
                return false;
            }
        }

        public void ReduceAvailableSeats(Reservation reservation)
        {
            try
            {
                var tickets = reservation.Tickets;
                if (reservation == null || reservation.Tickets == null)
                {
                    _logger.LogError("Reservation or its tickets are null.");
                    return;
                }
                foreach (ReservationTicket ticket in tickets)
                {
                    var availableSeats = _availableSeatsRepository.GetAvailableSeats(ticket.ZoneId, reservation.ConcertId);
                    if (availableSeats != null && availableSeats.Count >= ticket.Quantity)
                    {
                        availableSeats.Count -= ticket.Quantity;
                        _availableSeatsRepository.Save();
                    }
                    else
                    {
                        throw new NotEnoughSeatsException($"Nema dovoljno mesta za ovu zonu {ticket.ZoneId}.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while checking seats availability: {ex.Message}");
              
            }
        }
        public void ReturnAvailableSeats(Reservation reservation)
        {
            try
            {

                var tickets = _reservationTicketRepository
                    .GetTicketsForReservation(reservation.ReservationId);
                /*if (reservation == null || tickets == null)
                {
                    _logger.LogError("Reservation or its tickets are null.");
                    return;
                }*/
                //var tickets = reservation.Tickets;
                foreach (ReservationTicket ticket in tickets)
                {
                    var availableSeats = _availableSeatsRepository.GetAvailableSeats(ticket.ZoneId, reservation.ConcertId);
                    if (availableSeats != null)
                    {
                        availableSeats.Count += ticket.Quantity;
                        if (_availableSeatsRepository.Save())
                        {
                            _logger.LogInformation("Available seats updated successfully");
                        }
                        else
                        {

                            _logger.LogError("Error while updating available seats");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while checking seats availability: {ex.Message}");
            }
        }

        public void ReduceAvailableSeatsForTicket(ReservationTicket ticket)
        {
            try
            {
                var availableSeats = _availableSeatsRepository.GetAvailableSeats(ticket.ZoneId, ticket.Reservation.ConcertId);
                if (availableSeats != null)
                {
                    availableSeats.Count -= ticket.Quantity;
                    _availableSeatsRepository.Save();
                }
                else
                {
                    throw new NotEnoughSeatsException($"Nema dovoljno mesta za ovu zonu {ticket.ZoneId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while checking seats availability: {ex.Message}");
            }
        }

        public void ReturnAvailableSeatsForTicket(ReservationTicket ticket)
        {
            try
            {
                var availableSeats = _availableSeatsRepository.GetAvailableSeats(ticket.ZoneId, ticket.Reservation.ConcertId);
                if (availableSeats != null)
                {
                    availableSeats.Count += ticket.Quantity;
                    _availableSeatsRepository.Save();
                    _logger.LogInformation("Available seats updated successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while checking seats availability: {ex.Message}");
            }
        }

        public void UpdateAvailableSeatsForTicket(ReservationTicket existingTicket, ReservationTicket updatedTicket)
        {
            try
            {
                var availableSeats = _availableSeatsRepository.GetAvailableSeats(existingTicket.ZoneId, existingTicket.Reservation.ConcertId);
                if (availableSeats != null)
                {
                    availableSeats.Count += existingTicket.Quantity;
                    availableSeats.Count -= updatedTicket.Quantity;

                    _availableSeatsRepository.Save();
                }
                else
                {
                    throw new NotEnoughSeatsException($"Nema dovoljno mesta za ovu zonu {existingTicket.ZoneId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while checking seats availability: {ex.Message}");
            }
        }
    }
}

