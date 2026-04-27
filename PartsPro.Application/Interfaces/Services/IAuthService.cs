using PartsPro.Application.DTOs.Auth;

namespace PartsPro.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
    
    // Using the independent Staff DTO
    Task<LoginResponse> RegisterStaffAsync(StaffRegisterRequest request);
    
    Task<UserDto> CreateCustomerByStaffAsync(RegisterRequest request);
}
