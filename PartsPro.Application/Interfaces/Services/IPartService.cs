using PartsPro.Application.DTOs.Parts;

namespace PartsPro.Application.Interfaces.Services;

public interface IPartService
{
    Task<PartResponse> CreatePartAsync(CreatePartRequest request);
    Task<PartResponse> GetPartByIdAsync(int id);
    Task<IEnumerable<PartResponse>> GetAllPartsAsync(int pageNumber, int pageSize);
    Task UpdatePartAsync(int id, CreatePartRequest request);
    Task DeletePartAsync(int id);
    Task<IEnumerable<PartResponse>> SearchPartsAsync(string query);

}
