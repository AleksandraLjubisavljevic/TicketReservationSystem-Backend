using AutoMapper;
using FpisNovoAPI.Dto;
using FpisNovoAPI.Helper;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using FpisNovoAPI.Requests;
using FpisNovoAPI.Responces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Services
{
    public class ReservationService : IReservationService
    {
        //private readonly ITokenVersionStore _tokenVersionStore;
        private readonly IReservationRepository _reservationRepository;
        private readonly IReservationTicketRepository _reservationTicketRepository;
        private readonly IPromocodeRepository _promocodeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConcertRepository _concertRepository;
        private readonly IZoneRepository _zoneRepository;
        private readonly ITokenService _tokenService;
        private readonly IPromocodeService _promocodeService;
        private readonly IMapper _mapper;
        private readonly IAvailableSeatsService _availableSeatsService;
        private readonly ILogger<ReservationService> _logger;
        

        public ReservationService(
           // ITokenVersionStore tokenVersionStore,
            IReservationRepository reservationRepository,
            IPromocodeRepository promocodeRepository,
            IReservationTicketRepository reservationTicketRepository,
            ICustomerRepository customerRepository,
            IConcertRepository concertRepository,
            IZoneRepository zoneRepository,
            IAvailableSeatsService availableSeatsService,
            ITokenService tokenService,
            IPromocodeService promocodeService,
            IMapper mapper,
            ILogger<ReservationService> logger
            )
        {
             //_tokenVersionStore = tokenVersionStore;
            _reservationRepository = reservationRepository;
            _promocodeRepository = promocodeRepository;
            _availableSeatsService = availableSeatsService;
            _reservationTicketRepository = reservationTicketRepository;
            _customerRepository = customerRepository;
            _concertRepository = concertRepository;
            _zoneRepository = zoneRepository;
            _tokenService = tokenService;
            _promocodeService = promocodeService;
            _mapper = mapper;
            _logger = logger;
            
        }
        public ICollection<ReservationDto> GetReservationsWithTickets()
        {
            try
            {
                var reservations = _reservationRepository.GetReservations();
                if (reservations == null)
                {
                    _logger.LogError($"Reservations not found");
                    return null;
                }
                var reservationsWithTickets = new List<ReservationDto>();

                foreach (var reservation in reservations)
                {
                    var tickets = _reservationTicketRepository.GetTicketsForReservation(reservation.ReservationId);
                    if (tickets == null)
                    {
                        _logger.LogError($"Tickets for this reservation are not found");
                        return null;
                    }
                    var reservationDto = _mapper.Map<ReservationDto>(reservation);
                    reservationDto.Tickets = _mapper.Map<List<ReservationTicketDto>>(tickets);
                    reservationsWithTickets.Add(reservationDto);
                }

                return reservationsWithTickets;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching tickets: {ex.Message}");
                return null;
            }
        }
        public ReservationResponse CreateReservation(CreateReservationRequest reservationNew)
        {
            try
            {
                var reservation = _mapper.Map<Reservation>(reservationNew);
                if (reservation == null)
                {
                    _logger.LogError($"An error occurred while maping reservation");
                    return null;
                }
                reservation.Token = _tokenService.GenerateReservationToken(reservation.CustomerId);
                string generatedPromocode=null;
                using (var transaction = _reservationRepository.BeginTransaction())
                {
                    if (!_availableSeatsService.AreEnoughSeatsAvailable(reservation))
                    {
                        return HandleInsufficientSeats();
                    }

                    _availableSeatsService.ReduceAvailableSeats(reservation);
                    var promoCodeValidationResult = _promocodeService.ValidatePromoCode(reservation.UsedPromocodeId);

                    double totalPrice = CalculateTotalPrice(reservation.Tickets, promoCodeValidationResult.IsValid);
                    reservation.TotalPrice = totalPrice;
                    if (reservation.Customer == null)
                    {
                        _logger.LogError($"Customer can not be null");
                        return null;
                    }
                    var customer = _customerRepository.GetCustomerByEmail(reservation.Customer.Email);
                    if (customer != null)
                    {

                        reservation.Customer = customer;
                    }
                    
                    if (!_reservationRepository.CreateReservation(reservation))
                    {
                        _logger.LogError($"An error occurred while creating reservation");
                        return null;
                    }
                    
                    var promocodeUsed = _promocodeRepository.GetPromocodeByReservationUsedId(reservation.UsedPromocodeId);
                    if (promocodeUsed != null)
                    {
                        if (!_promocodeService.MarkPromoCodeAsUsed(promocodeUsed.PromocodeId, reservation.ReservationId))
                        {
                            transaction.Rollback();
                            return null;
                        }
                    }
                    generatedPromocode = _promocodeService.GeneratePromoCode(reservation);
                    var promocodeId = _promocodeService.CreatePromoCode(generatedPromocode, reservation);

                    if (promocodeId == -1)
                    {
                        transaction.Rollback();
                        return null;
                    }

                    reservation.PromocodeId = promocodeId;

                    if (!_reservationRepository.UpdateReservation(reservation))
                    {
                        transaction.Rollback();
                        return null;
                    }

                    transaction.Commit();
                }

                var reservationResponse = new ReservationResponse
                {
                    Message = "Success",
                    PromoCode = generatedPromocode,
                    Token = reservation.Token
                };

                return reservationResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating reservation: {ex.Message}");
                var reservationResponse = new ReservationResponse
                {
                    Message = "Error",
                    PromoCode = null,
                    Token = null
                };
                return reservationResponse;
            }
        }
        public Reservation GetReservationByEmailAndToken(string email, string token)
        {
            try
            {
                var reservation = _reservationRepository.GetReservationByEmailAndToken(email, token);

                if (reservation == null)
                {
                    _logger.LogError($"Reservation not found");
                    return null;
                }

                var customer = _customerRepository.GetCustomer(reservation.CustomerId);
                if (customer == null)
                {
                    _logger.LogError($"Customer not found");
                    return null;
                }
                reservation.Customer = customer;

                var concert = _concertRepository.GetConcert(reservation.ConcertId);
                if (concert == null)
                {
                    _logger.LogError($"Concert not found");
                    return null;
                }
                reservation.Concert = concert;

                List<ReservationTicket> tickets = new List<ReservationTicket>();
                ICollection<ReservationTicket> allTickets = _reservationTicketRepository.GetTickets();
                if (allTickets == null)
                {
                    _logger.LogError($"Tickets not found");
                    return null;
                }
                foreach (ReservationTicket ticket in allTickets)
                {
                    if (ticket.ReservationId == reservation.ReservationId)
                    {
                        ticket.ZoneData = _zoneRepository.GetZone(ticket.ZoneId);
                        tickets.Add(ticket);
                    }
                }

                return reservation;
            }
            
            catch(Exception ex)
            {
                _logger.LogError($"An error occurred while fetching reservation: {ex.Message}");
                return null;
            }
        }
        public bool UpdateReservation(UpdateReservationRequest reservationUpdate)
        {
            using (var transaction = _reservationRepository.BeginTransaction())
            {
                try
                {
                    var reservationToUpdate = _reservationRepository.GetReservation(reservationUpdate.ReservationId);
                    if (reservationToUpdate == null)
                    {
                        _logger.LogError($"Can not find reservation");
                        return false;
                    }
                    var tickets = _reservationTicketRepository.GetTicketsForReservation(reservationToUpdate.ReservationId);
                    if (tickets == null)
                    {
                        _logger.LogError($"Can not find tickets for reservation");
                        return false;
                    }
                    var reservationTicketsUpdate = _mapper.Map<List<ReservationTicket>>(reservationUpdate.Tickets);

                    var removedTickets = new List<ReservationTicket>();
                    var addedTickets = new List<ReservationTicket>();
                    foreach (var existingTicket in tickets)
                    {
                        var updatedTicket = reservationUpdate.Tickets.FirstOrDefault(t => t.ReservationTicketId == existingTicket.ReservationTicketId);
                        if (updatedTicket != null)
                        {
                            _availableSeatsService.UpdateAvailableSeatsForTicket(existingTicket, _mapper.Map<ReservationTicket>(updatedTicket));
                            existingTicket.Quantity = updatedTicket.Quantity;
                            _reservationTicketRepository.UpdateReservationTicket(existingTicket);

                        }
                        else
                        {
                            removedTickets.Add(existingTicket);
                        }
                    }
                    foreach (var removedTicket in removedTickets)
                    {
                        tickets.Remove(removedTicket);
                        _reservationTicketRepository.DeleteReservationTicket(removedTicket);
                        _availableSeatsService.ReturnAvailableSeatsForTicket(removedTicket);
                    }
                    foreach (var updatedTicket in reservationTicketsUpdate)
                    {
                        var existingTicket = tickets.FirstOrDefault(t => t.ReservationTicketId == updatedTicket.ReservationTicketId);

                        if (existingTicket == null)
                        {
                            var newTicket = _mapper.Map<ReservationTicket>(updatedTicket);
                            newTicket.ReservationId = reservationUpdate.ReservationId;
                            _reservationTicketRepository.CreateReservationTicket(newTicket);
                            _availableSeatsService.ReduceAvailableSeatsForTicket(newTicket);
                        }
                    }
                    bool promocodeDiscount = false;
                    if (reservationToUpdate.UsedPromocodeId != -1)
                    {
                        promocodeDiscount = true;
                    }
                    double totalPrice = CalculateTotalPrice(reservationTicketsUpdate, promocodeDiscount);
                    reservationToUpdate.TotalPrice = totalPrice;
                    reservationToUpdate.Tickets = reservationTicketsUpdate;

                    if (!_reservationRepository.UpdateReservation(reservationToUpdate))
                    {
                        transaction.Rollback();
                        return false;
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public bool DeleteReservation(int reservationId)
        {
            using (var transaction = _reservationRepository.BeginTransaction())
            {
                try
                {
                    var reservationToDelete = _reservationRepository.GetReservation(reservationId);
                    if (reservationToDelete == null)
                    {
                        return false;
                    }
                    var customerId = reservationToDelete.CustomerId;
                    var otherReservationsByCustomer = _reservationRepository.GetReservationsByCustomerId(customerId);
                    _availableSeatsService.ReturnAvailableSeats(reservationToDelete);
                    var promocodeToDelete = _promocodeRepository.GetPromocodeByReservationId(reservationId);
                    if (promocodeToDelete != null && !_promocodeRepository.DeletePromocode(promocodeToDelete))
                    {
                        transaction.Rollback();
                        return false;
                    }

                    var promocodeUsed = _promocodeRepository.GetPromocodeByReservationCreatedId(reservationId);
                    if (promocodeUsed != null)
                    {
                        promocodeUsed.UsedReservationId = null;
                        promocodeUsed.IsUsed = false;

                        if (!_promocodeRepository.UpdatePromocode(promocodeUsed))
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    if (!_reservationRepository.DeleteReservation(reservationToDelete))
                    {
                        transaction.Rollback();
                        return false;
                    }
                    if (otherReservationsByCustomer != null && otherReservationsByCustomer.Count == 1)
                    {
                        var customerToDelete = _customerRepository.GetCustomer(customerId);
                        if (customerToDelete != null)
                        {
                            _customerRepository.DeleteCustomer(customerToDelete);
                        }
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        private double CalculateTotalPrice(ICollection<ReservationTicket> tickets, bool applyPromoCodeDiscount)
        {
            double totalPrice = 0;
            var currentDate = DateTime.Now;

            foreach (var ticket in tickets)
            {
                ticket.ZoneData = _zoneRepository.GetZone(ticket.ZoneId);
                if (ticket.ZoneData != null)
                {
                    var discountFactor = (int)Math.Floor((double)ticket.Quantity / 5);
                    var discount = discountFactor * (ticket.ZoneData.Price / 2);

                    var ticketTotal = ticket.ZoneData.Price * ticket.Quantity - discount;
                    if (currentDate < new DateTime(2023, 11, 1))
                    {
                        ticketTotal *= 0.9;
                    }
                    
                    totalPrice += ticketTotal;
                }
            }
            if (applyPromoCodeDiscount)
            {
                totalPrice *= 0.95; 
            }

            return totalPrice;
        }
        
        private ReservationResponse HandleInsufficientSeats()
        {
            return new ReservationResponse
            {
                Message = "Not enough available seats.",
                PromoCode = null,
                Token = null
            };
        }
        private ReservationResponse HandleInvalidPromoCode(string errorMessage)
        {
            return new ReservationResponse
            {
                Message = errorMessage,
                PromoCode = null,
                Token = null
            };
        }
        

    }



}


