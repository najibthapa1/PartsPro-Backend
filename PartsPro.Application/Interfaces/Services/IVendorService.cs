using PartsPro.Application.DTOs.Vendors;

namespace PartsPro.Application.Interfaces.Services;

public interface IVendorService
{
    Task<VendorResponse?> GetVendorByIdAsync(int id);
    Task<IEnumerable<VendorResponse>> GetAllVendorsAsync(int pageNumber = 1, int pageSize = 10);
    Task<VendorResponse> CreateVendorAsync(VendorRequest request);
    Task UpdateVendorAsync(int id, VendorRequest request);
    Task DeleteVendorAsync(int id);
}
