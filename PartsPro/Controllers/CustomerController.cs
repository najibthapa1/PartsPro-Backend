using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.DTOs;
using PartsPro.DTOs.CustomerDTOs;
using PartsPro.Services;

namespace PartsPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // F12: Customer Self-Register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = _customerService.Register(registerDto);
                if (result)
                    return Ok(new { message = "Registration successful" });

                return BadRequest(new { message = "Registration failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // F12: Get Customer Profile
        [Authorize]
        [HttpGet("profile/{customerId}")]
        public IActionResult GetProfile(int customerId)
        {
            try
            {
                var profile = _customerService.GetProfile(customerId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // F12: Update Customer Profile
        [Authorize]
        [HttpPut("profile/{customerId}")]
        public IActionResult UpdateProfile(int customerId, [FromBody] UpdateProfileDto updateDto)
        {
            try
            {
                var result = _customerService.UpdateProfile(customerId, updateDto);
                if (result)
                    return Ok(new { message = "Profile updated successfully" });

                return BadRequest(new { message = "Update failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // F12: Add Vehicle to Customer Profile
        [Authorize]
        [HttpPost("{customerId}/vehicles")]
        public IActionResult AddVehicle(int customerId, [FromBody] VehicleDto vehicleDto)
        {
            try
            {
                var result = _customerService.AddVehicle(customerId, vehicleDto);
                if (result)
                    return Ok(new { message = "Vehicle added successfully" });

                return BadRequest(new { message = "Failed to add vehicle" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // F12: Get Customer Vehicles
        [Authorize]
        [HttpGet("{customerId}/vehicles")]
        public IActionResult GetVehicles(int customerId)
        {
            try
            {
                var vehicles = _customerService.GetCustomerVehicles(customerId);
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // F14: Get Complete Customer History
        [Authorize]
        [HttpGet("{customerId}/history")]
        public IActionResult GetCustomerHistory(int customerId)
        {
            try
            {
                var history = _customerService.GetCustomerHistory(customerId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}