using AutoMapper;
using FpisNovoAPI.Dto;
using FpisNovoAPI.Interfaces;
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
    public class AvailableSeatsController : ControllerBase
    {
        private readonly IAvailableSeatsRepository _availableSeatsRepository;
        private readonly IConcertRepository _concertRepository;
        private readonly IZoneRepository _zoneRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AvailableSeatsController> _logger;
        public AvailableSeatsController(IAvailableSeatsRepository availableSeatsRepository,
            IConcertRepository concertRepository,
            IZoneRepository zoneRepository,
            ILogger<AvailableSeatsController> logger,
            IMapper mapper)
        {
            _availableSeatsRepository = availableSeatsRepository;
            _concertRepository = concertRepository;
            _zoneRepository = zoneRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult GetAvailableSeats(int zoneId, int concertId)
        {
            try
            {

                if (!_concertRepository.ConcertExist(concertId))
                {
                    return NotFound();
                }
                if (!_zoneRepository.ZoneExists(zoneId))
                {
                    return NotFound();
                }
                var availableSeats = _mapper.Map<AvailableSeatsDto>(_availableSeatsRepository.GetAvailableSeats(zoneId, concertId));
                if (availableSeats == null)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(availableSeats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while checking seats availability: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("check-availability")]
        public IActionResult CheckAvailability(int zoneId, int numberOfCards,int concertId)
        {
            try 
            { 
                var availableSeats = _availableSeatsRepository.GetAvailableSeats(zoneId, concertId); 

                if (availableSeats != null && availableSeats.Count >= numberOfCards)
                {
                    return Ok(true); 
                }
                else
                {
                    return Ok(false); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while checking seats availability: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}



