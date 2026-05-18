using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.Application.DTOs.Auth;
using PartsPro.Application.Interfaces.Services;

namespace PartsPro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Handles user login - validates credentials and sends back a JWT token
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Public endpoint for customers to create their own account
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Only admins can use this to add new staff members to the system
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("register-staff")]
    public async Task<ActionResult<LoginResponse>> RegisterStaff(StaffRegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authService.RegisterStaffAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Staff or admin uses this to register a walk-in customer with their vehicle details
    /// </summary>
    [Authorize(Roles = "Staff,Admin")]
    [HttpPost("register-customer-by-staff")]
    public async Task<ActionResult<StaffCustomerRegisterResponse>> CreateCustomerByStaff(StaffCustomerRegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authService.CreateCustomerByStaffAsync(request);
        return Ok(response);
    }
}
