using AutoMapper;
using FpisNovoAPI.Data;
using FpisNovoAPI.Dto;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using Microsoft.AspNetCore.Http;
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
    public class ConcertController : ControllerBase
    {
        private readonly IConcertRepository _concertRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ConcertController> _logger;
        public ConcertController(IConcertRepository concertRepository, IMapper mapper, ILogger<ConcertController> logger)
        {
            _concertRepository = concertRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ConcertDto>))]
        public IActionResult GetConcerts()
        {
            try
            {
                var concerts = _mapper.Map<List<ConcertDto>>(_concertRepository.GetConcerts());
                return Ok(concerts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching concerts: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("{concertId}")]
        [ProducesResponseType(200, Type = typeof(ConcertDto))]
        [ProducesResponseType(400)]
        public IActionResult GetConcert(int concertId)
        {
            try
            {
                if (!_concertRepository.ConcertExist(concertId))
                {
                    return NotFound();
                }
            
                var concert = _concertRepository.GetConcert(concertId);
                if (concert == null)
                {
                    return NotFound();
                }

                var concertDto = _mapper.Map<ConcertDto>(concert);
                return Ok(concertDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching concert with ID {concertId}: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateConcert([FromBody] ConcertDto concertNew)
        {
            try
            {
                if (concertNew == null)
                {
                    return BadRequest(ModelState);
                }
                var concert =
                    _concertRepository.GetConcerts().Where(c => c.Name.Trim().ToUpper() == concertNew.Name.TrimEnd().ToUpper()).FirstOrDefault();

                if (concert != null)
                {
                    ModelState.AddModelError("", "Concert already exists");
                    return StatusCode(422, ModelState);
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var concertMap = _mapper.Map<Concert>(concertNew);

                if (!_concertRepository.CreateConcert(concertMap))
                {
                    ModelState.AddModelError("", "Something went wrong while saving");
                    return StatusCode(500, ModelState);
                }

                return Ok("Successfully created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating a concert: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{concertId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateConcert(int concertId, [FromBody]ConcertDto concertUpdate)
        {
            try
            {
                if (concertUpdate == null)
                {
                    return BadRequest(ModelState);
                }
                if (!_concertRepository.ConcertExist(concertId))
                {
                    return NotFound();
                }
                if (concertId != concertUpdate.ConcertId)
                {
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var concertMap = _mapper.Map<Concert>(concertUpdate);

                if (!_concertRepository.UpdateConcert(concertMap))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
            
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating a concert with ID {concertId}: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpDelete("{concertId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteConcert(int concertId)
        {
            try
            {
                if (!_concertRepository.ConcertExist(concertId))
                {
                    return NotFound();
                }
                var concertDelete = _concertRepository.GetConcert(concertId);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!_concertRepository.DeleteConcert(concertDelete))
                {
                    ModelState.AddModelError("","Something went wrong while deleting");
                    return StatusCode(500, ModelState);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting a concert with ID {concertId}: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
            
        }
    }

}

