using AutoMapper;
using FpisNovoAPI.Data;
using FpisNovoAPI.Dto;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using FpisNovoAPI.Requests;
using FpisNovoAPI.Responces;
using FpisNovoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FpisNovoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConcertRepository _concertRepository;
        private readonly IAvailableSeatsRepository _availableSeatsRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IReservationTicketRepository _reservationTicketRepository;
        private readonly IZoneRepository _zoneRepository;
        private readonly IPromocodeRepository _promocodeRepository;
        private readonly ITokenService _tokenService;
        private readonly IReservationService _reservationService;
        private readonly IAvailableSeatsService _availableSeatsService;
        private readonly ILogger<ReservationController> _logger;
        public ReservationController(IReservationRepository reservationRepository,
                                     IConcertRepository concertRepository,
                                     ICustomerRepository customerRepository,
                                     IAvailableSeatsRepository availableSeatsRepository,
                                     IAvailableSeatsService availableSeatsService,
                                     IMapper mapper, 
                                     IConfiguration configuration,
                                     IReservationTicketRepository reservationTicketRepository,
                                     IPromocodeRepository promocodeRepository,
                                     IZoneRepository zoneRepository,
                                     ITokenService tokenService,
                                     IReservationService reservationService,
                                     ILogger<ReservationController> logger)
        {
            _reservationRepository = reservationRepository;
            _customerRepository = customerRepository;
            _concertRepository = concertRepository;
            _availableSeatsRepository = availableSeatsRepository;
            _availableSeatsService = availableSeatsService;
            _mapper = mapper;
            _configuration = configuration;
            _reservationTicketRepository = reservationTicketRepository;
            _zoneRepository = zoneRepository;
            _promocodeRepository = promocodeRepository;
            _tokenService = tokenService;
            _reservationService = reservationService;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reservation>))]
        public IActionResult GetReservations()
        {
            try
            {
                var reservationsWithTickets = _reservationService.GetReservationsWithTickets();
                return Ok(reservationsWithTickets);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching reservations: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        /*public IActionResult GetReservations()
        {
            try
            {
                var reservations = _reservationRepository.GetReservations();
                if (reservations == null)
                {
                    return NotFound();
                }
                var reservationsDto = _mapper.Map<List<ReservationDto>>(reservations);
                var reservationsWithTickets = new List<ReservationDto>();
                foreach (ReservationDto reservation in reservationsDto)
                {
                    var tickets = _reservationRepository.GetTickets(reservation.ReservationId);
                    if (tickets == null)
                    {
                        return NotFound();
                    }
                    var ticketsDto = _mapper.Map<List<ReservationTicketDto>>(tickets);
                    reservation.Tickets = ticketsDto;
                    reservationsWithTickets.Add(reservation);
                }

                return Ok(reservationsWithTickets);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching promocode: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }*/
        [HttpGet("{reservationId}")]
        [ProducesResponseType(200, Type = typeof(ReservationDto))]
        [ProducesResponseType(400)]
        [Authorize]
        public IActionResult GetReservation(int reservationId)
        {
            try
            {
                if (!_reservationRepository.ReservationExists(reservationId))
                {
                    return NotFound();
                }
                var res = _reservationRepository.GetReservation(reservationId);

                if(res == null)
                {
                    return NotFound();
                }
                var reservationDto = _mapper.Map<ReservationDto>(res);
                return Ok(reservationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching reservations: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("reservations")]
        [Authorize]
        public ActionResult GetReservationByEmailAndToken(string email, string token)
        {
            try
            {
                var reservationDto = _reservationService.GetReservationByEmailAndToken(email, token);

                if (reservationDto == null)
                {
                    return NotFound();
                }

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(reservationDto, options);

                return Ok(json);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred while fetching reservation: {e.Message}");
                return StatusCode(500, "An error occurred.");
            }
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(ReservationResponse), 200)]
        [ProducesResponseType(400)]
        public IActionResult CreateReservation([FromBody] CreateReservationRequest reservationNew)
        {
            try
            {
                if (reservationNew == null)
                {
                    return BadRequest(ModelState);
                }

                var reservation = _reservationService.CreateReservation(reservationNew);

                if (reservation == null)
                {
                    ModelState.AddModelError("", "Something went wrong while creating reservation");
                    return StatusCode(500, ModelState);
                }

                if (reservation.Message == "Success")
                {
                    return Ok(reservation);
                }
                else if (reservation.Message == "Not enough available seats.")
                {
                    ModelState.AddModelError("", "Not enough available seats or invalid promo code.");
                    return BadRequest(ModelState);
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred during reservation creation.");
                    return StatusCode(500, ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating reservation: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [Authorize]
        public IActionResult UpdateReservation([FromBody] UpdateReservationRequest reservationUpdate)
        {
            try 
            { 
                if (reservationUpdate == null)
                {
                    return BadRequest(ModelState);
                }
                var updateResult = _reservationService.UpdateReservation(reservationUpdate);
                if (updateResult)
                {
                    return NoContent();
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong while updating reservation");
                    return StatusCode(500, ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating reservation: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpDelete("{reservationId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [Authorize]
        public IActionResult DeleteReservation(int reservationId)
        {
            try
            {
                var deleteResult = _reservationService.DeleteReservation(reservationId);

                if (deleteResult)
                {
                    return NoContent();
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong while deleting reservation");
                    return StatusCode(500, ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting reservation: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        
        
        
        
    }
}

