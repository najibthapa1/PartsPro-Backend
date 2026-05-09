using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PartsPro.Application.DTOs.Staff;
using PartsPro.Application.Exceptions;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Application.Interfaces.Services;
using PartsPro.Domain.Entities;

namespace PartsPro.Application.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<StaffService> _logger;

    public StaffService(
        IStaffRepository staffRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<StaffService> logger)
    {
        _staffRepository = staffRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<StaffResponse>> GetAllStaffAsync(int pageNumber = 1, int pageSize = 10)
    {
        var staffQuery = _staffRepository.FindAll(trackChanges: false)
            .Include(s => s.User)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var staffList = await staffQuery.ToListAsync();

        return staffList.Select(s => new StaffResponse
        {
            Id = s.Id,
            Department = s.Department,
            CreatedAt = s.CreatedAt,
            UserId = s.UserId,
            FullName = s.User?.FullName ?? string.Empty,
            Email = s.User?.Email ?? string.Empty,
            Phone = s.User?.PhoneNumber,
            IsActive = s.User?.IsActive ?? false
        });
    }

    public async Task<StaffResponse> GetStaffByIdAsync(int id)
    {
        var staff = await _staffRepository.FindByCondition(s => s.Id == id, trackChanges: false)
            .Include(s => s.User)
            .FirstOrDefaultAsync();

        if (staff == null)
            throw new NotFoundException($"Staff with ID {id} not found.");

        return new StaffResponse
        {
            Id = staff.Id,
            Department = staff.Department,
            CreatedAt = staff.CreatedAt,
            UserId = staff.UserId,
            FullName = staff.User?.FullName ?? string.Empty,
            Email = staff.User?.Email ?? string.Empty,
            Phone = staff.User?.PhoneNumber,
            IsActive = staff.User?.IsActive ?? false
        };
    }

    public async Task UpdateStaffAsync(int id, UpdateStaffRequest request)
    {
        var staff = await _staffRepository.FindByCondition(s => s.Id == id, trackChanges: true)
            .Include(s => s.User)
            .FirstOrDefaultAsync();

        if (staff == null)
            throw new NotFoundException($"Staff with ID {id} not found.");

        // Update Staff properties
        staff.Department = request.Department;

        // Update User properties
        if (staff.User != null)
        {
            staff.User.FullName = request.FullName;
            staff.User.Email = request.Email;
            staff.User.UserName = request.Email;
            staff.User.PhoneNumber = request.Phone;

            var result = await _userManager.UpdateAsync(staff.User);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning($"Failed to update staff user: {errors}");
                throw new BadRequestException($"Failed to update user details: {errors}");
            }
        }

        _staffRepository.Update(staff);
        await _staffRepository.SaveChangesAsync();
        
        _logger.LogInformation($"Staff {id} updated successfully.");
    }

    public async Task DeleteStaffAsync(int id)
    {
        var staff = await _staffRepository.FindByCondition(s => s.Id == id, trackChanges: true)
            .Include(s => s.User)
            .FirstOrDefaultAsync();

        if (staff == null)
            throw new NotFoundException($"Staff with ID {id} not found.");

        if (staff.User != null)
        {
            staff.User.IsActive = false;
            var result = await _userManager.UpdateAsync(staff.User);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning($"Failed to deactivate staff user: {errors}");
                throw new BadRequestException($"Failed to deactivate user: {errors}");
            }
        }

        await _staffRepository.SaveChangesAsync();
        
        _logger.LogInformation($"Staff {id} deactivated (soft deleted).");
    }
}
