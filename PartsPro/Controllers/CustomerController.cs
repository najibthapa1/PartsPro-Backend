using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.Application.DTOs.Auth;
using PartsPro.Application.DTOs.Customers;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Application.Interfaces.Services;
using System.Security.Claims;

namespace PartsPro.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IAuthService _authService;
    private readonly ICustomerRepository _customerRepository;

    public CustomerController(ICustomerService customerService, IAuthService authService, ICustomerRepository customerRepository)
    {
        _customerService = customerService;
        _authService = authService;
        _customerRepository = customerRepository;
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
    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpGet("profile/{customerId}")]
    public async Task<ActionResult<CustomerProfileResponse>> GetProfile(int customerId)
    {
        if (!await CanAccessCustomerAsync(customerId))
            return Forbid();

        var profile = await _customerService.GetProfileAsync(customerId);
        return Ok(profile);
    }

    /// <summary>
    /// Update a customer profile.
    /// </summary>
    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpPut("profile/{customerId}")]
    public async Task<IActionResult> UpdateProfile(int customerId, [FromBody] UpdateCustomerRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await CanAccessCustomerAsync(customerId))
            return Forbid();

        await _customerService.UpdateProfileAsync(customerId, request);
        return Ok(new { message = "Profile updated successfully" });
    }

    /// <summary>
    /// Add a vehicle to the customer profile.
    /// </summary>
    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpPost("{customerId}/vehicles")]
    public async Task<IActionResult> AddVehicle(int customerId, [FromBody] AddVehicleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await CanAccessCustomerAsync(customerId))
            return Forbid();

        await _customerService.AddVehicleAsync(customerId, request);
        return Ok(new { message = "Vehicle added successfully" });
    }

    /// <summary>
    /// Get all vehicles for a customer.
    /// </summary>
    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpGet("{customerId}/vehicles")]
    public async Task<ActionResult<IEnumerable<CustomerVehicleResponse>>> GetVehicles(int customerId)
    {
        if (!await CanAccessCustomerAsync(customerId))
            return Forbid();

        var vehicles = await _customerService.GetVehiclesAsync(customerId);
        return Ok(vehicles);
    }

    /// <summary>
    /// Get complete customer history.
    /// </summary>
    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpGet("{customerId}/history")]
    public async Task<ActionResult<CustomerHistoryResponse>> GetCustomerHistory(int customerId)
    {
        if (!await CanAccessCustomerAsync(customerId))
            return Forbid();

        var history = await _customerService.GetCustomerHistoryAsync(customerId);
        return Ok(history);
    }

    /// <summary>
    /// Search for customers by name, phone, ID, or vehicle plate number.
    /// Staff can use this to quickly find customer records at the counter.
    /// </summary>
    /// <param name="query">The search term</param>
    /// <returns>A list of matching customers with their vehicles and credit balance</returns>
    [HttpGet("search/{query}")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<ActionResult<IEnumerable<CustomerSearchResponse>>> SearchCustomers(string query)
    {
        var customers = await _customerService.SearchCustomersAsync(query);
        return Ok(customers);
    }

    private async Task<bool> CanAccessCustomerAsync(int customerId)
    {
        if (User.IsInRole("Admin") || User.IsInRole("Staff"))
            return true;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return false;

        var customer = await _customerRepository.GetByUserIdAsync(userId);
        return customer?.Id == customerId;
    }
}