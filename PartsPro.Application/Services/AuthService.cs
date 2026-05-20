using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PartsPro.Application.DTOs.Auth;
using PartsPro.Application.Exceptions;
using PartsPro.Application.Interfaces.Services;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PartsPro.Application.Services;

/// <summary>
/// Handles everything related to logging in, signing up, and creating JWT tokens
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly ICustomerRepository _customerRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger<AuthService> logger,
        ICustomerRepository customerRepository,
        IStaffRepository staffRepository,
        IVehicleRepository vehicleRepository)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
        _customerRepository = customerRepository;
        _staffRepository = staffRepository;
        _vehicleRepository = vehicleRepository;
    }

    /// <summary>
    /// Checks the user's email and password, then hands back a JWT token if everything looks good
    /// </summary>
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning($"Login failed: Invalid email attempt for {request.Email}");
            throw new UnauthorizedException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning($"Login failed: Account disabled for {request.Email}");
            throw new UnauthorizedException("User account is disabled");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            _logger.LogWarning($"Login failed: Invalid password for {request.Email}");
            throw new UnauthorizedException("Invalid email or password");
        }

        var token = await GenerateTokenAsync(user);
        var role = await GetUserRoleAsync(user);

        // include customerId in the response for customer users
        int? customerId = null;
        if (role == "Customer")
        {
            var customer = await _customerRepository.GetByUserIdAsync(user.Id);
            customerId = customer?.Id;
        }

        return new LoginResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                CustomerId = customerId,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Role = role,
                IsActive = user.IsActive
            }
        };
    }

    /// <summary>
    /// Lets a customer sign up on their own through the public registration page
    /// </summary>
    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogWarning($"Registration failed: Email {request.Email} already exists");
            throw new ConflictException("User with this email already exists");
        }

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.Phone,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning($"Registration failed: {errors}");
            throw new BadRequestException($"Failed to create user: {errors}");
        }

        await _userManager.AddToRoleAsync(user, "Customer");

        var customer = new Customer
        {
            UserId = user.Id,
            Address = request.Address,
            FullName = request.FullName
        };
        _customerRepository.Create(customer);
        await _customerRepository.SaveChangesAsync();

        if (!string.IsNullOrEmpty(request.PlateNumber) && !string.IsNullOrEmpty(request.VehicleModel))
        {
            var vehicle = new Vehicle
            {
                CustomerId = customer.Id,
                PlateNumber = request.PlateNumber,
                Model = request.VehicleModel,
                Year = request.VehicleYear
            };
            _vehicleRepository.Create(vehicle);
            await _vehicleRepository.SaveChangesAsync();
        }

        var token = await GenerateTokenAsync(user);

        return new LoginResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                CustomerId = customer.Id,  // Return the integer customer ID
                Email = user.Email,
                FullName = user.FullName,
                Role = "Customer",
                IsActive = user.IsActive
            }
        };
    }

    /// <summary>
    /// Creates a new staff account - only admins can do this from the dashboard
    /// </summary>
    public async Task<LoginResponse> RegisterStaffAsync(StaffRegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogWarning($"Staff registration failed: Email {request.Email} already exists");
            throw new ConflictException("User already exists");
        }

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning($"Staff registration failed: {errors}");
            throw new BadRequestException(errors);
        }

        await _userManager.AddToRoleAsync(user, "Staff");

        var staff = new Staff 
        { 
            UserId = user.Id, 
            Department = request.Department 
        };
        _staffRepository.Create(staff);
        await _staffRepository.SaveChangesAsync();

        _logger.LogInformation($"Staff user created successfully: {request.Email}");

        var token = await GenerateTokenAsync(user);

        return new LoginResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = "Staff",
                IsActive = user.IsActive
            }
        };
    }

    /// <summary>
/// Staff uses this to register a walk-in customer at the counter.
/// Staff enters the customer's password from the registration form.
/// </summary>
public async Task<StaffCustomerRegisterResponse> CreateCustomerByStaffAsync(StaffCustomerRegisterRequest request)
{
    var existingUser = await _userManager.FindByEmailAsync(request.Email);
    if (existingUser != null)
    {
        _logger.LogWarning($"Customer creation failed: Email {request.Email} already exists");
        throw new ConflictException("User already exists");
    }

    var user = new ApplicationUser
    {
        Email = request.Email,
        UserName = request.Email,
        FullName = request.FullName,
        PhoneNumber = request.Phone,
        IsActive = true
    };

    var result = await _userManager.CreateAsync(user, request.Password);
    if (!result.Succeeded)
    {
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        _logger.LogWarning($"Customer creation failed: {errors}");
        throw new BadRequestException(errors);
    }

    await _userManager.AddToRoleAsync(user, "Customer");

    var customer = new Customer
    {
        UserId = user.Id,
        Address = request.Address,
        FullName = request.FullName
    };

    _customerRepository.Create(customer);
    await _customerRepository.SaveChangesAsync();

    var vehicle = new Vehicle
    {
        CustomerId = customer.Id,
        PlateNumber = request.PlateNumber,
        Model = request.VehicleModel,
        Year = request.VehicleYear
    };

    if (!string.IsNullOrEmpty(request.VehicleMake))
    {
        vehicle.Notes = $"Make: {request.VehicleMake}";
    }

    _vehicleRepository.Create(vehicle);
    await _vehicleRepository.SaveChangesAsync();

    _logger.LogInformation($"Customer created by staff: {request.Email}");

    return new StaffCustomerRegisterResponse
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        Role = "Customer",
        IsActive = user.IsActive,
        CustomerId = customer.Id
    };
}

    /// <summary>
    /// Builds a JWT token with the user's claims baked in - used after every successful login or signup
    /// </summary>
    private async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        var userRole = await GetUserRoleAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, userRole)
        };

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryMinutes"] ?? "60")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Quick helper to grab the user's role - defaults to "Customer" if somehow they don't have one
    /// </summary>
    private async Task<string> GetUserRoleAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault() ?? "Customer";
    }
}
