using PartsPro.Application.DTOs.Auth;

namespace PartsPro.Application.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Register new customer user
    /// </summary>
    Task<LoginResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Get JWT token for user
    /// </summary>
    Task<string> GenerateTokenAsync(PartsPro.Domain.Entities.ApplicationUser user);

    /// <summary>
    /// Get user role from Identity roles
    /// </summary>
    Task<string> GetUserRoleAsync(PartsPro.Domain.Entities.ApplicationUser user);
}

