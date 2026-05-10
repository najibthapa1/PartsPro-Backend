using PartsPro.Application.DTOs.Staff;

namespace PartsPro.Application.Interfaces.Services;

public interface IStaffService
{
    Task<IEnumerable<StaffResponse>> GetAllStaffAsync(int pageNumber = 1, int pageSize = 10);
    Task<StaffResponse> GetStaffByIdAsync(int id);
    Task UpdateStaffAsync(int id, UpdateStaffRequest request);
    Task DeleteStaffAsync(int id);
}
