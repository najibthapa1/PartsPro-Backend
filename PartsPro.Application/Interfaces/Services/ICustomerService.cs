using PartsPro.Application.DTOs.Customers;

namespace PartsPro.Application.Interfaces.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerProfileResponse>> GetAllCustomersAsync(int pageNumber = 1, int pageSize = 10);
    Task<CustomerProfileResponse> GetProfileAsync(int customerId);
    Task UpdateProfileAsync(int customerId, UpdateCustomerRequest request);
    Task AddVehicleAsync(int customerId, AddVehicleRequest request);
    Task<IEnumerable<CustomerVehicleResponse>> GetVehiclesAsync(int customerId);
    Task<CustomerHistoryResponse> GetCustomerHistoryAsync(int customerId);
    Task<IEnumerable<CustomerSearchResponse>> SearchCustomersAsync(string query);
}

