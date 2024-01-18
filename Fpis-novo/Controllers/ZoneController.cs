using AutoMapper;
using FpisNovoAPI.Data;
using FpisNovoAPI.Dto;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FpisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZoneController : ControllerBase
    {
        private readonly IZoneRepository _zoneRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ZoneController> _logger;
        public ZoneController(IZoneRepository zoneRepository, IMapper mapper, ILogger<ZoneController> logger)
        {
            _zoneRepository = zoneRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ZoneDto>))]
        public IActionResult GetZones()
        {
            try
            {
                var zones = _mapper.Map<List<ZoneDto>>(_zoneRepository.GetZones());
                return Ok(zones);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching zones: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }
        [HttpGet("{zoneId}")]
        [ProducesResponseType(200, Type = typeof(Zone))]
        [ProducesResponseType(400)]
        public IActionResult GetZone(int zoneId)
        {
            try
            {
                if (!_zoneRepository.ZoneExists(zoneId))
                {
                    return NotFound();
                }
                var zone = _zoneRepository.GetZone(zoneId);
                if (zone == null)
                {
                    return NotFound();
                }
                var zoneDto = _mapper.Map<ZoneDto>(zone);
                return Ok(zoneDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching zone with ID {zoneId}: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateZone([FromBody] ZoneDto zoneNew)
        {
            try
            {

                if (zoneNew == null)
                {
                    return BadRequest(ModelState);
                }
                var zone =
                    _zoneRepository.GetZones().Where(c => c.Name.Trim().ToUpper() == zoneNew.Name.TrimEnd().ToUpper()).FirstOrDefault();

                if (zone != null)
                {
                 ModelState.AddModelError("", "Zone already exists");
                 return StatusCode(422, ModelState);
                }
            
                if (!ModelState.IsValid)
                return BadRequest(ModelState);

                var zoneMap = _mapper.Map<Zone>(zoneNew);

                if (!_zoneRepository.CreateZone(zoneMap))
                {
                    ModelState.AddModelError("", "Something went wrong while saving");
                    return StatusCode(500, ModelState);
                }

                return Ok("Successfully created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating a zone: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{zoneId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateZone(int zoneId, [FromBody] ZoneDto zoneUpdate)
        {
           try
            {
                if (zoneUpdate == null)
                {
                    return BadRequest(ModelState);
                }
                if (!_zoneRepository.ZoneExists(zoneId))
                {
                    return NotFound();
                }
                if (zoneId != zoneUpdate.ZoneId)
                {
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var zoneMap = _mapper.Map<Zone>(zoneUpdate);

                if (!_zoneRepository.UpdateZone(zoneMap))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating a zone: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpDelete("{zoneId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteZone(int zoneId)
        {
            try
            {
                if (!_zoneRepository.ZoneExists(zoneId))
                {
                    return NotFound();
                }
                var zoneDelete = _zoneRepository.GetZone(zoneId);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!_zoneRepository.DeleteZone(zoneDelete))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting");
                    return StatusCode(500, ModelState);
                }
                return NoContent();
                }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting a zone with ID {zoneId}: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        
    }
}
