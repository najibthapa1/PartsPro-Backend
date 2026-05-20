using PartsPro.Application.DTOs.Auth;

namespace PartsPro.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
    
    // Staff registration uses its own DTO since it needs a Department field
    Task<LoginResponse> RegisterStaffAsync(StaffRegisterRequest request);
    
    Task<StaffCustomerRegisterResponse> CreateCustomerByStaffAsync(StaffCustomerRegisterRequest request);
}
