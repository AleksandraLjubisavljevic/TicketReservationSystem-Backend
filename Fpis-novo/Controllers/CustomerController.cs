using AutoMapper;
using FpisNovoAPI.Data;
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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerController> _logger;
        public CustomerController(ICustomerRepository customerRepository, IMapper mapper, ILogger<CustomerController> logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
        public IActionResult GetCustomers()
        {
            try
            {
                var customers = _mapper.Map<List<CustomerDto>>(_customerRepository.GetCustomers());
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching customers: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
       /* [HttpGet("ByEmail/{email}")]
        [ProducesResponseType(200, Type = typeof(CustomerDto))]
        public IActionResult GetCustomer(string email)
        {
            if (!_customerRepository.CustomerAlreadyExists(email))
            {
                return NotFound();
            }

            var customer = _mapper.Map<CustomerDto>(_customerRepository.GetCustomerByEmail(email));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(customer);
        }*/
        [HttpGet("{customerId}")]
        [ProducesResponseType(200, Type = typeof(CustomerDto))]
        [ProducesResponseType(400)]
        public IActionResult GetCustomer(int customerId)
        {
            try
            {
                if (!_customerRepository.CustomerExists(customerId))
                {
                    return NotFound();
                }
                var customer = _customerRepository.GetCustomer(customerId);
                if (customer == null)
                {
                    return NotFound();
                }
                
                var customerDto = _mapper.Map<CustomerDto>(customer);
                return Ok(customerDto);
            
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching customer: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
            
        }
        
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCustomer([FromBody] CustomerDto customerNew)
        {
            try
            {
                if (customerNew == null)
                {
                    return BadRequest(ModelState);
                }
                var customer =
                    _customerRepository.GetCustomers().Where(c => c.Email.Trim().ToUpper() == customerNew.Email.TrimEnd().ToUpper()).FirstOrDefault();

                if (customer != null)
                {
                    ModelState.AddModelError("", "Customer already exists");
                    return StatusCode(422, ModelState);
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var customerMap = _mapper.Map<Customer>(customerNew);
                if (!_customerRepository.CreateCustomer(customerMap))
                {
                    ModelState.AddModelError("", "Something went wrong while saving");
                    return StatusCode(500, ModelState);
                }
                return Ok("Successfully created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating customer: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPut("{customerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCustomer(int customerId, [FromBody] CustomerDto customerUpdate)
        {
            try 
            { 
                if (customerUpdate == null)
                {
                    return NotFound();
                }
                if (customerId != customerUpdate.CustomerId)
                {
                    return BadRequest(ModelState);
                }
                if (!_customerRepository.CustomerExists(customerId))
                {
                    return NotFound();
                }
                var customer = _mapper.Map<Customer>(customerUpdate);
                if (!_customerRepository.UpdateCustomer(customer))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating customer: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpDelete("{customerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCustomer(int customerId)
        {
            try
            {
                if (!_customerRepository.CustomerExists(customerId))
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var customer = _customerRepository.GetCustomer(customerId);
                if (!_customerRepository.DeleteCustomer(customer))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting");
                    return StatusCode(500, ModelState);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting customer: {ex.Message}");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
       
    }
}
