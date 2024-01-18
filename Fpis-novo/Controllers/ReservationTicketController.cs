using AutoMapper;
using FpisNovoAPI.Dto;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationTicketController : Controller
    {
        private readonly IReservationTicketRepository _reservationTicketRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservationTicketController> _logger;
        public ReservationTicketController(IReservationTicketRepository reservationTicketRepository, IMapper mapper, ILogger<ReservationTicketController> logger)
        {
            _reservationTicketRepository = reservationTicketRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReservationTicket>))]
        public IActionResult GetReservationTickets()
        {
            try
            {
                var tickets = _mapper.Map<List<ReservationTicketDto>>(_reservationTicketRepository.GetTickets());
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching reservation tickets: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{ticketId}")]
        [ProducesResponseType(200, Type = typeof(ReservationTicket))]
        [ProducesResponseType(400)]
        public IActionResult GetReservationTicket(int ticketId)
        {
            try
            {
                if (!_reservationTicketRepository.ReservationTicketExists(ticketId))
                {
                    return NotFound();
                }
                var ticket = _reservationTicketRepository.GetReservationTicket(ticketId);
                
                if (ticket == null)
                {
                    return NotFound();
                }
                var ticketDto = _mapper.Map<ReservationTicketDto>(ticket);
                return Ok(ticketDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching reservation ticket: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("ByReservation/{reservationId}")]
        [ProducesResponseType(200, Type = typeof(ReservationTicket))]
        [ProducesResponseType(400)]
        public IActionResult GetTicketsForReservation(int reservationId)
        {
            try
            {
                var tickets = _reservationTicketRepository.GetTicketsForReservation(reservationId);
                if (tickets == null)
                {
                    return NotFound();
                }
                var ticketsDto = _mapper.Map<List<ReservationTicketDto>>(tickets);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching tickets: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReservationTicket([FromBody] ReservationTicketDto ticketNew)
        {
            try
            {
                if (ticketNew == null)
                {
                    return BadRequest(ModelState);
                }

                var ticketMap = _mapper.Map<ReservationTicket>(ticketNew);

                if (!_reservationTicketRepository.CreateReservationTicket(ticketMap))
                {
                    ModelState.AddModelError("", "Something went wrong while savin");
                    return StatusCode(500, ModelState);
                }

                return Ok("Successfully created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while saving reservation tickets: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{ticketId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReservationTicket(int ticketId, [FromBody] ReservationTicketDto reservationTicketUpdate)
        {
            try
            {
                if (reservationTicketUpdate == null)
                {
                    return BadRequest(ModelState);
                }
                if (!_reservationTicketRepository.ReservationTicketExists(ticketId))
                {
                    return NotFound();
                }
                if (ticketId != reservationTicketUpdate.ReservationTicketId)
                {
                    return BadRequest(ModelState);
                }

                var reservationTicketMap = _mapper.Map<ReservationTicket>(reservationTicketUpdate);

                if (!_reservationTicketRepository.UpdateReservationTicket(reservationTicketMap))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating reservation tickets: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
        [HttpDelete("{ticketId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReservationTicket(int ticketId)
        {
            try { 
                if (!_reservationTicketRepository.ReservationTicketExists(ticketId))
                {
                    return NotFound();
                }
                var ticketDelete = _reservationTicketRepository.GetReservationTicket(ticketId);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!_reservationTicketRepository.DeleteReservationTicket(ticketDelete))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting");
                    return StatusCode(500, ModelState);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting reservation tickets: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
