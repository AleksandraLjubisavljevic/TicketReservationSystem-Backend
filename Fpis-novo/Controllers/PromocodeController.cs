using AutoMapper;
using FpisNovoAPI.Data;
using FpisNovoAPI.Dto;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using Microsoft.AspNetCore.Authorization;
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
    public class PromocodeController : Controller
    {
        private readonly IPromocodeRepository _promocodeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PromocodeController> _logger;
        public PromocodeController(IPromocodeRepository promocodeRepository, IMapper mapper, ILogger<PromocodeController> logger)
        {
            _promocodeRepository = promocodeRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Promocode>))]
        public IActionResult GetPromocodes()
        {
            try
            {
                var promocodes = _mapper.Map<List<PromocodeDto>>(_promocodeRepository.GetPromocodes());
                return Ok(promocodes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching promocodes: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
            
        }
        [HttpGet("{promocodeId}")]
        [ProducesResponseType(200, Type = typeof(Promocode))]
        [ProducesResponseType(400)]
        public IActionResult GetPromocode(int promocodeId)
        {
            try
            {
                if (!_promocodeRepository.PromocodeExists(promocodeId))
                {
                    return NotFound();
                }
                var promocode = _promocodeRepository.GetPromocode(promocodeId);
                if (promocode == null)
                {
                    return NotFound();
                }
                return Ok(promocode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching promocode: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("ByCode/{code}")] 
        [ProducesResponseType(200, Type = typeof(Promocode))]
        [ProducesResponseType(400)]
        public IActionResult GetPromocodeByCode(string code)
        {
            try
            {
                if (code==null)
                {
                    return BadRequest();
                }
            
                var promocode = _mapper.Map<PromocodeDto>(_promocodeRepository.GetPromocodeByCode(code));
                if (promocode == null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(promocode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while checking promocode: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePromocode([FromBody] PromocodeDto promocodeNew)
        {
            try
            {


                if (promocodeNew == null)
                {
                    return BadRequest(ModelState);
                }
                var promocode =
                    _promocodeRepository.GetPromocodes().Where(c => c.Code.Trim().ToUpper() == promocodeNew.Code.TrimEnd().ToUpper()).FirstOrDefault();

                if (promocode != null)
                {
                    ModelState.AddModelError("", "Promocode already exists");
                    return StatusCode(422, ModelState);
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var promocodeMap = _mapper.Map<Promocode>(promocodeNew);

                if (!_promocodeRepository.CreatePromocode(promocodeMap))
                {
                    ModelState.AddModelError("", "Something went wrong while savin");
                    return StatusCode(500, ModelState);
                }

                return Ok("Successfully created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating promocode: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{promocodeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePromocode(int promocodeId, [FromBody] PromocodeDto promocodeUpdate)
        {
            try { 
                if (promocodeUpdate == null)
                {
                    return BadRequest(ModelState);
                }
                if (!_promocodeRepository.PromocodeExists(promocodeId))
                {
                    return NotFound();
                }
                if (promocodeId != promocodeUpdate.PromocodeId)
                {
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var promocodeMap = _mapper.Map<Promocode>(promocodeUpdate);

                if (!_promocodeRepository.UpdatePromocode(promocodeMap))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating promocode: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpPut("{reservationId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [Authorize]
        public IActionResult CancelPromocode(int reservationId)
        {
            try
            {
                var promocode = _promocodeRepository.GetPromocodeByReservationCreatedId(reservationId);
                if (promocode == null)
                {
                    return NotFound();
                }
                promocode.IsUsed = false;
                promocode.ReservationId = 0;
                if (!_promocodeRepository.UpdatePromocode(promocode))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating promocode: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{reservationId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [Authorize]
        public IActionResult DeletePromocode(int reservationId)
        {
            try
            {
                var promocode = _promocodeRepository.GetPromocodeByReservationCreatedId(reservationId);
                if (promocode == null)
                {
                    return NotFound();
                }


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!_promocodeRepository.DeletePromocode(promocode))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting");
                    return StatusCode(500, ModelState);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting promocode: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        
    }
}
