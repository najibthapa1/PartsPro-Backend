using PartsPro.Application.DTOs;
 
namespace PartsPro.Application.Interfaces;
 
public interface IVendorService
{
    Task<List<VendorResponse>> GetAllVendorsAsync();
    Task<VendorResponse> GetVendorByIdAsync(int id);
    Task<VendorResponse> CreateVendorAsync(VendorRequest request);
    Task<VendorResponse> UpdateVendorAsync(int id, VendorRequest request);
    Task DeleteVendorAsync(int id);
}