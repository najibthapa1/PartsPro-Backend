using Microsoft.EntityFrameworkCore;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Domain.Entities;
using PartsPro.Infrastructure.Data;

namespace PartsPro.Infrastructure.Repositories;

public class StaffRepository(AppDbContext context) 
    : RepositoryBase<Staff>(context), IStaffRepository
{
    public async Task<Staff?> GetByUserIdAsync(string userId)
    {
        return await Context.Set<Staff>()
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }
}
