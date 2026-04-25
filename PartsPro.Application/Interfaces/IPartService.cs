using PartsPro.Application.DTOs;
 
namespace PartsPro.Application.Interfaces;
 
public interface IPartService
{
    Task<List<PartResponse>> GetAllPartsAsync();
    Task<PartResponse> GetPartByIdAsync(int id);
    Task<List<PartResponse>> GetLowStockPartsAsync();
    Task<PartResponse> CreatePartAsync(PartRequest request);
    Task<PartResponse> UpdatePartAsync(int id, PartRequest request);
    Task DeletePartAsync(int id);
}