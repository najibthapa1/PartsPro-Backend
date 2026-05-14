using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.Application.DTOs.Auth;
using PartsPro.Application.DTOs.Customers;
using PartsPro.Application.Interfaces.Services;

namespace PartsPro.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IAuthService _authService;

    public CustomerController(ICustomerService customerService, IAuthService authService)
    {
        _customerService = customerService;
        _authService = authService;
    }

    /// <summary>
    /// Get a paginated list of customers.
    /// </summary>
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerProfileResponse>>> GetAll(int pageNumber = 1, int pageSize = 10)
    {
        var customers = await _customerService.GetAllCustomersAsync(pageNumber, pageSize);
        return Ok(customers);
    }

    /// <summary>
    /// Customer self-registration. Kept here for compatibility, but registration is handled by auth service.
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Get a customer profile summary.
    /// </summary>
    [Authorize]
    [HttpGet("profile/{customerId}")]
    public async Task<ActionResult<CustomerProfileResponse>> GetProfile(int customerId)
    {
        var profile = await _customerService.GetProfileAsync(customerId);
        return Ok(profile);
    }

    /// <summary>
    /// Update a customer profile.
    /// </summary>
    [Authorize]
    [HttpPut("profile/{customerId}")]
    public async Task<IActionResult> UpdateProfile(int customerId, [FromBody] UpdateCustomerRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _customerService.UpdateProfileAsync(customerId, request);
        return Ok(new { message = "Profile updated successfully" });
    }

    /// <summary>
    /// Add a vehicle to the customer profile.
    /// </summary>
    [Authorize]
    [HttpPost("{customerId}/vehicles")]
    public async Task<IActionResult> AddVehicle(int customerId, [FromBody] AddVehicleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _customerService.AddVehicleAsync(customerId, request);
        return Ok(new { message = "Vehicle added successfully" });
    }

    /// <summary>
    /// Get all vehicles for a customer.
    /// </summary>
    [Authorize]
    [HttpGet("{customerId}/vehicles")]
    public async Task<ActionResult<IEnumerable<CustomerVehicleResponse>>> GetVehicles(int customerId)
    {
        var vehicles = await _customerService.GetVehiclesAsync(customerId);
        return Ok(vehicles);
    }

    /// <summary>
    /// Get complete customer history.
    /// </summary>
    [Authorize]
    [HttpGet("{customerId}/history")]
    public async Task<ActionResult<CustomerHistoryResponse>> GetCustomerHistory(int customerId)
    {
        var history = await _customerService.GetCustomerHistoryAsync(customerId);
        return Ok(history);
    }
}