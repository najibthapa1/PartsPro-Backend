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
    /// Authenticate a user and return a JWT token
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
    /// Register a new customer user (Self-registration)
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
    /// Register a new staff member (Admin only)
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
    /// Create a customer user profile (Staff only)
    /// </summary>
    [Authorize(Roles = "Staff,Admin")]
    [HttpPost("register-customer-by-staff")]
    public async Task<ActionResult<UserDto>> CreateCustomerByStaff(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _authService.CreateCustomerByStaffAsync(request);
        return Ok(response);
    }
}
