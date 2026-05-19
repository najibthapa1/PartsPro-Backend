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

    Task<IEnumerable<CustomerAppointmentResponse>> GetAppointmentsAsync(int customerId);
    Task<CustomerAppointmentResponse> CreateAppointmentAsync(int customerId, CreateAppointmentRequest request);
    Task<CustomerAppointmentResponse> UpdateAppointmentStatusAsync(int appointmentId, UpdateAppointmentStatusRequest request);
    Task DeleteAppointmentAsync(int appointmentId);

    Task<IEnumerable<CustomerPartRequestResponse>> GetPartRequestsAsync(int customerId);
    Task<CustomerPartRequestResponse> CreatePartRequestAsync(int customerId, CreatePartRequestCustomerRequest request);
    Task<CustomerPartRequestResponse> UpdatePartRequestStatusAsync(int requestId, UpdatePartRequestStatusRequest request);
    Task DeletePartRequestAsync(int requestId);

    Task<IEnumerable<CustomerReviewResponse>> GetReviewsAsync(int customerId);
    Task<CustomerReviewResponse> CreateReviewAsync(int customerId, CreateReviewRequest request);
    Task DeleteReviewAsync(int reviewId);

    Task<CustomerInsightSummaryResponse> GetCustomerInsightsAsync();
}