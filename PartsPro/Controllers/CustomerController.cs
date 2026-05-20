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

    public CustomerController(
        ICustomerService customerService,
        IAuthService authService,
        ICustomerRepository customerRepository)
    {
        _customerService = customerService;
        _authService = authService;
        _customerRepository = customerRepository;
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerProfileResponse>>> GetAll(int pageNumber = 1, int pageSize = 10)
    {
        var customers = await _customerService.GetAllCustomersAsync(pageNumber, pageSize);
        return Ok(customers);
    }

    [HttpPost("register")]
    public async Task<ActionResult<object>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpGet("profile/{customerId:int}")]
    public async Task<ActionResult<CustomerProfileResponse>> GetProfile(int customerId)
    {
        if (!await CanAccessCustomerAsync(customerId)) return Forbid();
        var profile = await _customerService.GetProfileAsync(customerId);
        return Ok(profile);
    }

    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpPut("profile/{customerId:int}")]
    public async Task<IActionResult> UpdateProfile(int customerId, [FromBody] UpdateCustomerRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await CanAccessCustomerAsync(customerId)) return Forbid();
        await _customerService.UpdateProfileAsync(customerId, request);
        return Ok(new { message = "Profile updated successfully" });
    }

    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpPost("{customerId:int}/vehicles")]
    public async Task<IActionResult> AddVehicle(int customerId, [FromBody] AddVehicleRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await CanAccessCustomerAsync(customerId)) return Forbid();
        await _customerService.AddVehicleAsync(customerId, request);
        return Ok(new { message = "Vehicle added successfully" });
    }

    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpGet("{customerId:int}/vehicles")]
    public async Task<ActionResult<IEnumerable<CustomerVehicleResponse>>> GetVehicles(int customerId)
    {
        if (!await CanAccessCustomerAsync(customerId)) return Forbid();
        var vehicles = await _customerService.GetVehiclesAsync(customerId);
        return Ok(vehicles);
    }

    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpGet("{customerId:int}/history")]
    public async Task<ActionResult<CustomerHistoryResponse>> GetCustomerHistory(int customerId)
    {
        if (!await CanAccessCustomerAsync(customerId)) return Forbid();
        var history = await _customerService.GetCustomerHistoryAsync(customerId);
        return Ok(history);
    }

    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpGet("{customerId:int}/appointments")]
    public async Task<ActionResult<IEnumerable<CustomerAppointmentResponse>>> GetAppointments(int customerId)
    {
        if (!await CanAccessCustomerAsync(customerId)) return Forbid();
        var appointments = await _customerService.GetAppointmentsAsync(customerId);
        return Ok(appointments);
    }

    [Authorize(Roles = "Customer")]
    [HttpPost("{customerId:int}/appointments")]
    public async Task<ActionResult<CustomerAppointmentResponse>> CreateAppointment(int customerId, [FromBody] CreateAppointmentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await CanAccessCustomerAsync(customerId)) return Forbid();
        var appointment = await _customerService.CreateAppointmentAsync(customerId, request);
        return Ok(appointment);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("appointments/{appointmentId:int}/status")]
    public async Task<ActionResult<CustomerAppointmentResponse>> UpdateAppointmentStatus(int appointmentId, [FromBody] UpdateAppointmentStatusRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var appointment = await _customerService.UpdateAppointmentStatusAsync(appointmentId, request);
        return Ok(appointment);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpDelete("appointments/{appointmentId:int}")]
    public async Task<IActionResult> DeleteAppointment(int appointmentId)
    {
        await _customerService.DeleteAppointmentAsync(appointmentId);
        return Ok(new { message = "Appointment deleted successfully" });
    }

    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpGet("{customerId:int}/part-requests")]
    public async Task<ActionResult<IEnumerable<CustomerPartRequestResponse>>> GetPartRequests(int customerId)
    {
        if (!await CanAccessCustomerAsync(customerId)) return Forbid();
        var requests = await _customerService.GetPartRequestsAsync(customerId);
        return Ok(requests);
    }

    [Authorize(Roles = "Customer")]
    [HttpPost("{customerId:int}/part-requests")]
    public async Task<ActionResult<CustomerPartRequestResponse>> CreatePartRequest(int customerId, [FromBody] CreatePartRequestCustomerRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await CanAccessCustomerAsync(customerId)) return Forbid();
        var partRequest = await _customerService.CreatePartRequestAsync(customerId, request);
        return Ok(partRequest);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("part-requests/{requestId:int}/status")]
    public async Task<ActionResult<CustomerPartRequestResponse>> UpdatePartRequestStatus(int requestId, [FromBody] UpdatePartRequestStatusRequest request)
    {
        var partRequest = await _customerService.UpdatePartRequestStatusAsync(requestId, request);
        return Ok(partRequest);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpDelete("part-requests/{requestId:int}")]
    public async Task<IActionResult> DeletePartRequest(int requestId)
    {
        await _customerService.DeletePartRequestAsync(requestId);
        return Ok(new { message = "Part request deleted successfully" });
    }

    [Authorize(Roles = "Admin,Staff,Customer")]
    [HttpGet("{customerId:int}/reviews")]
    public async Task<ActionResult<IEnumerable<CustomerReviewResponse>>> GetReviews(int customerId)
    {
        if (!await CanAccessCustomerAsync(customerId)) return Forbid();
        var reviews = await _customerService.GetReviewsAsync(customerId);
        return Ok(reviews);
    }

    [Authorize(Roles = "Customer")]
    [HttpPost("{customerId:int}/reviews")]
    public async Task<ActionResult<CustomerReviewResponse>> CreateReview(int customerId, [FromBody] CreateReviewRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await CanAccessCustomerAsync(customerId)) return Forbid();
        var review = await _customerService.CreateReviewAsync(customerId, request);
        return Ok(review);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpDelete("reviews/{reviewId:int}")]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        await _customerService.DeleteReviewAsync(reviewId);
        return Ok(new { message = "Review deleted successfully" });
    }

    [HttpGet("search/{query}")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<ActionResult<IEnumerable<CustomerSearchResponse>>> SearchCustomers(string query)
    {
        var customers = await _customerService.SearchCustomersAsync(query);
        return Ok(customers);
    }

    /// <summary>
    /// Generate customer-related insights report (regulars, high spenders, pending credits).
    /// Staff can use this to identify business opportunities and follow up on credit records.
    /// </summary>
    [HttpGet("reports/insights")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<ActionResult<CustomerInsightSummaryResponse>> GetCustomerInsights()
    {
        var insights = await _customerService.GetCustomerInsightsAsync();
        return Ok(insights);
    }

    private async Task<bool> CanAccessCustomerAsync(int customerId)
    {
        if (User.IsInRole("Admin") || User.IsInRole("Staff")) return true;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return false;

        var customer = await _customerRepository.GetByUserIdAsync(userId);
        return customer?.Id == customerId;
    }
}
